<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Raporlar.aspx.cs" Inherits="STAJCAFE.Raporlar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        select {
            position: relative;
            z-index: 1000;
        }

        .dropdown-container {
            position: relative;
            overflow: visible;
            display: inline-block;
            margin-right: 10px;
        }
    </style>

    <div class="dropdown-container">
        <asp:DropDownList ID="ddlYil" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_SelectedIndexChanged" />
    </div>
    <div class="dropdown-container">
        <asp:DropDownList ID="ddlAy" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_SelectedIndexChanged" />
    </div>

    <div  style="display:none;">
    <asp:Literal ID="ltChartData" runat="server" />
    </div>

    <canvas id="ciroChart" width="500" height="200"></canvas>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

    <script>
        var chartData = <%= string.IsNullOrEmpty(ltChartData.Text) ? "null" : ltChartData.Text %>;

        if (chartData) {
            const ctx = document.getElementById('ciroChart').getContext('2d');
            new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: chartData.labels,
                    datasets: [{
                        label: 'Günlük Ciro (₺)',
                        data: chartData.data,
                        backgroundColor: 'rgba(54, 162, 235, 0.6)'
                    }]
                }
            });
        }
    </script>

</asp:Content>
