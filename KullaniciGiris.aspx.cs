using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data.SqlClient;



namespace STAJCAFE
{

    public partial class KullaniciGiris : System.Web.UI.Page
    {
        protected void ButtonGiris_Click(object sender, EventArgs e)
        {
            String kullaniciAdi = textKullaniciAdi.Text.Trim();
            String kullaniciSifre = textSifre.Text.Trim();


            if (string.IsNullOrEmpty(textKullaniciAdi.Text))
            {
                mesajLabel.Text = "Lutfen kullanici adi alanini doldurunuz ";
                return;
            }


            if (string.IsNullOrEmpty(textSifre.Text))
            {
                mesajLabel.Text = "Sifre alanini doldurmaniz gerekiyor";
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {

                    string query = "";

                    con.Open();

                    query = "SELECT COUNT(*) FROM tb_Kullanicilar WHERE KullaniciAdi = @musteriAdi AND KullaniciSoyadi = @musteriSoyadi AND Sifre = @sifre";

                    SqlCommand cmd = new SqlCommand(query, con);
                    //// cmd.Parameters.AddWithValue("@musteriId",t);
                    //cmd.Parameters.AddWithValue("@musteriAdi", textAd.Text);
                    //cmd.Parameters.AddWithValue("@musteriSoyadi", textSoyad.Text);
                    //cmd.Parameters.AddWithValue("@sifre", textSifre.Text);

                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {


                        mesajLabel.Text = "Basariyla giris yaptiniz";
                        //Response.Write("<script type='text/javascript'>alert('Updated successfully!');</script>");

                        ClearForm();
                    }
                    else
                    {
                        mesajLabel.Text = "Lütfen kaydolun";
                        //Response.Write("<script type='text/javascript'>alert('Created successfully!');</script>");





                    }
                }
            }
            catch (Exception ex)
            {
                mesajLabel.Text = ex.Message;

            }
        }

        protected void ButtonKaydol_Click(object sender, EventArgs e)
        {
            // Örnek: Kayıt sayfasına yönlendirme
            Response.Redirect("KayitOl.aspx");
        }



        protected void textSoyad_TextChanged(object sender, EventArgs e)
        {
            // Burada bir işlem yapacaksan yaz. Boş bile olsa tanımlı olmalı.
        }



        protected void ClearForm()
        {
            textKullaniciAdi.Text = "";
            textSifre.Text = "";
        }


    }
}