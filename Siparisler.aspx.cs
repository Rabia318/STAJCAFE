using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Web.UI.WebControls;

namespace STAJCAFE
{
    public partial class Siparisler : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MasalariGetir();
                MusterileriGetir();
                KategorileriGetir();
                SiparisleriGetir();
                btnGuncelle.Enabled = false;
                btnSil.Enabled = false;
            }
        }

        private void MasalariGetir()
        {
            ddlMasa.Items.Clear();
            ddlMasa.Items.Add(new ListItem("Seçiniz", ""));
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT masaId, konum FROM masalar ORDER BY masaId", conn);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string text = $"Masa {dr["masaId"]} - {dr["konum"]}";
                        ddlMasa.Items.Add(new ListItem(text, dr["masaId"].ToString()));
                    }
                }
            }
        }

        private void MusterileriGetir()
        {
            ddlMusteri.Items.Clear();
            ddlMusteri.Items.Add(new ListItem("Seçiniz", ""));
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT musteriId, musteriAdi, musteriSoyadi FROM musteriler ORDER BY musteriId", conn);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        string text = $"{dr["musteriAdi"]} {dr["musteriSoyadi"]}";
                        ddlMusteri.Items.Add(new ListItem(text, dr["musteriId"].ToString()));
                    }
                }
            }
        }

        private void KategorileriGetir()
        {
            ddlKategori.Items.Clear();
            ddlKategori.Items.Add(new ListItem("Seçiniz", ""));
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT katId, kategoriAdi FROM kategoriler ORDER BY katId", conn);
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ddlKategori.Items.Add(new ListItem(dr["kategoriAdi"].ToString(), dr["katId"].ToString()));
                    }
                }
            }
        }

        protected void ddlKategori_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlUrun.Items.Clear();
            ddlUrun.Items.Add(new ListItem("Seçiniz", ""));
            if (!string.IsNullOrEmpty(ddlKategori.SelectedValue))
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT urunId, urunAdi FROM urunler WHERE katId=@katId ORDER BY urunAdi", conn);
                    cmd.Parameters.AddWithValue("@katId", ddlKategori.SelectedValue);
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            ddlUrun.Items.Add(new ListItem(dr["urunAdi"].ToString(), dr["urunId"].ToString()));
                        }
                    }
                }
            }
        }

        private void SiparisleriGetir()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sorgu = @"SELECT siparisId, masaId, musteriId, urunAdi, adet, tutar, siparisTarihi FROM siparisler ORDER BY siparisTarihi DESC";
                MySqlDataAdapter da = new MySqlDataAdapter(sorgu, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvSiparisler.DataSource = dt;
                gvSiparisler.DataBind();
            }
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            // Ürün boşsa veya ikisi de boşsa veya ikisi de doluysa işlem yapılmasın
            bool masaSecildi = !string.IsNullOrEmpty(ddlMasa.SelectedValue);
            bool musteriSecildi = !string.IsNullOrEmpty(ddlMusteri.SelectedValue);

            if (string.IsNullOrEmpty(ddlUrun.SelectedValue) ||
                (!masaSecildi && !musteriSecildi) || (masaSecildi && musteriSecildi))
            {
                return;
            }

            if (!int.TryParse(txtAdet.Text, out int adet) || adet < 1)
            {
                return;
            }

            int urunId = int.Parse(ddlUrun.SelectedValue);
            string urunAdi = ddlUrun.SelectedItem.Text;
            int masaId = masaSecildi ? int.Parse(ddlMasa.SelectedValue) : 0;
            int musteriId = musteriSecildi ? int.Parse(ddlMusteri.SelectedValue) : 0;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                MySqlCommand fiyatCmd = new MySqlCommand("SELECT fiyat FROM urunler WHERE urunId=@urunId", conn);
                fiyatCmd.Parameters.AddWithValue("@urunId", urunId);
                int fiyat = Convert.ToInt32(fiyatCmd.ExecuteScalar());
                int tutar = fiyat * adet;

                MySqlCommand cmd = new MySqlCommand(@"
                    INSERT INTO siparisler (masaId, musteriId, urunId, urunAdi, adet, tutar) 
                    VALUES (@masaId, @musteriId, @urunId, @urunAdi, @adet, @tutar)", conn);

                cmd.Parameters.AddWithValue("@masaId", masaId != 0 ? (object)masaId : DBNull.Value);
                cmd.Parameters.AddWithValue("@musteriId", musteriId != 0 ? (object)musteriId : DBNull.Value);
                cmd.Parameters.AddWithValue("@urunId", urunId);
                cmd.Parameters.AddWithValue("@urunAdi", urunAdi);
                cmd.Parameters.AddWithValue("@adet", adet);
                cmd.Parameters.AddWithValue("@tutar", tutar);

                cmd.ExecuteNonQuery();
            }

            SiparisleriGetir();
            Temizle();
        }

        protected void gvSiparisler_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvSiparisler.SelectedDataKey != null)
            {
                int siparisId = Convert.ToInt32(gvSiparisler.SelectedDataKey.Value);
                ViewState["siparisId"] = siparisId;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM siparisler WHERE siparisId=@id", conn);
                    cmd.Parameters.AddWithValue("@id", siparisId);
                    using (MySqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            ddlMasa.SelectedValue = dr["masaId"] != DBNull.Value ? dr["masaId"].ToString() : "";
                            ddlMusteri.SelectedValue = dr["musteriId"] != DBNull.Value ? dr["musteriId"].ToString() : "";
                            ddlKategori.SelectedIndex = 0;
                            ddlUrun.Items.Clear();
                            ddlUrun.Items.Add(new ListItem("Seçiniz", ""));

                            if (dr["urunId"] != DBNull.Value)
                            {
                                int urunId = Convert.ToInt32(dr["urunId"]);
                                using (MySqlConnection conn2 = new MySqlConnection(connStr))
                                {
                                    conn2.Open();
                                    MySqlCommand cmdKat = new MySqlCommand("SELECT katId FROM urunler WHERE urunId=@urunId", conn2);
                                    cmdKat.Parameters.AddWithValue("@urunId", urunId);
                                    var katIdObj = cmdKat.ExecuteScalar();
                                    if (katIdObj != null)
                                    {
                                        string katId = katIdObj.ToString();
                                        ddlKategori.SelectedValue = katId;

                                        MySqlCommand cmdUrun = new MySqlCommand("SELECT urunId, urunAdi FROM urunler WHERE katId=@katId ORDER BY urunAdi", conn2);
                                        cmdUrun.Parameters.AddWithValue("@katId", katId);
                                        using (MySqlDataReader drUrun = cmdUrun.ExecuteReader())
                                        {
                                            while (drUrun.Read())
                                            {
                                                ddlUrun.Items.Add(new ListItem(drUrun["urunAdi"].ToString(), drUrun["urunId"].ToString()));
                                            }
                                        }
                                    }
                                }

                                ddlUrun.SelectedValue = urunId.ToString();
                            }

                            txtAdet.Text = dr["adet"].ToString();

                            btnGuncelle.Enabled = true;
                            btnSil.Enabled = true;
                            btnEkle.Enabled = false;
                        }
                    }
                }
            }
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (ViewState["siparisId"] == null)
                return;

            int siparisId = (int)ViewState["siparisId"];

            if (!int.TryParse(txtAdet.Text, out int adet) || adet < 1)
                return;

            int urunId = int.Parse(ddlUrun.SelectedValue);
            string urunAdi = ddlUrun.SelectedItem.Text;

            int masaId = !string.IsNullOrEmpty(ddlMasa.SelectedValue) ? int.Parse(ddlMasa.SelectedValue) : 0;
            int musteriId = !string.IsNullOrEmpty(ddlMusteri.SelectedValue) ? int.Parse(ddlMusteri.SelectedValue) : 0;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                MySqlCommand fiyatCmd = new MySqlCommand("SELECT fiyat FROM urunler WHERE urunId=@urunId", conn);
                fiyatCmd.Parameters.AddWithValue("@urunId", urunId);
                int fiyat = Convert.ToInt32(fiyatCmd.ExecuteScalar());
                int tutar = fiyat * adet;

                MySqlCommand cmd = new MySqlCommand(@"
                    UPDATE siparisler SET masaId=@masaId, musteriId=@musteriId, urunId=@urunId, urunAdi=@urunAdi, adet=@adet, tutar=@tutar
                    WHERE siparisId=@siparisId", conn);

                cmd.Parameters.AddWithValue("@masaId", masaId != 0 ? (object)masaId : DBNull.Value);
                cmd.Parameters.AddWithValue("@musteriId", musteriId != 0 ? (object)musteriId : DBNull.Value);
                cmd.Parameters.AddWithValue("@urunId", urunId);
                cmd.Parameters.AddWithValue("@urunAdi", urunAdi);
                cmd.Parameters.AddWithValue("@adet", adet);
                cmd.Parameters.AddWithValue("@tutar", tutar);
                cmd.Parameters.AddWithValue("@siparisId", siparisId);

                cmd.ExecuteNonQuery();
            }

            SiparisleriGetir();
            Temizle();
        }

        protected void btnSil_Click(object sender, EventArgs e)
        {
            if (ViewState["siparisId"] == null)
                return;

            int siparisId = (int)ViewState["siparisId"];

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("DELETE FROM siparisler WHERE siparisId=@siparisId", conn);
                cmd.Parameters.AddWithValue("@siparisId", siparisId);
                cmd.ExecuteNonQuery();
            }

            SiparisleriGetir();
            Temizle();
        }

        private void Temizle()
        {
            ddlMasa.SelectedIndex = 0;
            ddlMusteri.SelectedIndex = 0;
            ddlKategori.SelectedIndex = 0;
            ddlUrun.Items.Clear();
            ddlUrun.Items.Add(new ListItem("Seçiniz", ""));
            txtAdet.Text = "1";
            btnEkle.Enabled = true;
            btnGuncelle.Enabled = false;
            btnSil.Enabled = false;
            ViewState["siparisId"] = null;
            gvSiparisler.SelectedIndex = -1;
        }

        protected void gvSiparisler_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSiparisler.PageIndex = e.NewPageIndex;
            SiparisleriGetir();
        }
    }
}
