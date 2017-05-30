<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="Applications.aspx.cs" Inherits="ACG.EA.AppCentre.Web.Applications" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        $(function () {

        });
    </script>
    <style>
        div.appResults table 
        { border-collapse: separate; border-spacing: 10px 10px; }
        
        div.appResults td { vertical-align: top; }
        
        .appResults
        {
            margin-top: 20px
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
        <h3 class="page-header">Application Search</h3>
        <div><label>Search By</label>&nbsp;
         <input type="text" name="applicationId" id="fldSearchTxt" />&nbsp;
         <label for="name"><input type="radio" name="fldSearchby" id="name" value="Name" checked />&nbsp;Name</label>&nbsp;
         <label for="keyword"><input type="radio" name="fldSearchby" id="keyword" value="Keyword" />&nbsp;Keyword</label>
         <%--<select id="fldappSearch">
             <option value="Name">Name</option>
             <option value="Keyword">Keyword</option>
         </select>--%>
         <button data-bind="click: loadSearchedUserApps">Search</button>
        </div>
        <div class="appResults" data-bind="visible: searchedUsersApps().length > 0">
            <table>
                <thead>
                    <tr>
                        <th>Application</th>
                        <th>Description</th>
                    </tr>
                </thead>
                <tbody data-bind="foreach: searchedUsersApps">
                    <tr>
                        <td>
                            <a href="#" data-bind="attr: { href: application_uri, title: application_id }" target="_blank"><span data-bind="text: application_name"></span></a>
                        </td>
                        <td>
                            <span data-bind="text: application_desc"></span>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    <hr />
    
</asp:Content>
