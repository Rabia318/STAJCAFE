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
                FavoriMasa();
                EnCokSatanlar();
                HaftalikCiroGrafik();
                HaftalikMusteriGrafik();
            }
        }

        void ToplamSiparis()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT COUNT(*) FROM siparisGecmisi", conn);
                lblToplamSiparis.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        void ToplamCiro()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT IFNULL(SUM(s.adet * u.fiyat), 0)
                    FROM siparisGecmisi s
                    JOIN urunler u ON s.urunId = u.urunId";
                var cmd = new MySqlCommand(sql, conn);
                var result = cmd.ExecuteScalar();
                decimal total = (result != DBNull.Value && result != null) ? Convert.ToDecimal(result) : 0;
                lblToplamCiro.Text = total.ToString("N2") + " ₺";
            }
        }

        void AktifMasalar()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT COUNT(DISTINCT masaId)
                    FROM siparisGecmisi
                    WHERE tarih >= CURDATE() - INTERVAL 1 DAY";
                var cmd = new MySqlCommand(sql, conn);
                lblAktifMasalar.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        void DisSiparis()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT COUNT(*)
                    FROM siparisGecmisi
                    WHERE masaId IS NULL AND musteriId IS NOT NULL";
                var cmd = new MySqlCommand(sql, conn);
                lblDisSiparis.Text = cmd.ExecuteScalar()?.ToString() ?? "0";
            }
        }

        void FavoriMasa()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT masaId
                    FROM siparisGecmisi
                    WHERE masaId IS NOT NULL
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

        void EnCokSatanlar()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT u.urunAdi AS Urun, SUM(s.adet) AS SatisSayisi
                    FROM siparisGecmisi s
                    JOIN urunler u ON s.urunId = u.urunId
                    GROUP BY u.urunAdi
                    ORDER BY SatisSayisi DESC
                    LIMIT 5";
                var da = new MySqlDataAdapter(sql, conn);
                var dt = new DataTable();
                da.Fill(dt);
                gvEnCokSatanlar.DataSource = dt;
                gvEnCokSatanlar.DataBind();
            }
        }

        void HaftalikCiroGrafik()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT DATE(s.tarih) AS Tarih, IFNULL(SUM(s.adet * u.fiyat), 0) AS Ciro
                    FROM siparisGecmisi s
                    JOIN urunler u ON s.urunId = u.urunId
                    WHERE s.tarih >= CURDATE() - INTERVAL 6 DAY
                    GROUP BY Tarih
                    ORDER BY Tarih";
                var da = new MySqlDataAdapter(sql, conn);
                var dt = new DataTable();
                da.Fill(dt);
                chartCiro.Series["Ciro"].Points.DataBind(dt.DefaultView, "Tarih", "Ciro", "");
                chartCiro.ChartAreas[0].AxisX.Title = "Tarih";
                chartCiro.ChartAreas[0].AxisY.Title = "Ciro (₺)";
            }
        }

        void HaftalikMusteriGrafik()
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT DATE(tarih) AS Tarih, COUNT(DISTINCT musteriId) AS MusteriSayisi
                    FROM siparisGecmisi
                    WHERE tarih >= CURDATE() - INTERVAL 6 DAY
                    GROUP BY Tarih
                    ORDER BY Tarih";
                var da = new MySqlDataAdapter(sql, conn);
                var dt = new DataTable();
                da.Fill(dt);
                chartMusteri.Series["Müşteri"].Points.DataBind(dt.DefaultView, "Tarih", "MusteriSayisi", "");
                chartMusteri.ChartAreas[0].AxisX.Title = "Tarih";
                chartMusteri.ChartAreas[0].AxisY.Title = "Müşteri Sayısı";
            }
        }
    }
}
