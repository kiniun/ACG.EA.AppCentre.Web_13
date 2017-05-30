<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="applicationManagement.aspx.cs" Inherits="ACG.EA.AppCentre.Web.applicationManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(function () {
            $('#btnnewApplication').click(function () {
                $('#newApp').show();
                $('#btnnewApplication').hide();
                $('#divAppDetails').hide();
            });
        });
    </script>
    <style>
        div#newApp table, div#divAppDetails table, div#appResults table 
        { border-collapse: separate; border-spacing: 10px 10px; }

        div#newApp td.appHdrs, div#divAppDetails td, div#appResults td { vertical-align: top; }
        
        #appResults
        {
            margin-top: 20px
        }
        #btnnewApplication { cursor:pointer; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
    <h3 class="page-header">Application Management</h3>
    <%if((bool)Session["IsAppCentreAdmin"]) { %>
     <div>
         <label>Search By</label>&nbsp;
         <select id="fldColumnSearch">
             <option value="id">Id</option>
             <option value="name">Name</option>
         </select>
         <input type="text" name="applicationId" id="fldSearchtxt" data-bind="value: searchTxt, valueUpdate: 'keyup'" />
         <button id="btnSearchForApp" data-bind="click: searchApps">Search</button>
         <button id="btnClearSearch" data-bind="click: clearSearch">Clear</button>
     </div>
    <br />
    <table id="tblApplications" style="width: 100%"></table>
     <div id="pgrps"></div>
    <br />
        <p class="lblSuccessText"></p>
    
    <div style="display: none" id="divAppDetails">
    <hr />
        <div style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana">Editing Application</div>
            <table>
                <tr>
                    <td>Application Name:</td>
                    <td><input type="text" name="fldAppId" id="fldAppId" disabled maxlength="15"/></td>
                </tr>
                <tr>
                    <td>Long Name:</td>
                    <td><input type="text" name="fldAppName" id="fldAppName" maxlength="50" style="width:506px"/></td>
                </tr>
                <tr>
                    <td>Url:</td>
                    <td><textarea name="fldAppUrl" id="fldAppUrl" maxlength="215" rows="2" cols="60"></textarea></td>
                </tr>
                <tr>
                    <td>Description:</td>
                    <td><textarea name="fldAppDescription" id="fldAppDescription" rows="3" cols="60"></textarea></td>
                </tr>
            </table><br />
            <button id="btnSaveAppDetails" data-bind="click: saveAppDetails">Save Details</button>
            <button class="btnCancelEdit" data-bind="click: CancelAppEdit">Cancel</button>
    </div>

    <hr />

    <p><a href="newApplication.aspx">Add New Application</a></p>
    
    <% } %>
</asp:Content>

