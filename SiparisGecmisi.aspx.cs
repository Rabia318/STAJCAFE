using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace STAJCAFE
{
    public partial class SiparisGecmisi : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSiparisGecmisi();
            }
        }

        private void BindSiparisGecmisi()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string sql = @"
                    SELECT sg.gecmisId, sg.masaId, u.urunAdi, sg.adet, sg.tarih
                    FROM siparisGecmisi sg
                    JOIN urunler u ON sg.urunId = u.urunId
                    ORDER BY sg.tarih DESC";

                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                gvSiparisGecmisi.DataSource = dt;
                gvSiparisGecmisi.DataBind();
            }
        }
    }
}
