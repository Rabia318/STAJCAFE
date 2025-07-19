using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace STAJCAFE
{
    public partial class RezervasyonIslemleri : System.Web.UI.Page
    {
        private HashSet<int> SeciliMasalar
        {
            get
            {
                if (ViewState["SeciliMasalar"] == null)
                    ViewState["SeciliMasalar"] = new HashSet<int>();
                return (HashSet<int>)ViewState["SeciliMasalar"];
            }
            set
            {
                ViewState["SeciliMasalar"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MusterileriGetir();
                MasalariOlustur();
                RezervasyonlariGetir();
                btnGuncelle.Enabled = false;
                btnSil.Enabled = false;
            }
            else
            {
                // Postback'te de masa butonlarını oluştur ve seçili olanları işaretle
                MasalariOlustur();
            }
        }

        private void MusterileriGetir()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = "SELECT musteriId, CONCAT(musteriAdi, ' ', musteriSoyadi) AS AdSoyad FROM musteriler ORDER BY musteriAdi";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    ddlMusteri.Items.Clear();
                    ddlMusteri.Items.Add(new ListItem("-- Seçiniz --", ""));
                    while (dr.Read())
                    {
                        ddlMusteri.Items.Add(new ListItem(dr["AdSoyad"].ToString(), dr["musteriId"].ToString()));
                    }
                }
            }
        }

        // Masaları oluştur - parametre olarak dolu masaId listesi alır
        private void MasalariOlustur(List<int> doluMasalar)
        {
            phMasalar.Controls.Clear();

            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = "SELECT masaId, konum, kapasite FROM masalar ORDER BY konum, masaId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();
                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int masaId = Convert.ToInt32(dr["masaId"]);
                        string konum = dr["konum"].ToString();
                        int kapasite = Convert.ToInt32(dr["kapasite"]);

                        Button btn = new Button();
                        btn.ID = "btnMasa_" + masaId;
                        btn.Text = $"{konum} ({kapasite})";
                        btn.CssClass = "masa-btn";
                        btn.CommandArgument = masaId.ToString();
                        btn.Click += MasaButton_Click;

                        if (doluMasalar != null && doluMasalar.Contains(masaId))
                        {
                            btn.Enabled = false;
                            btn.CssClass += " masa-dolu";
                        }
                        else if (SeciliMasalar.Contains(masaId))
                        {
                            btn.CssClass += " masa-secili";
                        }

                        phMasalar.Controls.Add(btn);
                    }
                }
            }
        }

        // Overload: parametresiz versiyon (tüm masalar aktif)
        private void MasalariOlustur()
        {
            MasalariOlustur(new List<int>());
        }

        protected void MasaButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            int masaId = int.Parse(btn.CommandArgument);
            var secili = SeciliMasalar;

            if (secili.Contains(masaId))
            {
                secili.Remove(masaId);
            }
            else
            {
                // Tek masa seçilecekse, önce temizle
                secili.Clear();
                secili.Add(masaId);
            }

            SeciliMasalar = secili;

            // Masa butonlarını yeniden oluştur ve güncelle
            MasalariOlustur();
        }

        private void RezervasyonlariGetir()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = @"
                SELECT 
                    r.rezervasyonId,
                    CONCAT(m.musteriAdi, ' ', m.musteriSoyadi) AS musteriAdiSoyadi,
                    CONCAT(ma.konum, ' (', ma.kapasite, ')') AS masaBilgi,
                    DATE_FORMAT(r.tarih, '%Y-%m-%d') AS tarih,
                    TIME_FORMAT(r.saatBas, '%H:%i') AS saatBas,
                    TIME_FORMAT(r.saatBit, '%H:%i') AS saatBit,
                    r.kisiSayisi
                FROM rezervasyon r
                LEFT JOIN musteriler m ON r.musteriId = m.musteriId
                LEFT JOIN masalar ma ON r.masaId = ma.masaId
                ORDER BY r.tarih DESC, r.saatBas DESC";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvRezervasyonlar.DataSource = dt;
                gvRezervasyonlar.DataBind();
            }
        }

        protected void gvRezervasyonlar_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRezervasyonlar.PageIndex = e.NewPageIndex;
            RezervasyonlariGetir();
        }

        protected void gvRezervasyonlar_SelectedIndexChanged(object sender, EventArgs e)
        {
            int rezervasyonId = Convert.ToInt32(gvRezervasyonlar.SelectedDataKey.Value);

            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = @"SELECT * FROM rezervasyon WHERE rezervasyonId=@rezId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@rezId", rezervasyonId);
                conn.Open();

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        ddlMusteri.SelectedValue = dr["musteriId"].ToString();
                        txtKisiSayisi.Text = dr["kisiSayisi"].ToString();
                        txtTarih.Text = Convert.ToDateTime(dr["tarih"]).ToString("yyyy-MM-dd");
                        txtSaatBas.Text = dr["saatBas"].ToString();
                        txtSaatBit.Text = dr["saatBit"].ToString();

                        // Seçili masa
                        int masaId = dr["masaId"] != DBNull.Value ? Convert.ToInt32(dr["masaId"]) : 0;
                        var secili = new HashSet<int>();
                        if (masaId != 0) secili.Add(masaId);
                        SeciliMasalar = secili;

                        MasalariOlustur();

                        btnGuncelle.Enabled = true;
                        btnSil.Enabled = true;
                        btnEkle.Enabled = false;

                        ViewState["SeciliRezervasyonId"] = rezervasyonId;
                    }
                }
            }
        }

        // Masa uygunluğunu kontrol eden metod
        private bool MasaRezervasyonuMusaitMi(int masaId, DateTime tarih, TimeSpan saatBas, TimeSpan saatBit, int? rezervasyonId = null)
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = @"
                    SELECT COUNT(*) FROM rezervasyon
                    WHERE masaId = @masaId
                    AND tarih = @tarih
                    AND NOT (@saatBit <= saatBas OR @saatBas >= saatBit)";

                if (rezervasyonId.HasValue)
                    query += " AND rezervasyonId != @rezervasyonId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@masaId", masaId);
                cmd.Parameters.AddWithValue("@tarih", tarih.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@saatBas", saatBas);
                cmd.Parameters.AddWithValue("@saatBit", saatBit);

                if (rezervasyonId.HasValue)
                    cmd.Parameters.AddWithValue("@rezervasyonId", rezervasyonId.Value);

                conn.Open();
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count == 0;
            }
        }

        // Seçilen tarih ve saat aralığında dolu masaların listesi
        private List<int> DolumasaListesiGetir(DateTime tarih, TimeSpan saatBas, TimeSpan saatBit)
        {
            List<int> doluMasalar = new List<int>();
            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = @"
                    SELECT DISTINCT masaId FROM rezervasyon
                    WHERE tarih = @tarih
                    AND NOT (@saatBit <= saatBas OR @saatBas >= saatBit)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@tarih", tarih.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@saatBas", saatBas);
                cmd.Parameters.AddWithValue("@saatBit", saatBit);

                using (MySqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        doluMasalar.Add(dr.GetInt32("masaId"));
                    }
                }
            }

            return doluMasalar;
        }

        protected void btnMusaitMasalariGoster_Click(object sender, EventArgs e)
        {
            DateTime tarih;
            TimeSpan saatBas, saatBit;

            if (!DateTime.TryParse(txtTarih.Text, out tarih) ||
                !TimeSpan.TryParse(txtSaatBas.Text, out saatBas) ||
                !TimeSpan.TryParse(txtSaatBit.Text, out saatBit))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen geçerli tarih ve saat aralığı giriniz.');", true);
                return;
            }

            if (saatBit <= saatBas)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Bitiş saati başlangıç saatinden büyük olmalı.');", true);
                return;
            }

            List<int> doluMasalar = DolumasaListesiGetir(tarih, saatBas, saatBit);

            MasalariOlustur(doluMasalar);
        }

        protected void btnEkle_Click(object sender, EventArgs e)
        {
            if (!VerileriKontrolEt()) return;

            int masaId = SeciliMasalar.First();
            DateTime tarih = DateTime.Parse(txtTarih.Text);
            TimeSpan saatBas = TimeSpan.Parse(txtSaatBas.Text);
            TimeSpan saatBit = TimeSpan.Parse(txtSaatBit.Text);

            if (!MasaRezervasyonuMusaitMi(masaId, tarih, saatBas, saatBit))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Seçilen masa, belirtilen tarih ve saat aralığında doludur. Lütfen farklı bir zaman veya masa seçiniz.');", true);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = @"INSERT INTO rezervasyon (masaId, musteriId, tarih, saatBas, saatBit, kisiSayisi) 
                                 VALUES (@masaId, @musteriId, @tarih, @saatBas, @saatBit, @kisiSayisi)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@masaId", masaId);
                cmd.Parameters.AddWithValue("@musteriId", ddlMusteri.SelectedValue);
                cmd.Parameters.AddWithValue("@tarih", txtTarih.Text);
                cmd.Parameters.AddWithValue("@saatBas", txtSaatBas.Text);
                cmd.Parameters.AddWithValue("@saatBit", txtSaatBit.Text);
                cmd.Parameters.AddWithValue("@kisiSayisi", Convert.ToInt32(txtKisiSayisi.Text));

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TemizleForm();
            RezervasyonlariGetir();
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Rezervasyon başarıyla eklendi.');", true);
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            if (!VerileriKontrolEt()) return;

            if (ViewState["SeciliRezervasyonId"] == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen önce bir rezervasyon seçiniz.');", true);
                return;
            }

            int rezervasyonId = (int)ViewState["SeciliRezervasyonId"];
            int masaId = SeciliMasalar.First();
            DateTime tarih = DateTime.Parse(txtTarih.Text);
            TimeSpan saatBas = TimeSpan.Parse(txtSaatBas.Text);
            TimeSpan saatBit = TimeSpan.Parse(txtSaatBit.Text);

            if (!MasaRezervasyonuMusaitMi(masaId, tarih, saatBas, saatBit, rezervasyonId))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Seçilen masa, belirtilen tarih ve saat aralığında doludur. Lütfen farklı bir zaman veya masa seçiniz.');", true);
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = @"UPDATE rezervasyon SET 
                                    masaId=@masaId, musteriId=@musteriId, tarih=@tarih, saatBas=@saatBas, saatBit=@saatBit, kisiSayisi=@kisiSayisi 
                                 WHERE rezervasyonId=@rezervasyonId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@masaId", masaId);
                cmd.Parameters.AddWithValue("@musteriId", ddlMusteri.SelectedValue);
                cmd.Parameters.AddWithValue("@tarih", txtTarih.Text);
                cmd.Parameters.AddWithValue("@saatBas", txtSaatBas.Text);
                cmd.Parameters.AddWithValue("@saatBit", txtSaatBit.Text);
                cmd.Parameters.AddWithValue("@kisiSayisi", Convert.ToInt32(txtKisiSayisi.Text));
                cmd.Parameters.AddWithValue("@rezervasyonId", rezervasyonId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TemizleForm();
            RezervasyonlariGetir();
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Rezervasyon başarıyla güncellendi.');", true);
        }

        protected void btnSil_Click(object sender, EventArgs e)
        {
            if (ViewState["SeciliRezervasyonId"] == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen önce bir rezervasyon seçiniz.');", true);
                return;
            }

            int rezervasyonId = (int)ViewState["SeciliRezervasyonId"];

            string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = "DELETE FROM rezervasyon WHERE rezervasyonId=@rezervasyonId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@rezervasyonId", rezervasyonId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }

            TemizleForm();
            RezervasyonlariGetir();
            ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Rezervasyon başarıyla silindi.');", true);
        }

        protected void btnTemizle_Click(object sender, EventArgs e)
        {
            TemizleForm();
        }

        private void TemizleForm()
        {
            ddlMusteri.SelectedIndex = 0;
            txtKisiSayisi.Text = "1";
            txtTarih.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtSaatBas.Text = "12:00";
            txtSaatBit.Text = "13:00";
            SeciliMasalar = new HashSet<int>();
            MasalariOlustur();
            btnGuncelle.Enabled = false;
            btnSil.Enabled = false;
            btnEkle.Enabled = true;
            ViewState["SeciliRezervasyonId"] = null;
        }

        private bool VerileriKontrolEt()
        {
            if (string.IsNullOrEmpty(ddlMusteri.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen müşteri seçiniz.');", true);
                return false;
            }

            if (!int.TryParse(txtKisiSayisi.Text, out int kisiSayisi) || kisiSayisi <= 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Geçerli bir kişi sayısı giriniz.');", true);
                return false;
            }

            if (!DateTime.TryParse(txtTarih.Text, out _))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Geçerli bir tarih giriniz.');", true);
                return false;
            }

            if (!TimeSpan.TryParse(txtSaatBas.Text, out TimeSpan saatBas) || !TimeSpan.TryParse(txtSaatBit.Text, out TimeSpan saatBit))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Saatleri HH:mm formatında giriniz.');", true);
                return false;
            }

            if (saatBit <= saatBas)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Bitiş saati başlangıç saatinden büyük olmalı.');", true);
                return false;
            }

            if (SeciliMasalar.Count != 1)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alert", "alert('Lütfen bir masa seçiniz.');", true);
                return false;
            }

            return true;
        }

        protected void ddlMusteri_SelectedIndexChanged(object sender, EventArgs e)
        {
            SeciliMasalar = new HashSet<int>();
            MasalariOlustur();
        }
    }
}