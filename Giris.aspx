  <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Giris.aspx.cs" Inherits="STAJCAFE.Giris" %>

<!DOCTYPE html>


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
        <script src="bootstrap.bundle.min.js"></script>
    <link href="bootstrap.min.css" rel="stylesheet" />

</head>
<body>
    <form id="form1" runat="server">

         <h3 class="text-center">CRUD Operations</h3>


           <div class="container">
                <div class="row">
                    <div class="col-md-3" style="padding: 2px">
                        <asp:TextBox ID="txtID" CssClass="form-control" runat="server" Enabled="false" />
                    </div>


                    <div class="col-md-9" style="padding: 2px">
                        <asp:TextBox ID="txtAciklama" CssClass="form-control" placeholder="Açıklama Giriniz" runat="server" MaxLength="100"></asp:TextBox>
                    </div>
                </div>



               <div class="row">
                    <div class="col-md" style="padding: 2px">
                        <asp:DropDownList ID="drpMusteri"  CssClass="form-control" runat="server" DataTextField="musteriAdi" DataValueField="id" ></asp:DropDownList>
                    </div>
                </div>

               <div class="row">
                    <div class="col-md" style="padding: 2px">
                        <asp:DropDownList ID="drpUrunler"  CssClass="form-control" runat="server" DataTextField="urunAdi" DataValueField="id" ></asp:DropDownList>
                    </div>
                </div>




                <div class="row">
                    <div class="col-md-6" style="padding: 2px">
                        <asp:TextBox ID="txtMiktar" CssClass="form-control" placeholder="Miktar Giriniz" runat="server" MaxLength="100"></asp:TextBox>
                    </div>

                    <div class="col-md-6" style="padding: 2px">
                        <asp:TextBox ID="txtFiyat" CssClass="form-control" placeholder="Fiyat Giriniz" runat="server" MaxLength="50"></asp:TextBox>
                    </div>
                </div>



                <div class="row">
                    <div class="col-md" style="padding: 2px">
                        <asp:Label ID="mesajLabel" runat="server" Text="" ForeColor="Red"></asp:Label>

                    </div>
                </div>


            </div>


            <div class="container">
                <div class="row mt-3">
                    <div class="col-md-4" style="padding: 2px">
                        <asp:Button ID="btnKaydet" Width="100%" CssClass="btn btn-primary" runat="server" Text="Kaydet" OnClick="btnKaydet_Click" />
                    </div>
                    <div class="col-md-4" style="padding: 2px">
                        <asp:Button ID="btnSil" Width="100%" CssClass="btn btn-danger" runat="server" Text="Sil" OnClick="btnSil_Click" />
                    </div>
                    <div class="col-md-4" style="padding: 2px">
                        <asp:Button ID="btnRead" Width="100%" CssClass="btn btn-success" runat="server" Text="Read" OnClick="btnRead_Click" />
                    </div>
                    <%--                    <div class="col-md-3" style="padding: 2px">
                        <asp:Button ID="btnGuncelle" Width="100%" CssClass="btn btn-warning" runat="server" Text="Güncelle" OnClick="btnGuncelle_Click" />
                    </div>--%>
                </div>
            </div>


            <div class="mt-4">
                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="100%" AutoGenerateSelectButton="True" DataKeyNames="id" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" CssClass="table table-bordered">
                    <Columns>
                        <asp:BoundField DataField="id" HeaderText="ID" SortExpression="id" />
                        <asp:BoundField DataField="musteriId" HeaderText="Müşteri Id" SortExpression="musteriId" />
                        <asp:BoundField DataField="musteriAdi" HeaderText="Müşteri Adı" SortExpression="musteriAdi" />
                        <asp:BoundField DataField="urunId" HeaderText="Ürün ID" SortExpression="urunId" />
                        <asp:BoundField DataField="urunAdi" HeaderText="Ürün Adı" SortExpression="urunAdi" />
                        <asp:BoundField DataField="miktar" HeaderText="Miktar" SortExpression="miktar" />
                        <asp:BoundField DataField="fiyat" HeaderText="Fiyat" SortExpression="fiyat"/>
                        <asp:BoundField DataField="aciklama" HeaderText="Açiklama" SortExpression="aciklama"/>
                    </Columns>
                </asp:GridView>
            </div>





    </form>
</body>
</html>
