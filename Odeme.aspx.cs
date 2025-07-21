using System;
using System.Configuration;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace STAJCAFE
{
    public partial class Odeme : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;
        int masaId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["masaId"] != null && int.TryParse(Request.QueryString["masaId"], out masaId))
                {
                    lblMasaBilgi.Text = "Ödeme yapılacak masa: " + masaId;
                }
                else
                {
                    Response.Redirect("Masalar.aspx");
                }
            }
        }

        protected void ddlOdemeYontemi_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlKartBilgileri.Visible = ddlOdemeYontemi.SelectedValue == "Kart";
            lblMesaj.Text = "";
        }

        protected void btnOdemeTamamla_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(Request.QueryString["masaId"], out masaId) || masaId == 0)
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Geçersiz masa bilgisi!";
                return;
            }

            if (ddlOdemeYontemi.SelectedValue == "Kart" && !KartBilgileriGecerliMi())
            {
                lblMesaj.ForeColor = System.Drawing.Color.Red;
                lblMesaj.Text = "Kart bilgilerini doğru doldurun!";
                return;
            }

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                MySqlTransaction tran = conn.BeginTransaction();
                try
                {
                    // Siparişleri geçmişe aktar
                    string insertSql = @"
                        INSERT INTO siparisGecmisi (masaId, urunId, adet, tarih)
                        SELECT masaId, urunId, adet, NOW()
                        FROM siparisler
                        WHERE masaId = @masaId";
                    MySqlCommand insertCmd = new MySqlCommand(insertSql, conn, tran);
                    insertCmd.Parameters.AddWithValue("@masaId", masaId);
                    insertCmd.ExecuteNonQuery();

                    // Siparişleri sil
                    string deleteSql = "DELETE FROM siparisler WHERE masaId = @masaId";
                    MySqlCommand deleteCmd = new MySqlCommand(deleteSql, conn, tran);
                    deleteCmd.Parameters.AddWithValue("@masaId", masaId);
                    deleteCmd.ExecuteNonQuery();

                    tran.Commit();

                    // Başarılı mesaj göster
                    lblMesaj.ForeColor = System.Drawing.Color.Green;
                    lblMesaj.Text = "Ödeme başarıyla gerçekleşti.";
                    btnOdemeTamamla.Enabled = false;  // Butonu devre dışı bırak

                    // İstersen burada ek işlemler yapabilirsin (örneğin 3 sn sonra yönlendirme)
                    // Response.AddHeader("REFRESH", "3;URL=Masalar.aspx");
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    lblMesaj.ForeColor = System.Drawing.Color.Red;
                    lblMesaj.Text = "Ödeme sırasında hata oluştu: " + ex.Message;
                }
            }
        }

        private bool KartBilgileriGecerliMi()
        {
            string kartNo = txtKartNo.Text.Trim();
            string sonKullanma = txtSonKullanma.Text.Trim();
            string cvv = txtCvv.Text.Trim();

            if (!Regex.IsMatch(kartNo, @"^\d{16}$"))
                return false;

            if (!Regex.IsMatch(sonKullanma, @"^(0[1-9]|1[0-2])\/\d{2}$"))
                return false;

            if (!Regex.IsMatch(cvv, @"^\d{3}$"))
                return false;

            return true;
        }
    }
}
