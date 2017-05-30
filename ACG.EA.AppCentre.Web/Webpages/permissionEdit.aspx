<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="permissionEdit.aspx.cs" Inherits="ACG.EA.AppCentre.Web.permissionEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">

        <%if ((bool)Session["IsAppAdmin"])
          { %>
    <div> 
        <h4><span id="manageAppBox" class="management">Manage User Permissions</span></h4>
        <table>
            <%--<tr>
                <td>All Avaialable Permissions:</td>
                <td><select data-bind="
    options: permissions,
    optionsText: 'PERMISSION_ID',
    optionsValue: 'PERMISSION_ID',
    optionsCaption: 'Choose...'" id="fldPermissionId" multiple="multiple"></select></td>
                </tr>--%>
            <tr>
                <td>
        <select id="fldApplicationId" data-bind="options: adminApps, optionsText: 'application_name',
    optionsValue: 'application_id', optionsCaption: 'Select App....',
    event: { change: showManageBox }, value: applicationId, valueUpdate: 'change'">
        </select></td>
                </tr>
        </table>

            <div id="bxUserPermissions">
                <br />
                <table id="tblAppUserPermissions"></table>
                <%--<table id="userpermission"></table>--%>
            </div>

        <div style="display: none" id="divPermissions">
        <div style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana">Editing Users Permission</div>
            <table>
                <tr>
                    <td>User Name:</td>
                    <td><input type="text" disabled="disabled" name="username" id="fldUsername"/></td>
                </tr>
                <tr>
                    <td>First Name:</td>
                    <td><input type="text" disabled="disabled" name="firstname" id="fldFirstname"/></td>
                </tr>
                <tr>
                    <td>Last Name:</td>
                    <td><input type="text" disabled="disabled" name="lastname" id="fldLastname"/></td>
                </tr>
                <tr>
                    <td>Permission:</td>
                    <td><input type="text" disabled="disabled" name="email" id="fldPermission"/></td>
                </tr>
                <tr>
                    <td><label for="fldGrant">Grant: </label></td>
                    <td><input type="checkbox" id ="fldGrant" value=""/></td>
                </tr>
            </table><br />
            <button id="btnsaveUserPermission">Save User Permission</button>
            <button class="btnCancel">Cancel</button>
        </div><hr />

        <div id="bxAddPermissions" style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana">Add User Permissions <span>(...click to show)</span></div>

        <div style="display: none" id="divAddPermissions">
            <div>
                <script>
                    $(function () {
                        Main.getAllUsers();
                    })
                </script>
                <table id="allUsers"></table><br />
            </div>
            <table>
                <tr>
                    <td>Application Permissions:<br />
                        <span style="font:bold 10px arial">Hold Ctrl key to add multiple</span>
                    </td>
                    <td>
                    <select id="fldPermissions" data-bind="options: appPermissions, optionsCaption: 'Select Permissions...'," multiple="multiple">
                                </select>
                    </td>
                </tr>
                <tr>
                    <td>Grant:</td>
                    <td><input type="checkbox" name="grantPermission" id="fldgrantPermission" data-bind="checked: grantPermission, valueUpdate: 'input'" />
                    </td>
                </tr>
            </table><br />
            <button id="btnAddPermission">Save Permissions</button>
            <button class="btnCancel">Cancel</button>
        </div>
            <hr /> 
        
        <div id="bxNewGroup" style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana">Manage Users Group <span>(...click to show)</span></div>

            <div style="display: none; " id="divNewGroup">
                <div style="margin-right: 100px">
                    Add User To Group(s)
            <table>
                <tr>
                    <td>Application Group:</td>
                    <td>
                        <div>
                            <select style="height: 100%" id="fldAppGroups" data-bind="options: appGroups, optionsCaption: 'Select Group...', optionsText: 'group_Id'" multiple="multiple">
                            </select>
                        </div>
                    </td>
                </tr>
            </table>
                    <br />
                    <button id="btUserToGroup">Save Group</button>
                    <%--<button id="btRmvUserFrmGroup">View User's Group</button>--%>
                    <button class="btnCancel">Cancel</button>
                </div>
                <div style="margin-top: 30px">
                    <button id="btnViewUsersGroups">View User's Group</button>
                    <table id="tblUserGrp"></table>
                    <button style="display: none" id="btRmvUserFrmGroup">Remove From Group</button>
                </div>
            </div>       
        <% } %>
    </div>
</asp:Content>
