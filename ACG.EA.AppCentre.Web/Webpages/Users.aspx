<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="ACG.EA.AppCentre.Web.Users" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(function () {
            Main.getuserProfile();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
    <h1 class="page-header">Users</h1>
    <table id="defaultGrid"></table>
    <hr />
    <br />
    <table id="appAdmins"></table>

</asp:Content>
