<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ErrorPage.aspx.cs" Inherits="ACG.EA.AppCentre.Web.ErrorPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div class="errorPageTitle">    
</div>

<div class="errorPageMsg"><br />
    <asp:Literal ID="Msg" Text="Application Error" runat="server"></asp:Literal>
</div>
    </form>
</body>
</html>
