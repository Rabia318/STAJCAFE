<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AnaSayfa1.aspx.cs" Inherits="STAJCAFE.AnaSayfa1" %>
<%@ Register Assembly="System.Web.DataVisualization" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .dashboard-box {
            border: 1px solid #DDA0DD; /* master sayfa mor ton */
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 20px;
            background-color: #F5F0FA; /* çok açık lavanta */
            text-align: center;
            box-shadow: 0 2px 5px rgba(221, 160, 221, 0.4);
            transition: box-shadow 0.3s ease;
        }
        .dashboard-box:hover {
            box-shadow: 0 4px 15px rgba(221, 160, 221, 0.7);
        }
        .dashboard-title {
            font-weight: 700;
            font-size: 18px;
            color: #4B0082; /* koyu mor */
            margin-bottom: 8px;
        }
        .dashboard-value {
            font-size: 28px;
            color: #800080; /* mor */
            font-weight: 700;
        }
        .dashboard-container {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
            gap: 25px;
            margin-bottom: 35px;
        }
        h2, h3 {
            color: #4B0082;
            margin-top: 30px;
            margin-bottom: 20px;
        }
    </style>

    <h2>📊 Ana Sayfa - Dashboard</h2>

    <div class="dashboard-container">
        <div class="dashboard-box">
            <div class="dashboard-title">Toplam Sipariş</div>
            <asp:Label ID="lblToplamSiparis" runat="server" CssClass="dashboard-value" />
        </div>
        <div class="dashboard-box">
            <div class="dashboard-title">Toplam Ciro (₺)</div>
            <asp:Label ID="lblToplamCiro" runat="server" CssClass="dashboard-value" />
        </div>
        <div class="dashboard-box">
            <div class="dashboard-title">Aktif Masalar</div>
            <asp:Label ID="lblAktifMasalar" runat="server" CssClass="dashboard-value" />
        </div>
        <div class="dashboard-box">
            <div class="dashboard-title">Dışarıdan Sipariş</div>
            <asp:Label ID="lblDisSiparis" runat="server" CssClass="dashboard-value" />
        </div>
        <div class="dashboard-box">
            <div class="dashboard-title">En Çok Tercih Edilen Masa</div>
            <asp:Label ID="lblFavoriMasa" runat="server" CssClass="dashboard-value" />
        </div>
    </div>

    <h3>🍽 En Çok Satan Ürünler</h3>
    <asp:GridView ID="gvEnCokSatanlar" runat="server" CssClass="table table-striped table-hover" AutoGenerateColumns="true" />

    <h3>📈 Haftalık Ciro</h3>
    <asp:Chart ID="chartCiro" runat="server" Width="600px" Height="300px">
        <Series>
            <asp:Series Name="Ciro" ChartType="Column" />
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1" />
        </ChartAreas>
    </asp:Chart>

    <h3>👥 Haftalık Müşteri Sayısı</h3>
    <asp:Chart ID="chartMusteri" runat="server" Width="600px" Height="300px">
        <Series>
            <asp:Series Name="Müşteri" ChartType="Column" />
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1" />
        </ChartAreas>
    </asp:Chart>

</asp:Content>
