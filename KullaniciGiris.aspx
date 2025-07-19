<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KullaniciGiris.aspx.cs" Inherits="STAJCAFE.KullaniciGiris" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>CAFE UYGULAMASI - Giriş</title>

    <style>
        body {
            margin: 0;
            padding: 0;
            background-color: #FAF7FB;
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
        }

        .center-container {
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .form-box {
            background-color: #FFFFFF;
            padding: 50px 40px;
            border-radius: 20px;
            box-shadow: 0 6px 25px rgba(165, 143, 201, 0.25);
            text-align: center;
            width: 400px;
            transition: all 0.3s ease;
        }

        .form-box h2 {
            color: #5D3A85;
            margin-bottom: 30px;
            font-size: 24px;
        }

        .form-box label {
            display: block;
            text-align: left;
            margin-top: 15px;
            font-weight: 600;
            color: #4B3B57;
            font-size: 14px;
        }

        .form-box input {
            width: 100%;
            padding: 12px;
            margin-top: 6px;
            border: 2px solid #D8BFD8;
            border-radius: 10px;
            font-size: 15px;
            background-color: #fff;
            color: #3A2F4D;
            box-shadow: inset 0 1px 3px rgba(0,0,0,0.05);
            transition: border-color 0.3s ease, box-shadow 0.3s ease;
        }

        .form-box input:focus {
            border-color: #A58FC9;
            outline: none;
            box-shadow: 0 0 10px rgba(165, 143, 201, 0.4);
        }

        .lavender-button {
            background-color: #D8BFD8;
            color: #4B3B57;
            font-weight: 600;
            border: none;
            border-radius: 20px;
            padding: 12px 30px;
            font-size: 16px;
            margin-top: 30px;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 4px 12px rgba(216, 191, 216, 0.4);
        }

        .lavender-button:hover {
            background-color: #BFA5D0;
            color: #3A2F4D;
            transform: translateY(-2px);
            box-shadow: 0 6px 18px rgba(216,191,216,0.6);
        }

        .mesaj-label {
            color: #A8416F;
            font-size: 15px;
            margin-top: 20px;
            font-weight: bold;
            display: block;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">
        <div class="center-container">
            <div class="form-box">
                <h2>Kullanıcı Girişi</h2>

                <asp:Label ID="labelKullaniciAdi" runat="server" Text="Kullanıcı Adı"></asp:Label>
                <asp:TextBox ID="textKullaniciAdi" runat="server" CssClass="form-control" />

                <asp:Label ID="labelSifre" runat="server" Text="Şifre"></asp:Label>
                <asp:TextBox ID="textSifre" runat="server" TextMode="Password" CssClass="form-control" />

                <asp:Label ID="mesajLabel" runat="server" CssClass="mesaj-label" />

                <asp:Button ID="buttonGiris" runat="server" Text="Giriş Yap" CssClass="lavender-button" OnClick="ButtonGiris_Click" />
            </div>
        </div>
    </form>
</body>
</html>
