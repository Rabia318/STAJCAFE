using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace STAJCAFE
{
    public partial class Giris : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GridiDoldur();


                string connStr = ConfigurationManager.ConnectionStrings["MySqlBaglanti"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter da = new SqlDataAdapter(@"SELECT *  FROM [SatislarDb].[dbo].[tb_Musteriler]", conn);
                    DataTable dt = new DataTable();
                    conn.Open();
                    da.Fill(dt);
                    drpMusteri.DataSource = dt;
                    drpMusteri.DataBind();
                }


                //string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter da = new SqlDataAdapter(@"SELECT *  FROM  [dbo].[tb_Urunler]", conn);
                    DataTable dt = new DataTable();
                    conn.Open();
                    da.Fill(dt);
                    drpUrunler.DataSource = dt;
                    drpUrunler.DataBind();
                }


                ClearForm();
            }

        }





        void GridiDoldur()
        {
            string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                SqlDataAdapter dap = new SqlDataAdapter(@"SELECT sp.[id]
      ,sp.[musteriId]
	  ,mus.musteriAdi
      ,sp.[urunId]
	  ,urun.urunAdi
      ,sp.[miktar]
      ,sp.[fiyat]
      ,sp.[aciklama]
  FROM [SatislarDb].[dbo].[tb_Siparisler] sp
  JOIN [dbo].[tb_Musteriler] mus On sp.musteriId = mus.id
  JOIN [dbo].[tb_Urunler] urun ON sp.urunId = urun.id

order by sp.id desc
", conn);
                DataTable dt = new DataTable();

                conn.Open();
                dap.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }







        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = GridView1.SelectedIndex;
            string id = GridView1.DataKeys[index].Value.ToString();

            GridViewRow row = GridView1.SelectedRow;

            txtID.Text = id;

            drpMusteri.ClearSelection();
            string musteri = "";
            musteri = Server.HtmlDecode(row.Cells[2].Text).Trim();
            if (musteri != "")
            {
                drpMusteri.Items.FindByValue(musteri).Selected = true;
            }

            drpUrunler.ClearSelection();
            string urun = "";
            urun = Server.HtmlDecode(row.Cells[4].Text).Trim();
            if (urun != "")
            {
                drpUrunler.Items.FindByValue(urun).Selected = true;
            }


            txtMiktar.Text = Server.HtmlDecode(row.Cells[6].Text);
            txtFiyat.Text = Server.HtmlDecode(row.Cells[7].Text);
            txtAciklama.Text = Server.HtmlDecode(row.Cells[8].Text);

        }

        protected void btnKaydet_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMiktar.Text))
            {
                mesajLabel.Text = "Miktar dolu olmalıdır....";
                return;
            }

            if (string.IsNullOrEmpty(txtFiyat.Text))
            {
                mesajLabel.Text = "Fiyat dolu olmalıdır....";
                return;
            }

            if (string.IsNullOrEmpty(txtAciklama.Text))
            {
                mesajLabel.Text = "Açıklama dolu olmalıdır....";
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string qeery = "";

                    con.Open();
                    if (txtID.Text != "")
                    {
                        qeery = "UPDATE [dbo].[tb_Siparisler] SET musteriId = @musteriId, urunId = @urunId, miktar = @miktar, fiyat = @fiyat, aciklama = @aciklama WHERE id = @id";
                    }
                    else
                    {
                        qeery = "INSERT INTO [dbo].[tb_Siparisler]           " +
                            " ([musteriId],[urunId],[miktar] ,[fiyat],[aciklama])      " +
                            "VALUES            (@musteriId,@urunId,@miktar,@fiyat,@aciklama)";
                    }

                    SqlCommand cmd = new SqlCommand(qeery, con);
                    cmd.Parameters.AddWithValue("@musteriId", drpMusteri.SelectedItem.Value);
                    cmd.Parameters.AddWithValue("@urunId", drpUrunler.SelectedItem.Value);
                    cmd.Parameters.AddWithValue("@miktar", txtMiktar.Text);
                    cmd.Parameters.AddWithValue("@fiyat", txtFiyat.Text);
                    cmd.Parameters.AddWithValue("@id", txtID.Text);
                    cmd.Parameters.AddWithValue("@aciklama", txtAciklama.Text);

                    int count = cmd.ExecuteNonQuery();
                    if (count > 0)
                    {

                        if (txtID.Text != "")
                        {
                            mesajLabel.Text = "Updated successfully!";
                            //Response.Write("<script type='text/javascript'>alert('Updated successfully!');</script>");
                        }
                        else
                        {
                            mesajLabel.Text = "Created successfully!";
                            //Response.Write("<script type='text/javascript'>alert('Created successfully!');</script>");
                        }


                        ClearForm();
                    }
                    else
                    {
                        mesajLabel.Text = "Sql Error!";
                        //Response.Write("<script type='text/javascript'>alert('Error while creating user.');</script>");
                    }
                }

                GridiDoldur();

            }
            catch (Exception ex)
            {
                mesajLabel.Text = ex.Message;
                //Response.Write("<script type='text/javascript'>alert('An error occurred: " + ex.Message + "');</script>");
            }




        }

        protected void btnSil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtID.Text))
            {
                mesajLabel.Text = "Lütfen silinecek kullanıcıyı seçin.";
                //Response.Write("<script type='text/javascript'>alert('Lütfen silinecek kullanıcıyı seçin.');</script>");
                return;
            }

            try
            {
                string connStr = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string deleteQuery = "DELETE FROM dbo.[tb_Siparisler] WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(deleteQuery, con);
                    cmd.Parameters.AddWithValue("@id", txtID.Text);

                    int count = cmd.ExecuteNonQuery();
                    if (count > 0)
                    {
                        mesajLabel.Text = "Record deleted sucseessfully";
                        //Response.Write("<script type='text/javascript'>alert('User deleted successfully!');</script>");
                        ClearForm();
                    }
                    else
                    {
                        mesajLabel.Text = "Error with delete";
                        //Response.Write("<script type='text/javascript'>alert('Error while deleting user.');</script>");
                    }
                }

                GridiDoldur();
            }
            catch (Exception ex)
            {
                mesajLabel.Text = ex.Message;
                //Response.Write("<script type='text/javascript'>alert('An error occurred: " + ex.Message + "');</script>");
            }

        }

        protected void btnRead_Click(object sender, EventArgs e)
        {

        }


        private void ClearForm()
        {
            txtID.Text = "";
            txtMiktar.Text = "";
            txtAciklama.Text = "";
            txtFiyat.Text = "";
        }

    }
}


