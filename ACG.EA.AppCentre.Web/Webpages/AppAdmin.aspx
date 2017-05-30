<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="AppAdmin.aspx.cs" Inherits="ACG.EA.AppCentre.Web.AppAdmin" %>

<%@ Register Src="~/Webpages/groupManagement.ascx" TagPrefix="uc1" TagName="groupManagement" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        div#tabs li a { cursor:default; }
    </style>
    <script>
        $(function () {
            $("#tabs").tabs();
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
    <div>
        <%if ((bool)Session["IsAppAdmin"])
          {%>
        <div style="margin-bottom: 10px"><p style="margin-bottom: 5px">Application</p>
            <select id="fldApplicationId" data-bind="
    options: adminApps,
    optionsText: 'application_name',
    optionsValue: 'application_id',
    optionsCaption: 'Select App....', event: { change: getAppGroupsAndPermissions },
    value: applicationId, valueUpdate: 'change'">
            </select> &nbsp;
            <a href="applicationManagement.aspx">Manage Applications</a>
        </div>
        <div id="tabs">
            <div class="btnGroups">
                <h4><span>Manage Groups</span></h4>
            </div>
                                
             <div>
                <p class="lblSuccessText"></p> 
                <uc1:groupManagement runat="server" id="groupManagement" />
            </div>
        </div>
        <script>
            $(function () {
                $('.btnGroups').click(function () {
                    $('#divGroups').show();
                    $('#divGroupDetails').hide();
                    $('#btnBack').hide();
                });
                $('.newPermissions').click(function () {
                    $('.newAppPermission').show();
                    $('.newPermissions').hide();
                    $('.lblSuccessText').text("");
                });
                $("input[type=submit], button")
                .button();
                $('#cancelPermission').click(function () {
                    $('.newAppPermission').hide();
                    $('.newPermissions').show();
                    $("input[type=text]").val("");
                });
            })
        </script>
        <%} %>
    </div>
</asp:Content>
