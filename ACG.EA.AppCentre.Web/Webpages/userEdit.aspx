<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="userEdit.aspx.cs" Inherits="ACG.EA.AppCentre.Web.userEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        td { padding-right: 10px;}
        div#divNewUser table { 	border-collapse: separate; border-spacing: 10px 10px; }
        div#divNewUser td.appHdrs { vertical-align: top; }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">
    <div>
        <div style="margin-top: 30px"><p>View and edit user profiles</p>
            <p style="font: bold 14px arial, verdana; height: 12px;">Double click to edit, click once on a user row to select for delete, add permissions and add to groups</p>
        <%if ((bool)Session["IsAppAdmin"])
          { %>
            <div>
            </div>
            <hr />
            <script>
                window.onkeypress = function (event) {
                    var uSearch = $('#btnSearchUser');
                    if (uSearch) {
                        if (event != undefined) {
                            if (event.keyCode == 13) {
                                uSearch.click();
                            }
                        }
                    }
                }
            </script>
        <div>
         <select id="fldColumnSearch">
             <option value="All">All</option>
             <option value="User_Name">User Name</option>
             <option value="First_Name">First Name</option>
             <option value="Last_Name">Last Name</option>
         </select>
         <input type="text" name="applicationId" id="fldSearchtxt" data-bind="value: searchTxt, valueUpdate: 'keyup'" />
         <button id="btnSearchUser" data-bind="click: searchUser">Search</button>
         <button id="btnClearSearch" data-bind="click: clearSearchUser">Clear</button>
            <br /><br />
            <table id="userView"></table><br />
            <div id="userViewEdit"></div>
     </div>
        <p class="lblSuccessText"></p>
        <div id="showAddUser" style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana">Add New/Edit Users <span>(...click to show)</span></div>
        
        <div style="display: none" id="divNewUser">
            <%--<div id="lblUserContextSuccess" style="display: none"></div>--%>
            <table>
                <tr>
                    <td class="appHdrs">User Name:</td>
                    <td><input type="text" placeholder="user name" name="username" id="fldUsername"/>
                    <span id="fldUserValidation"></span></td>
                </tr>
                <tr>
                    <td class="appHdrs">First Name:</td>
                    <td><input type="text" placeholder="first name" name="firstname" id="fldFirstname"/></td>
                </tr>
                <tr>
                    <td class="appHdrs">Last Name:</td>
                    <td><input type="text" placeholder="last name" name="lastname" id="fldLastname"/></td>
                </tr>
                <tr>
                    <td class="appHdrs">Email:</td>
                    <td><input type="text" placeholder="email" name="email" id="fldEmail"/></td>
                </tr>
                <tr>
                    <td class="appHdrs">Title:</td>
                    <td><input type="text" placeholder="title" name="title" id ="fldTitle"/></td>
                </tr>
                <tr>
                    <td class="appHdrs">Phone</td>
                    <td><input type="text" placeholder="phone" name="phone" id="fldPhoneNumber"/></td>
                </tr>
            </table><br />
            <button id="saveUserDetails">Save User</button>
            <button style="display: none" id="btnDeleteUser">Delete User</button>
            <button class="btnCancel">Cancel</button>
        </div>
        
        <% } %>
        </div>
    </div>
</asp:Content>
