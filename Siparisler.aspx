<%@ Page Title="Siparişler" Language="C#" MasterPageFile="~/Site1.master" AutoEventWireup="true" CodeBehind="Siparisler.aspx.cs" Inherits="STAJCAFE.Siparisler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        /* Mevcut stil kodları (değiştirilmedi) */
        .form-container {
            background-color: #F8F8FF;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 0 10px rgba(184, 143, 202, 0.3);
            max-width: 900px;
            margin: auto;
        }

        label {
            font-weight: 600;
            color: #4B0082;
            display: block;
            margin-bottom: 6px;
        }

        select, input[type="text"], input[type="number"] {
            width: 100%;
            padding: 8px;
            border: 1.5px solid #D8BFD8;
            border-radius: 5px;
            margin-bottom: 15px;
            font-size: 14px;
            color: #4B0082;
            background-color: #F8F8FF;
            transition: border-color 0.3s ease;
        }

        select:focus, input[type="text"]:focus, input[type="number"]:focus {
            border-color: #9b59b6;
            outline: none;
        }

        .inline-row {
            display: flex;
            gap: 20px;
            margin-bottom: 15px;
        }

        .inline-row > div {
            flex: 1;
        }

        .btn-custom {
            background-color: #D8BFD8;
            border: none;
            padding: 10px 18px;
            color: #4B0082;
            font-weight: 700;
            border-radius: 5px;
            cursor: pointer;
            margin-right: 10px;
            transition: background-color 0.3s ease;
        }

        .btn-custom:hover {
            background-color: #c9acd9;
        }

        .gridview-style {
            width: 100%;
            border-collapse: collapse;
            margin-top: 30px;
            font-size: 14px;
            color: #4B0082;
        }

        .gridview-style th, .gridview-style td {
            border: 1px solid #D8BFD8;
            padding: 10px;
            text-align: center;
        }

        .gridview-style th {
            background-color: #E6E6FA;
            font-weight: 700;
        }

        .gridview-style tr:nth-child(even) {
            background-color: #F0E6F6;
        }

        .gridview-style tr:hover {
            background-color: #c9acd9;
            color: #2c065d;
        }
    </style>

    <script type="text/javascript">
        function toggleMusteriMasa() {
            var ddlMasa = document.getElementById('<%= ddlMasa.ClientID %>');
            var ddlMusteri = document.getElementById('<%= ddlMusteri.ClientID %>');

            if (ddlMasa.value !== "") {
                ddlMusteri.disabled = true;
            } else {
                ddlMusteri.disabled = false;
            }

            if (ddlMusteri.value !== "") {
                ddlMasa.disabled = true;
            } else {
                ddlMasa.disabled = false;
            }
        }
    </script>

    <div class="form-container">
        <h2 style="color:#4B0082; margin-bottom: 20px;">Sipariş Yönetimi</h2>

        <div class="inline-row">
            <div>
                <label for="ddlMasa">Masa Seçimi</label>
                <asp:DropDownList ID="ddlMasa" runat="server" CssClass="ddl-style"
                    onchange="toggleMusteriMasa()" AutoPostBack="false" />
            </div>
            <div>
                <label for="ddlMusteri">Müşteri Seçimi</label>
                <asp:DropDownList ID="ddlMusteri" runat="server" CssClass="ddl-style"
                    onchange="toggleMusteriMasa()" AutoPostBack="false" />
            </div>
        </div>

        <div class="inline-row">
            <div>
                <label for="ddlKategori">Kategori</label>
                <asp:DropDownList ID="ddlKategori" runat="server" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlKategori_SelectedIndexChanged" CssClass="ddl-style" />
            </div>
            <div>
                <label for="ddlUrun">Ürün</label>
                <asp:DropDownList ID="ddlUrun" runat="server" CssClass="ddl-style" />
            </div>
            <div style="flex: 0 0 80px;">
                <label for="txtAdet">Adet</label>
                <asp:TextBox ID="txtAdet" runat="server" Text="1" CssClass="txt-style" />
            </div>
        </div>

        <div>
            <asp:Button ID="btnEkle" runat="server" Text="Sipariş Ekle" CssClass="btn-custom" OnClick="btnEkle_Click" />
            <asp:Button ID="btnGuncelle" runat="server" Text="Güncelle" CssClass="btn-custom" OnClick="btnGuncelle_Click" Enabled="false" />
            <asp:Button ID="btnSil" runat="server" Text="Sil" CssClass="btn-custom" OnClick="btnSil_Click" Enabled="false" />
        </div>

        <asp:GridView ID="gvSiparisler" runat="server" AutoGenerateColumns="false" CssClass="gridview-style"
            OnSelectedIndexChanged="gvSiparisler_SelectedIndexChanged" DataKeyNames="siparisId"
            AllowPaging="true" PageSize="8" OnPageIndexChanging="gvSiparisler_PageIndexChanging">

            <Columns>
                <asp:BoundField DataField="siparisId" HeaderText="Sipariş ID" ReadOnly="true" />
                <asp:BoundField DataField="masaId" HeaderText="Masa ID" />
                <asp:BoundField DataField="musteriId" HeaderText="Müşteri ID" />
                <asp:BoundField DataField="urunAdi" HeaderText="Ürün Adı" />
                <asp:BoundField DataField="adet" HeaderText="Adet" />
                <asp:BoundField DataField="tutar" HeaderText="Tutar (TL)" />
                <asp:BoundField DataField="siparisTarihi" HeaderText="Sipariş Tarihi" DataFormatString="{0:dd.MM.yyyy HH:mm}" />
                <asp:CommandField ShowSelectButton="True" SelectText="Seç" />
            </Columns>

        </asp:GridView>
    </div>

</asp:Content>
