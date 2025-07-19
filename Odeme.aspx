<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Odeme.aspx.cs" Inherits="STAJCAFE.Odeme" MasterPageFile="~/Site1.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        #odemePanel { max-width: 500px; margin: 50px auto; padding: 20px; border: 1px solid #b39ddb; background-color: #ede7f6; border-radius: 10px; color: #311b92; }
        .btnOdemeTamamla { background-color: #7e57c2; color: white; border: none; padding: 12px 30px; border-radius: 8px; font-weight: bold; cursor: pointer; transition: background-color 0.3s ease; }
        .btnOdemeTamamla:hover { background-color: #512da8; }
        .kartBilgi { margin-top: 15px; }
        .kartBilgi input { width: 100%; padding: 8px; margin-bottom: 10px; }
    </style>

    <asp:Panel ID="odemePanel" runat="server">
        <h2>Ödeme Sayfası</h2>
        <asp:Label ID="lblMasaBilgi" runat="server" Text=""></asp:Label><br /><br />

        <asp:Label Text="Ödeme Yöntemi:" AssociatedControlID="ddlOdemeYontemi" runat="server" />
        <asp:DropDownList ID="ddlOdemeYontemi" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlOdemeYontemi_SelectedIndexChanged">
            <asp:ListItem Text="Nakit" Value="Nakit" />
            <asp:ListItem Text="Kart" Value="Kart" />
        </asp:DropDownList>

        <asp:Panel ID="pnlKartBilgileri" runat="server" CssClass="kartBilgi" Visible="false">
            <asp:Label Text="Kart Numarası:" AssociatedControlID="txtKartNo" runat="server" />
            <asp:TextBox ID="txtKartNo" runat="server" MaxLength="16" />
            <asp:Label Text="Son Kullanma Tarihi (AA/YY):" AssociatedControlID="txtSonKullanma" runat="server" />
            <asp:TextBox ID="txtSonKullanma" runat="server" MaxLength="5" />
            <asp:Label Text="CVV:" AssociatedControlID="txtCvv" runat="server" />
            <asp:TextBox ID="txtCvv" runat="server" MaxLength="3" TextMode="Password" />
        </asp:Panel>

        <asp:Button ID="btnOdemeTamamla" runat="server" Text="Ödemeyi Tamamla" CssClass="btnOdemeTamamla" OnClick="btnOdemeTamamla_Click" />
        <br /><br />
        <asp:Label ID="lblMesaj" runat="server" ForeColor="Red"></asp:Label>
    </asp:Panel>
</asp:Content>
