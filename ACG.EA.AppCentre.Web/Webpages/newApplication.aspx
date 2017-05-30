<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="newApplication.aspx.cs" Inherits="ACG.EA.AppCentre.Web.newApplication" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
    <% if ((bool)Session["IsAppCentreAdmin"]) { %>
    <h4 id="newApp" style="cursor:pointer">Add New Application</h4>
    <hr />
    <table>
        <thead style="margin-bottom:5px; display: grid">
            <tr>
                <td>
                    Application Id: 
                </td>
                <td>
                    <input type="text" id="fldappId" placeholder="Id" name="appId" data-bind="value: id, valueUpdate: 'keyup'" /><label id="lblAppId" style="color:red; display: none">* Required</label>
                </td>
            </tr>
            <tr>
                <td>
                    Name: 
                </td>
                <td>
                    <input type="text" id="fldappName" placeholder="Name" name="appName" data-bind="value: name, valueUpdate: 'keyup'" />
                    <label id="lblAppName" style="color:red; display: none">* Required</label>
                </td>
            </tr>
            <tr>
                <td>
                    Url: 
                </td>
                <td>
                    <input type="text" id="fldappUri" placeholder="Url" name="appUrl" data-bind="value: uri, valueUpdate: 'keyup'" />
                    <%--<label id="lblAppUri" style="color:red; display: none">* Required</label>--%>
                </td>
            </tr>
            <tr>
                <td>
                    Description: 
                </td>
                <td>
                    <input type="text" id="fldappDesc" placeholder="Description" name="appDesc" data-bind="value: description, valueUpdate: 'input'" />
                    <%--<label id="lblAppDesc" style="color:red; display: none">* Required</label>--%>
                </td>
            </tr>
        </thead>        
    </table>
    <button data-bind="click: addNewApp" id="addNewApp">Add New</button>
    <br /><br />
    <p class="lblSuccessText"></p> 
    <script type="text/javascript">
        
    </script>
    <% }
       else
       { %><p>You do not have permissions for this page</p>
    <% } %>
</asp:Content>
