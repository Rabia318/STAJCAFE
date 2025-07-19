<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RezervasyonIslemleri.aspx.cs" Inherits="STAJCAFE.RezervasyonIslemleri" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2 style="margin-bottom: 20px; text-align:center;">Rezervasyon Yönetimi</h2>

    <asp:Panel ID="pnlForm" runat="server" CssClass="pnlForm container-center">

        <div class="form-row">
            <div class="form-group">
                <asp:Label ID="lblMusteri" runat="server" Text="Müşteri:" AssociatedControlID="ddlMusteri" CssClass="form-label" />
                <asp:DropDownList ID="ddlMusteri" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlMusteri_SelectedIndexChanged" />
            </div>

            <div class="form-group">
                <asp:Label ID="lblKisiSayisi" runat="server" Text="Kişi Sayısı:" AssociatedControlID="txtKisiSayisi" CssClass="form-label" />
                <asp:TextBox ID="txtKisiSayisi" runat="server" CssClass="form-control" Text="1" />
            </div>

            <div class="form-group">
                <asp:Label ID="lblTarih" runat="server" Text="Tarih:" AssociatedControlID="txtTarih" CssClass="form-label" />
                <asp:TextBox ID="txtTarih" runat="server" CssClass="form-control" Text='<%# DateTime.Now.ToString("yyyy-MM-dd") %>' />
                <ajaxToolkit:CalendarExtender ID="ceTarih" runat="server" TargetControlID="txtTarih" Format="yyyy-MM-dd" />
            </div>

            <div class="form-group">
                <asp:Label ID="lblSaatBas" runat="server" Text="Başlangıç Saati:" AssociatedControlID="txtSaatBas" CssClass="form-label" />
                <asp:TextBox ID="txtSaatBas" runat="server" CssClass="form-control" Text="12:00" />
            </div>

            <div class="form-group">
                <asp:Label ID="lblSaatBit" runat="server" Text="Bitiş Saati:" AssociatedControlID="txtSaatBit" CssClass="form-label" />
                <asp:TextBox ID="txtSaatBit" runat="server" CssClass="form-control" Text="13:00" />
            </div>
        </div>

        <div style="margin-top: 15px; margin-bottom: 20px;">
            <asp:Button ID="btnMusaitMasalariGoster" runat="server" Text="Müsait Masaları Göster" CssClass="btn btn-info" OnClick="btnMusaitMasalariGoster_Click" />
        </div>

        <asp:Label ID="lblMasalar" runat="server" Text="Masalar:" Font-Bold="true" />
        <asp:PlaceHolder ID="phMasalar" runat="server" />
        <br />

        <asp:Panel ID="pnlButtons" runat="server" CssClass="button-row" Style="margin-top: 20px;">
            <asp:Button ID="btnEkle" runat="server" Text="Rezervasyon Ekle" CssClass="btn btn-primary" OnClick="btnEkle_Click" />
            <asp:Button ID="btnGuncelle" runat="server" Text="Rezervasyon Güncelle" CssClass="btn btn-warning" OnClick="btnGuncelle_Click" Enabled="false" />
            <asp:Button ID="btnSil" runat="server" Text="Rezervasyon Sil" CssClass="btn btn-danger" OnClick="btnSil_Click" Enabled="false" />
            <asp:Button ID="btnTemizle" runat="server" Text="Temizle" CssClass="btn btn-secondary" OnClick="btnTemizle_Click" />
        </asp:Panel>

    </asp:Panel>

    <br />

    <asp:GridView ID="gvRezervasyonlar" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-bordered grid-center"
        AllowPaging="true" PageSize="10" OnPageIndexChanging="gvRezervasyonlar_PageIndexChanging" OnSelectedIndexChanged="gvRezervasyonlar_SelectedIndexChanged"
        DataKeyNames="rezervasyonId" >
        <Columns>
            <asp:BoundField DataField="rezervasyonId" HeaderText="ID" ReadOnly="true" Visible="false" />
            <asp:BoundField DataField="musteriAdiSoyadi" HeaderText="Müşteri" />
            <asp:BoundField DataField="masaBilgi" HeaderText="Masa" />
            <asp:BoundField DataField="tarih" HeaderText="Tarih" DataFormatString="{0:yyyy-MM-dd}" />
            <asp:BoundField DataField="saatBas" HeaderText="Başlangıç" DataFormatString="{0:HH:mm}" />
            <asp:BoundField DataField="saatBit" HeaderText="Bitiş" DataFormatString="{0:HH:mm}" />
            <asp:BoundField DataField="kisiSayisi" HeaderText="Kişi Sayısı" />
            <asp:CommandField ShowSelectButton="true" SelectText="Seç" />
        </Columns>
    </asp:GridView>

    <style>
        .container-center {
            max-width: 1000px;
            margin: 0 auto;
            padding: 20px 25px;
            background: #f8f8ff;
            border-radius: 8px;
            box-shadow: 0 0 12px rgba(0,0,0,0.12);
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .pnlForm.container-center {
            padding: 20px 25px;
        }

        .form-row {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
        }

        .form-group {
            flex: 1 1 180px;
            min-width: 180px;
            display: flex;
            flex-direction: column;
        }

        .form-label {
            margin-bottom: 6px;
            font-weight: 600;
            font-size: 14px;
            color: #4b0082;
        }

        .form-control {
            padding: 8px 12px;
            font-size: 14px;
            border: 1.8px solid #6f42c1;
            border-radius: 6px;
            transition: border-color 0.3s ease;
        }

        .form-control:focus {
            border-color: #4b0082;
            outline: none;
            box-shadow: 0 0 8px #c9acd9;
        }

        .button-row > * {
            margin-right: 15px;
            min-width: 140px;
            font-weight: 600;
        }

        /* Masa butonları yatay ve geniş */
        .masa-btn {
            margin: 5px 6px;
            padding: 14px 35px;
            border-radius: 10px;
            border: 2px solid #6f42c1;
            background-color: #d8bfd8;
            color: #4b0082;
            font-weight: 700;
            cursor: pointer;
            transition: background-color 0.3s ease, color 0.3s ease;
            min-width: 110px;
            font-size: 16px;
            white-space: nowrap;
        }

        .masa-btn:hover:not([disabled]) {
            background-color: #c9acd9;
            color: #3a0066;
        }

        .masa-dolu {
            background-color: #b0a8b9 !important;
            color: #6c757d !important;
            cursor: not-allowed !important;
            border-color: #8a85a1 !important;
        }

        .masa-secili {
            background-color: #6f42c1 !important;
            color: white !important;
            border-color: #4b0082 !important;
        }

        /* GridView ortalanmış ve genişliği container'a uyumlu */
        .grid-center {
            max-width: 1000px;
            margin: 0 auto 40px auto;
            width: 100%;
        }
    </style>

</asp:Content>

