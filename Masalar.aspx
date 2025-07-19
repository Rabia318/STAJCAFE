<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Masalar.aspx.cs" Inherits="STAJCAFE.Masalar" MasterPageFile="~/Site1.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .masa {
            display: inline-block;
            width: 120px;
            height: 120px;
            margin: 10px;
            font-size: 18px;
            font-weight: bold;
            text-align: center;
            line-height: 120px;
            border-radius: 10px;
            cursor: pointer;
            color: white;
            border: none;
        }

        .dolu {
            background-color: #4B0082; /* koyu mor */
        }

        .bos {
            background-color: #DDA0DD; /* açık mor */
        }

        .secili {
            border: 3px solid #F0E68C; /* açık sarı kenarlık */
        }

        .urunler {
            margin-top: 30px;
        }

        .odemePanel {
            margin-top: 30px;
            padding: 10px;
            border-top: 2px solid #4B0082; /* koyu mor */
        }
    </style>

    <asp:Label ID="lblMesaj" runat="server" ForeColor="Red"></asp:Label>
    <br />
    <asp:Repeater ID="rptMasalar" runat="server" OnItemCommand="rptMasalar_ItemCommand">
        <ItemTemplate>
            <asp:Button 
                ID="btnMasa" 
                runat="server" 
                Text='<%# Eval("konum") %>' 
                CommandArgument='<%# Eval("masaId") %>' 
                CommandName="sec" 
                CssClass='<%# Convert.ToBoolean(Eval("doluMu")) ? "masa dolu" : "masa bos" %>' />
        </ItemTemplate>
    </asp:Repeater>

    <asp:Panel ID="pnlSiparisler" runat="server" Visible="false" CssClass="urunler">
        <h3>Seçilen Masa: <asp:Label ID="lblSeciliMasa" runat="server"></asp:Label></h3>
        <asp:GridView ID="gvSiparisler" runat="server" AutoGenerateColumns="false">
            <Columns>
                <asp:BoundField HeaderText="Ürün" DataField="urunAd" />
                <asp:BoundField HeaderText="Adet" DataField="adet" />
                <asp:BoundField HeaderText="Fiyat (₺)" DataField="fiyat" />
                <asp:BoundField HeaderText="Toplam" DataField="toplam" />
            </Columns>
        </asp:GridView>

        <div class="odemePanel">
            <h4>Toplam Tutar: <asp:Label ID="lblToplamTutar" runat="server" Font-Bold="true"></asp:Label> ₺</h4>
            <asp:Button ID="btnOdemeYap" runat="server" Text="Ödeme Yap" OnClick="btnOdemeYap_Click" />
        </div>
    </asp:Panel>

</asp:Content>
