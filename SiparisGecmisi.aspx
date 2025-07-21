<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SiparisGecmisi.aspx.cs" Inherits="STAJCAFE.SiparisGecmisi" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <asp:GridView ID="gvSiparisGecmisi" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"   Style="width: 90%; max-width: 1200px; margin: 0 auto;">
        <Columns>
            <asp:BoundField DataField="gecmisId" HeaderText="ID" />
            <asp:BoundField DataField="masaId" HeaderText="Masa No" />
            <asp:BoundField DataField="urunAdi" HeaderText="Ürün Adı" />
            <asp:BoundField DataField="adet" HeaderText="Adet" />
            <asp:BoundField DataField="tarih" HeaderText="Tarih" DataFormatString="{0:dd.MM.yyyy HH:mm}" />
        </Columns>
    </asp:GridView>


</asp:Content>
