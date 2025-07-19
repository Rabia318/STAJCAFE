using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace STAJCAFE
{
    public partial class Masalar : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;
        int secilenMasaId;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MasalariGetir();
            }
        }

        private void MasalariGetir()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT masaId, konum, 
                    IF(EXISTS (SELECT 1 FROM siparisler s WHERE s.masaId = m.masaId), 1, 0) AS doluMu
                    FROM masalar m";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader dr = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(dr);

                rptMasalar.DataSource = dt;
                rptMasalar.DataBind();
            }
        }

        protected void rptMasalar_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "sec")
            {
                int.TryParse(e.CommandArgument.ToString(), out secilenMasaId);
                pnlSiparisler.Visible = true;

                // Butonun Text değerini almak için:
                var btn = e.Item.FindControl("btnMasa") as System.Web.UI.WebControls.Button;
                lblSeciliMasa.Text = btn != null ? btn.Text : secilenMasaId.ToString();

                SiparisleriGetir(secilenMasaId);
            }
        }

        private void SiparisleriGetir(int masaId)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT u.urunAdi AS urunAd, s.adet, u.fiyat, (s.adet * u.fiyat) AS toplam
                    FROM siparisler s
                    INNER JOIN urunler u ON s.urunId = u.urunId
                    WHERE s.masaId = @masaId";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@masaId", masaId);

                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSiparisler.DataSource = dt;
                gvSiparisler.DataBind();

                decimal toplamTutar = 0;
                foreach (DataRow row in dt.Rows)
                {
                    toplamTutar += Convert.ToDecimal(row["toplam"]);
                }

                lblToplamTutar.Text = toplamTutar.ToString("0.00");
                ViewState["masaId"] = masaId;
            }
        }

        protected void btnOdemeYap_Click(object sender, EventArgs e)
        {
            if (ViewState["masaId"] != null)
            {
                int masaId = Convert.ToInt32(ViewState["masaId"]);
                Response.Redirect("Odeme.aspx?masaId=" + masaId);
            }
        }
    }
}
