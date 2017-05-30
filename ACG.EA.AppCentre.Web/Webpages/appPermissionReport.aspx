<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="appPermissionReport.aspx.cs" Inherits="ACG.EA.AppCentre.Web.appPermissionReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
    <script src="../Support/Scripts/js/jqGridExportToExcel.js"></script>

        <%if ((bool)Session["IsAppAdmin"])
          { %>
    <div> 
        <h4><span id="manageAppBox" class="management">User Permissions Report</span></h4>
        <table>
            <tr>
                <td>
        <select id="fldApplicationId" data-bind="options: adminApps, optionsText: 'application_name',
    optionsValue: 'application_id', optionsCaption: 'Select App....',
    event: { change: getAppTargetValues }, value: applicationId, valueUpdate: 'change'">
        </select>
                </td>
                <td>
                    <select id="fldAppUserTarget" data-bind="options: appTargetValues, optionsText: 'target_name',
    optionsValue: 'target_id', optionsCaption: 'Select Target....', value: targetId, valueUpdate: 'change'"></select>
                </td>
                </tr>
        </table>
        <br />
        <button data-bind="click: showManageBox">Get Users</button>
        <div id="bxUserPermissions">
            <%--<div id="btnExport2Excel">Export To Excel</div>--%>
            <table id="tblAppUserPermissions"></table>
            <div id="pager"></div>
        </div>
        <script>
            $(function () {
                $("input[type=submit], button")
        .button().click(function (event) {
            event.preventDefault();
        });
            })
        </script>    
        <% } %>
    </div>
</asp:Content>
