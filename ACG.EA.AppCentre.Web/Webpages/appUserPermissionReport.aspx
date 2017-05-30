<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="appUserPermissionReport.aspx.cs" Inherits="ACG.EA.AppCentre.Web.appUserPermissionReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
    <script src="../Support/Scripts/js/jqGridExportToExcel.js"></script>

        <%if ((bool)Session["IsAppAdmin"])
          { %>
    <div> 
        <h4><span id="manageAppBox" class="management">Users Permissions Report</span></h4>
        <table>
            <thead>
                <tr>
                    <td>Application</td>
                    <td>Group</td>
                </tr>
            </thead>
            <tr>
                <td style="padding-right: 15px">
        <select id="fldApplicationId" data-bind="options: adminApps, optionsText: 'application_name',
    optionsValue: 'application_id', optionsCaption: 'Select App....',
    event: { change: loadReportAppGroups }, value: applicationId, valueUpdate: 'change'">
        </select>
                </td>
                <td>
                    <select id="fldAppGroupRpt" data-bind="options: appGroups, optionsText: 'groupDescription',
    optionsValue: 'group_Id'"></select>
                </td>
                </tr>
        </table>
        <br />
        <button data-bind="click: showUserAppGroups">Get Users</button>
        <div id="bxUserPermissions">
            <%--<div id="btnExport2Excel">Export To Excel</div>--%>
            <table id="tblAppUserPermissions"></table>
            <div id="pager"></div>
        </div>
        
        <script type="text/javascript">
            $(function () {
                $("input[type=submit], button")
        .button().click(function (event) {
            event.preventDefault();
        });
            });
        </script>    
        <% } %>
    </div>
</asp:Content>

