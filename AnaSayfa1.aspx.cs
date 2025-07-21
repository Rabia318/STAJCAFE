using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Configuration;
using System.Web.UI;

namespace STAJCAFE
{
    public partial class AnaSayfa1 : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ToplamSiparis();
                ToplamCiro();
                AktifMasalar();
                DisSiparis();
                EnCokSatanlar();
                FavoriMasa();
                HaftalikCiroGrafik();
                HaftalikMusteriGrafik();
            }
        }

        void ToplamSiparis()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM siparisler", conn);
                lblToplamSiparis.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        void ToplamCiro()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT IFNULL(SUM(tutar), 0) FROM siparisler", conn);
                var result = cmd.ExecuteScalar();
                decimal total = result != DBNull.Value && result != null ? Convert.ToDecimal(result) : 0;
                lblToplamCiro.Text = total.ToString("N2");
            }
        }

        void AktifMasalar()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(DISTINCT masaId) FROM siparisler", conn);
                lblAktifMasalar.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        void DisSiparis()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM siparisler WHERE musteriId IS NOT NULL", conn);
                lblDisSiparis.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        void EnCokSatanlar()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT urunAdi AS Urun, SUM(adet) AS SatisSayisi 
                    FROM siparisler 
                    GROUP BY urunAdi 
                    ORDER BY SatisSayisi DESC 
                    LIMIT 5";

                var da = new MySqlDataAdapter(sql, conn);
                var dt = new DataTable();
                da.Fill(dt);

                gvEnCokSatanlar.DataSource = dt;
                gvEnCokSatanlar.DataBind();
            }
        }

        void FavoriMasa()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                // Bu sorgu sadece en çok sipariş verilen masaId'yi getirir (tek değer)
                string sql = @"
                    SELECT masaId
                    FROM siparisler
                    GROUP BY masaId
                    ORDER BY COUNT(*) DESC
                    LIMIT 1";

                var cmd = new MySqlCommand(sql, conn);
                var result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    int masaId = Convert.ToInt32(result);

                    var cmd2 = new MySqlCommand("SELECT konum FROM masalar WHERE masaId = @masaId", conn);
                    cmd2.Parameters.AddWithValue("@masaId", masaId);

                    var konum = cmd2.ExecuteScalar()?.ToString() ?? "Bilinmiyor";
                    lblFavoriMasa.Text = konum;
                }
                else
                {
                    lblFavoriMasa.Text = "Veri yok";
                }
            }
        }

        void HaftalikCiroGrafik()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT DAYNAME(siparisTarihi) AS Gun, IFNULL(SUM(tutar),0) AS Ciro
                    FROM siparisler
                    WHERE siparisTarihi >= CURDATE() - INTERVAL 7 DAY
                    GROUP BY Gun
                    ORDER BY FIELD(Gun, 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday')";

                var da = new MySqlDataAdapter(sql, conn);
                var dt = new DataTable();
                da.Fill(dt);

                chartCiro.Series["Ciro"].Points.DataBind(dt.DefaultView, "Gun", "Ciro", "");
                chartCiro.ChartAreas[0].AxisX.Title = "Günler";
                chartCiro.ChartAreas[0].AxisY.Title = "Ciro (₺)";
            }
        }

        void HaftalikMusteriGrafik()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string sql = @"
                    SELECT DAYNAME(siparisTarihi) AS Gun, COUNT(DISTINCT musteriId) AS MusteriSayisi
                    FROM siparisler
                    WHERE siparisTarihi >= CURDATE() - INTERVAL 7 DAY
                    GROUP BY Gun
                    ORDER BY FIELD(Gun, 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday')";

                var da = new MySqlDataAdapter(sql, conn);
                var dt = new DataTable();
                da.Fill(dt);

                chartMusteri.Series["Müşteri"].Points.DataBind(dt.DefaultView, "Gun", "MusteriSayisi", "");
                chartMusteri.ChartAreas[0].AxisX.Title = "Günler";
                chartMusteri.ChartAreas[0].AxisY.Title = "Müşteri Sayısı";
            }
        }
    }
}
