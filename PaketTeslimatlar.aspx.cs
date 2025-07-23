using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace STAJCAFE
{
    public partial class PaketTeslimatlar : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SiparisleriGetir();
            }
        }

        private void SiparisleriGetir()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = @"
                    SELECT 
                        s.siparisId,
                        s.musteriId,
                        s.masaId,
                        m.musteriAdi,
                        m.musteriSoyadi,
                        m.musteriNumarasi,
                        CONCAT(a.il, ', ', a.ilce, ', ', a.mahalle, ', ', a.sokak) AS adres,
                        GROUP_CONCAT(CONCAT(u.urunAdi, ' x', s.adet) SEPARATOR ', ') AS urunler,
                        SUM(u.fiyat * s.adet) AS toplamTutar,
                        s.durum,
                        s.kuryeId
                    FROM siparisler s
                    LEFT JOIN musteriler m ON s.musteriId = m.musteriId
                    LEFT JOIN adresler a ON m.adresId = a.adresId
                    JOIN urunler u ON s.urunId = u.urunId
                    WHERE s.musteriId IS NOT NULL
                    GROUP BY s.siparisId
                    ORDER BY s.siparisId";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptTeslimatlar.DataSource = dt;
                rptTeslimatlar.DataBind();
            }
        }

        protected void rptTeslimatlar_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddlKuryeler = (DropDownList)e.Item.FindControl("ddlKuryeler");
                RadioButtonList rblDurum = (RadioButtonList)e.Item.FindControl("rblDurum");
                Image imgDurum = (Image)e.Item.FindControl("imgDurum");
                Label litUyari = (Label)e.Item.FindControl("litUyari");

                litUyari.Text = "";

                DataRowView drv = (DataRowView)e.Item.DataItem;

                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string kuryeQuery = "SELECT kuryeId, kuryeAdi FROM kuryeler ORDER BY kuryeAdi";
                    MySqlCommand cmd = new MySqlCommand(kuryeQuery, conn);
                    MySqlDataReader dr = cmd.ExecuteReader();

                    ddlKuryeler.DataSource = dr;
                    ddlKuryeler.DataTextField = "kuryeAdi";
                    ddlKuryeler.DataValueField = "kuryeId";
                    ddlKuryeler.DataBind();
                }

                if (drv["kuryeId"] != DBNull.Value)
                {
                    ddlKuryeler.SelectedValue = drv["kuryeId"].ToString();
                }
                else
                {
                    ddlKuryeler.Items.Insert(0, new ListItem("-- Kurye Seçiniz --", ""));
                }

                if (drv["durum"] != DBNull.Value)
                {
                    string durum = drv["durum"].ToString();
                    rblDurum.SelectedValue = durum;
                    imgDurum.ImageUrl = GetAnimation(durum);
                }
            }
        }

        protected void DurumGuncelle(object sender, EventArgs e)
        {
            RadioButtonList rbl = (RadioButtonList)sender;
            RepeaterItem item = (RepeaterItem)rbl.NamingContainer;

            HiddenField hfSiparisId = (HiddenField)item.FindControl("hfSiparisId");
            int siparisId = Convert.ToInt32(hfSiparisId.Value);

            string secilenDurum = rbl.SelectedValue;

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                if (secilenDurum == "Teslim Edildi")
                {
                    // Sipariş bilgilerini al
                    string selectQuery = @"SELECT musteriId, masaId, urunId, adet FROM siparisler WHERE siparisId = @siparisId";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@siparisId", siparisId);

                    int musteriId = 0;
                    int masaId = 0;
                    int urunId = 0;
                    int adet = 0;

                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            musteriId = reader["musteriId"] != DBNull.Value ? Convert.ToInt32(reader["musteriId"]) : 0;
                            masaId = reader["masaId"] != DBNull.Value ? Convert.ToInt32(reader["masaId"]) : 0;
                            urunId = reader["urunId"] != DBNull.Value ? Convert.ToInt32(reader["urunId"]) : 0;
                            adet = reader["adet"] != DBNull.Value ? Convert.ToInt32(reader["adet"]) : 0;
                        }
                    }

                    // Eğer masaId 0 ise null gönder
                    object masaIdParam = masaId != 0 ? (object)masaId : DBNull.Value;

                    // Siparişi siparisGecmisi tablosuna ekle (masaId null olabilir)
                    string insertQuery = @"INSERT INTO siparisGecmisi (musteriId, masaId, urunId, adet, tarih) 
                                           VALUES (@musteriId, @masaId, @urunId, @adet, @tarih)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@musteriId", musteriId);
                    insertCmd.Parameters.AddWithValue("@masaId", masaIdParam); // null olabilir
                    insertCmd.Parameters.AddWithValue("@urunId", urunId);
                    insertCmd.Parameters.AddWithValue("@adet", adet);
                    insertCmd.Parameters.AddWithValue("@tarih", DateTime.Now);
                    insertCmd.ExecuteNonQuery();

                    // Ana sipariş tablosundan siparişi sil
                    string deleteQuery = "DELETE FROM siparisler WHERE siparisId = @siparisId";
                    MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, conn);
                    deleteCmd.Parameters.AddWithValue("@siparisId", siparisId);
                    deleteCmd.ExecuteNonQuery();
                }
                else
                {
                    // Diğer durumlarda sadece durum güncellemesi yap
                    string query = "UPDATE siparisler SET durum = @durum WHERE siparisId = @siparisId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@durum", secilenDurum);
                    cmd.Parameters.AddWithValue("@siparisId", siparisId);
                    cmd.ExecuteNonQuery();
                }
            }

            SiparisleriGetir();
        }

        protected void btnGuncelle_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;

            HiddenField hfSiparisId = (HiddenField)item.FindControl("hfSiparisId");
            DropDownList ddlKuryeler = (DropDownList)item.FindControl("ddlKuryeler");
            Label litUyari = (Label)item.FindControl("litUyari");

            int siparisId = Convert.ToInt32(hfSiparisId.Value);

            litUyari.Text = "";

            if (string.IsNullOrEmpty(ddlKuryeler.SelectedValue))
            {
                litUyari.Text = "Lütfen bir kurye seçiniz.";
                return;
            }

            int kuryeId = int.Parse(ddlKuryeler.SelectedValue);

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "UPDATE siparisler SET kuryeId = @kuryeId WHERE siparisId = @siparisId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@kuryeId", kuryeId);
                cmd.Parameters.AddWithValue("@siparisId", siparisId);
                cmd.ExecuteNonQuery();
            }

            // Güncelleme sonrası popup'taki kurye detaylarını güncellemek için detayı tekrar göster
            Button btnDetay = item.FindControl("btnDetay") as Button;
            if (btnDetay != null)
            {
                btnDetay_Click(btnDetay, EventArgs.Empty);
            }

            SiparisleriGetir();
        }

        protected void btnDetay_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int siparisId = Convert.ToInt32(btn.CommandArgument);

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();

                string query = @"
                    SELECT 
                        m.musteriAdi, m.musteriSoyadi, m.musteriNumarasi,
                        CONCAT(a.il, ', ', a.ilce, ', ', a.mahalle, ', ', a.sokak) AS adres,
                        k.kuryeAdi, k.telefon AS kuryeTel, k.aracBilgisi,
                        GROUP_CONCAT(CONCAT(u.urunAdi, ' x', s.adet) SEPARATOR ', ') AS urunler,
                        SUM(u.fiyat * s.adet) AS toplamTutar
                    FROM siparisler s
                    LEFT JOIN musteriler m ON s.musteriId = m.musteriId
                    LEFT JOIN adresler a ON m.adresId = a.adresId
                    LEFT JOIN kuryeler k ON s.kuryeId = k.kuryeId
                    JOIN urunler u ON s.urunId = u.urunId
                    WHERE s.siparisId = @siparisId
                    GROUP BY s.siparisId, m.musteriAdi, m.musteriSoyadi, m.musteriNumarasi, a.il, a.ilce, a.mahalle, a.sokak,
                             k.kuryeAdi, k.telefon, k.aracBilgisi";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@siparisId", siparisId);

                MySqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendFormat("<b>Müşteri:</b> {0} {1}<br/>", dr["musteriAdi"], dr["musteriSoyadi"]);
                    sb.AppendFormat("<b>Telefon:</b> {0}<br/>", dr["musteriNumarasi"]);
                    sb.AppendFormat("<b>Adres:</b> {0}<br/>", dr["adres"]);
                    sb.AppendFormat("<b>Kurye:</b> {0}<br/>", dr["kuryeAdi"] != DBNull.Value ? dr["kuryeAdi"] : "Atanmadı");
                    sb.AppendFormat("<b>Kurye Telefon:</b> {0}<br/>", dr["kuryeTel"] != DBNull.Value ? dr["kuryeTel"] : "-");
                    sb.AppendFormat("<b>Araç Bilgisi:</b> {0}<br/>", dr["aracBilgisi"] != DBNull.Value ? dr["aracBilgisi"] : "-");
                    sb.AppendFormat("<b>Ürünler:</b> {0}<br/>", dr["urunler"]);
                    sb.AppendFormat("<b>Toplam Tutar:</b> {0} ₺<br/>", dr["toplamTutar"]);

                    litDetaylar.Text = sb.ToString();
                }
                else
                {
                    litDetaylar.Text = "Detay bulunamadı.";
                }

                ModalPopupExtender1.Show();
            }
        }

        protected void btnKapat_Click(object sender, EventArgs e)
        {
            ModalPopupExtender1.Hide();
        }

        public string GetAnimation(string durum)
        {
            if (durum == "Hazırlanıyor") return "~/images/hazirlaniyor.gif";
            if (durum == "Yolda") return "~/images/yolda.gif";
            if (durum == "Teslim Edildi") return "~/images/teslimedildi.gif";
            return "";
        }
    }
}
