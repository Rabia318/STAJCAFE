using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace STAJCAFE
{
    public partial class Raporlar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            if(!IsPostBack)
            {

                int yilimiz = DateTime.Now.Year;
                int baslangicyil = yilimiz - 25;
                for(int yil=yilimiz;yilimiz>=baslangicyil;yil--)
                {


                    DropDownList1.Items.Add(new ListItem(yil.ToString(), yil.ToString()));


                }




            }




        }
    }
}