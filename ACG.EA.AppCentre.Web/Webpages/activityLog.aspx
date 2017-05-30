<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="activityLog.aspx.cs" Inherits="ACG.EA.AppCentre.Web.activityLog" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(function () {
            Main.loadActivityLog();
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
     <%if ((bool)Session["IsAppAdmin"])
                          { %>
        <div>
            <select id="fldActLogsAppId" data-bind="options: adminApps, optionsText: 'application_name',
    optionsValue: 'application_id', optionsCaption: 'Select App....', value: applicationId, valueUpdate: 'change'"
                onchange="Main.loadActivityLog">
            </select>
            <hr />
            <table id="tblActivityLog"></table>
        </div>
    <% } %>
</asp:Content>
