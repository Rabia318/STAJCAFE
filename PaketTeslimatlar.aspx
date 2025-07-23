<%@ Page Title="Paket Teslimatlar" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="PaketTeslimatlar.aspx.cs" Inherits="STAJCAFE.PaketTeslimatlar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .siparis-grid {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
        }
        .siparis-kart {
            border: 1px solid #ccc;
            border-radius: 10px;
            padding: 15px;
            width: 300px;
            box-shadow: 0 4px 6px rgba(0,0,0,0.1);
            background-color: #f9f9f9;
        }
        .siparis-kart h4 {
            margin-top: 0;
        }
        .modalPopup {
            box-shadow: 0 0 20px rgba(0, 0, 0, 0.4);
        }
        .modalBackground {
            background-color: rgba(0, 0, 0, 0.5);
        }
        .uyari-mesaji {
            color: red;
            font-weight: bold;
        }
    </style>

    <h2>Paket Teslimatlar</h2>

    <div class="siparis-grid">
        <asp:Repeater ID="rptTeslimatlar" runat="server" OnItemDataBound="rptTeslimatlar_ItemDataBound">
            <ItemTemplate>
                <div class="siparis-kart">
                    <h4>Sipariş ID: <%# Eval("siparisId") %></h4>
                    <p><strong>Müşteri:</strong> <%# Eval("musteriAdi") %> <%# Eval("musteriSoyadi") %></p>
                    <p><strong>Telefon:</strong> <%# Eval("musteriNumarasi") %></p>
                    <p><strong>Adres:</strong> <%# Eval("adres") %></p>
                    <p><strong>Ürünler:</strong> <%# Eval("urunler") %></p>
                    <p><strong>Toplam Tutar:</strong> <%# Eval("toplamTutar") %> ₺</p>

                    <p>
                        <asp:DropDownList ID="ddlKuryeler" runat="server" />
                    </p>
                    
                    <p>
                        <asp:RadioButtonList ID="rblDurum" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="DurumGuncelle">
                            <asp:ListItem Text="Hazırlanıyor" Value="Hazırlanıyor" />
                            <asp:ListItem Text="Yolda" Value="Yolda" />
                            <asp:ListItem Text="Teslim Edildi" Value="Teslim Edildi" />
                        </asp:RadioButtonList>
                    </p>

                    <asp:HiddenField ID="hfSiparisId" runat="server" Value='<%# Eval("siparisId") %>' />

                    <asp:Image ID="imgDurum" runat="server" ImageUrl='<%# GetAnimation(Eval("durum").ToString()) %>' Width="150" />

                    <br />

                    <asp:Button ID="btnGuncelle" runat="server" Text="Kurye Güncelle" CommandArgument='<%# Eval("siparisId") %>' OnClick="btnGuncelle_Click" CssClass="btn btn-success" />

                    <asp:Button ID="btnDetay" runat="server" Text="Detayları Göster" CommandArgument='<%# Eval("siparisId") %>' OnClick="btnDetay_Click" CssClass="btn btn-info" />

                    <asp:Label ID="litUyari" runat="server" CssClass="uyari-mesaji" />
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <asp:Panel ID="pnlPopup" runat="server" CssClass="modalPopup" style="display:none; background: white; padding: 20px; border-radius: 10px; width: 500px;">
        <h4>Sipariş Detayları</h4>
        <asp:Literal ID="litDetaylar" runat="server" />
        <br /><br />
        <asp:Button ID="btnKapat" runat="server" Text="Kapat" OnClick="btnKapat_Click" CssClass="btn btn-danger" />
    </asp:Panel>

    <asp:HiddenField ID="hdnDummy" runat="server" />
    <ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" 
        TargetControlID="hdnDummy" 
        PopupControlID="pnlPopup" 
        BackgroundCssClass="modalBackground" 
        CancelControlID="btnKapat" />
</asp:Content>
