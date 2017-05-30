<%@ Page Title="" Language="C#" MasterPageFile="~/Webpages/Site.Master" AutoEventWireup="true" CodeBehind="userPermissions.aspx.cs" Inherits="ACG.EA.AppCentre.Web.userPermissions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">

        <%if ((bool)Session["IsAppAdmin"])
          { %>
    <div> 
        <h4><span id="manageAppBox" class="management">Manage User Permissions</span></h4>
        <div>
        <select id="fldApplicationId" data-bind="options: adminApps, optionsText: 'application_name',
    optionsValue: 'application_id', optionsCaption: 'Select App....',
    event: { change: refreshUserDetails }, value: applicationId, valueUpdate: 'change'">
        </select>
        </div>
        <hr />
        <div id="userSection"><label>Search By</label>&nbsp;
             <select id="fldColumnSearch">
                 <%--<option value="0">Search by...</option>--%>
                 <option value="User_Name">User Name</option>
                 <option value="First_Name">First Name</option>
                 <option value="Last_Name">Last Name</option>
             </select>
             <input type="text" name="applicationId" id="fldSearchtxt" data-bind="value: searchTxt, valueUpdate: 'keyup'" />
             <button id="btnSearchUser" data-bind="click: searchUserPermission">Search</button>
             <button id="btnClearSearch" data-bind="click: clearSearchUser">Clear</button>
            <br />
            <table id="userPermissions"></table><br />
            <div id="userViewEdit"></div>
        </div>
            <hr /> 
        
        <div id="bxNewGroup" style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana" data-bind="click: getUserGroups">Manage Users Group <span>(...click to show)</span></div>
        <div style="display: none" id="divNewGroup">
            <p id="fldGroupUpdate"></p>       
            <table>
                <thead>
                    <tr>
                        <th style="padding-right: 20px"><span data-bind="text: applicationId"></span> Groups</th>
                        <th style="padding-right: 20px">Member</th>
                    </tr>
                </thead>
                <tbody data-bind="foreach: userGroups">
                    <tr>
                        <td style="padding-right: 20px">
                            <span class="fldUserGroups" style="padding-left: 5px" data-bind="text: GROUP_ID"></span>
                            <span style="display: none" data-bind="text: USER_APPLICATION_GROUP_ID" class="fldUserGroupId"></span>
                        </td>
                        <td>
                            <input type="checkbox" name="chkPermission" class="memberOf" data-bind="checked: isMember"/>
                        </td>
                    </tr>
                    </tbody>
            </table><br />
            <button data-bind="click: updateUserGroups" id="btnSaveUserGroups">Update User Groups</button>
            <button class="btnCancel">Cancel</button>

        </div>
        <hr />

        <div id="bxAddPermissions" style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana" data-bind="click: getUserPermissions">Manage User Permissions <span>(...click to show)</span></div>

        <div style="display: none" id="divPermissions">
            <p id="fldPermissionsUpdate"></p>       
            <table>
                <thead>
                        <tr>
                            <th style="padding-right: 20px">Grant</th>
                            <th style="padding-right: 20px"><span data-bind="text: applicationId"></span> Permissions</th>
                            <th style="padding-right: 10px">Targets</th>
                        </tr>
                </thead>
                <tbody data-bind="foreach: userPermissions">
                        <tr>
                            <td>
                                <input type="checkbox" name="chkPermission" class="hasPermssion" data-bind="checked: grant"/>
                            </td>
                            <td style="padding-right: 20px">
                                <span class="fldUserPermissions" style="padding-left: 5px" data-bind="text: permission_Id"></span>
                                <span style="display: none" data-bind="text: user_permission_Id" class="fldusrPermissionId"></span>
                            </td>
                            <td><span style="display: none" class="fldPermTargets" data-bind="text: target_value_Id"></span>
                                <span data-bind="text: target"></span>
                            </td>
                        </tr>
                    </tbody>
            </table><br />
            <button data-bind="click: updateUserPermissions" id="btnSavePermission">Update User Permissions</button>
            <button class="btnCancel">Cancel</button>

        </div>
                   
        <% } %>
    </div>
    <script>
        window.onkeypress = function () {
            var uSearch = $('#btnSearchUser');
            if (uSearch) {
                if (event.keyCode == 13) {
                    uSearch.click();
                }
            }
        }
    </script>
</asp:Content>