using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;

namespace STAJCAFE
{
    public partial class Raporlar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Yıl dropdown
                ddlYil.DataSource = Enumerable.Range(DateTime.Now.Year - 25, 26).Reverse();
                ddlYil.DataBind();
                ddlYil.SelectedValue = DateTime.Now.Year.ToString();

                // Ay dropdown
                ddlAy.DataSource = Enumerable.Range(1, 12).Select(i => new
                {
                    AyAdi = System.Globalization.CultureInfo.GetCultureInfo("tr-TR").DateTimeFormat.GetMonthName(i),
                    AyNo = i
                });
                ddlAy.DataTextField = "AyAdi";
                ddlAy.DataValueField = "AyNo";
                ddlAy.DataBind();
                ddlAy.SelectedValue = DateTime.Now.Month.ToString();

                LoadChartData();
            }
        }

        protected void ddl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlYil.SelectedIndex >= 0 && ddlAy.SelectedIndex >= 0)
            {
                LoadChartData();
            }
        }

        private void LoadChartData()
        {
            int yil = int.Parse(ddlYil.SelectedValue);
            int ay = int.Parse(ddlAy.SelectedValue);

            List<string> gunler = new List<string>();
            List<decimal> cirolar = new List<decimal>();

            string cs = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;
            using (MySqlConnection conn = new MySqlConnection(cs))
            {
                string query = @"
                    SELECT 
                        DAY(sg.tarih) AS Gun,
                        SUM(sg.adet * u.fiyat) AS GunlukCiro
                    FROM siparisGecmisi sg
                    JOIN urunler u ON sg.urunId = u.urunId
                    WHERE YEAR(sg.tarih) = @yil AND MONTH(sg.tarih) = @ay
                    GROUP BY DAY(sg.tarih)
                    ORDER BY Gun";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@yil", yil);
                cmd.Parameters.AddWithValue("@ay", ay);

                conn.Open();
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    gunler.Add(reader["Gun"].ToString());
                    cirolar.Add(Convert.ToDecimal(reader["GunlukCiro"]));
                }
            }

            var json = new JavaScriptSerializer().Serialize(new
            {
                labels = gunler,
                data = cirolar
            });

            ltChartData.Text = json;
        }
    }
}
