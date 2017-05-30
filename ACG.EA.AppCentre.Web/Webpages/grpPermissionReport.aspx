<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="grpPermissionReport.aspx.cs" Inherits="ACG.EA.AppCentre.Web.grpPermissionReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
    <script src="../Support/Scripts/js/jqGridExportToExcel.js"></script>

        <%if ((bool)Session["IsAppAdmin"])
          { %>
    <div> 
        <h4><span id="manageAppBox" class="management">Group Permissions Report</span></h4>
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
        <button data-bind="click: showAppGroupPermissions" id="showAppGroupPermissions">Get Group Permissions</button>
        <div id="bxUserPermissions">
            <table id="tblAppGrpPermissions"></table>
            <div id="pager"></div>
        </div>  
        <% } %>
    </div>
</asp:Content>

