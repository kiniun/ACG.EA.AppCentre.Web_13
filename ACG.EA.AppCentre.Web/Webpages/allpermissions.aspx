<%@ Page Title="" Language="C#" MasterPageFile="Site.Master" AutoEventWireup="true" CodeBehind="allpermissions.aspx.cs" Inherits="ACG.EA.AppCentre.Web.allpermissions" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">    
        <script>
            $(document).keypress(function (event) {
                var uSearch = $('#btnSearchUser');
                if (uSearch) {
                    if (event != undefined) {
                        if (event.keyCode == 13 || event.which == 13) {
                            uSearch.click();
                        }
                    }
                }
            });
        </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="DashBoardPlaceHolder" runat="server">

        <%if ((bool)Session["IsAppAdmin"])
          { %>
    <div> 
        <h4><span id="manageAppBox" class="management">Manage User Permissions</span></h4>
        <div id="userSection"><label>Search By</label>&nbsp;
             <select id="fldColumnSearch">
                 <option value="All">All</option>
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
        <div>
            <p>Application:</p>
        <select id="fldApplicationId" data-bind="options: adminApps, optionsText: 'application_name',
    optionsValue: 'application_id', optionsCaption: 'Select App....',
    event: { change: refreshUserDetails }, value: applicationId, valueUpdate: 'keydown'">
        </select>
        </div>
            <hr /> 
        
        <div id="bxNewGroup" style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana">Manage Users Group</div>
        <div style="display: none" id="divNewGroup">
            <p id="fldGroupUpdate"></p>       
            <table>
                <thead>
                    <tr>
                        <th style="padding-right: 20px"><span data-bind="text: applicationId"></span> Groups</th>
                        <th style="padding-right: 20px">Member</th>
                        <th class="checkedGroupPermissions">Permissions</th>
                    </tr>
                </thead>
                <tbody data-bind="foreach: userGroups">
                    <tr>
                        <td style="padding-right: 20px">
                            <span class="fldUserGroups" style="display: none" data-bind="text: GROUP_ID"></span>
                            <span style="padding-left: 5px" data-bind="text: GROUP_NAME"></span>
                            <span style="display: none" data-bind="text: USER_APPLICATION_GROUP_ID" class="fldUserGroupId"></span>
                        </td>
                        <td>
                            <input type="checkbox" name="chkPermission" class="memberOf" data-bind="checked: isMember"/>
                        </td>
                        <td class="checkedGroupPermissions" data-bind="text: Permissions"></td>
                        <td class="checkedGroupPermTargets" data-bind="text: TargetValues"></td>
                    </tr>
                    </tbody>
            </table><br />
            <button data-bind="click: updateUserGroups" id="btnSaveUserGroups">Update User Groups</button>
            <button class="btnCancel">Cancel</button>

        </div>
        <hr />

        <div id="bxAddPermissions" style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana">Manage User Permissions</div>

        <div style="display: none" id="divPermissions">
            <p id="fldPermissionsUpdate"></p>       
            <table>
                <thead>
                        <tr>
                            <th style="padding-right: 20px">Grant</th>
                            <th style="padding-right: 20px"><span data-bind="text: applicationId"></span> Permissions</th>
                            <th></th>
                        </tr>
                </thead>
                <tbody data-bind="foreach: userPermissions">
                        <tr data-bind="css: { 'groupEnabled': groupEnabled }">
                            <td>
                                <div class="chkBxImageShow" data-bind="style: { backgroundImage: grant == 'Explicit' ? 'url(../Support/Content/Images/chkbox3_on.GIF)' : grant == 'Group' ? 'url(../Support/Content/Images/chkbox2_on.GIF)' : group_permission_Id != null ? 'url(../Support/Content/Images/chkbox4_on.GIF)' : 'url(../Support/Content/Images/chkbox1_off.GIF)' }">&nbsp;
                                </div>
                                <div class="chkBxImage" data-bind="style: { backgroundImage: grant == 'Explicit' ? 'url(../Support/Content/Images/chkbox3_on.GIF)' : grant == 'Group' ? 'url(../Support/Content/Images/chkbox2_on.GIF)' : group_permission_Id != null ? 'url(../Support/Content/Images/chkbox4_on.GIF)' : 'url(../Support/Content/Images/chkbox1_off.GIF)' }">&nbsp;
                                </div>
                                <span class="chkBxChecked" data-bind="text: grant == 'None' ? 'off' : ''"></span>
                            </td>
                            <td style="padding-right: 20px">
                                <span class="fldUserPermissions" style="padding-left: 5px;" data-bind="text: permission_Id"></span>
                                <span data-bind="text: permission_Name"></span>
                                <span data-bind="text: user_permission_Id" class="fldusrPermissionId"></span>
                                <span data-bind="text: group_permission_Id" class="fldusrGrpPermissionId"></span>
                                <span class="fldusrGrpPermEnable"></span>
                            </td>
                            <td>         
                                <span class="fldPermTargetValue" data-bind="text: targetValueId"></span>
                                <span data-bind="text: targetValue"></span>
                            </td>
                        </tr>
                    </tbody>
            </table><br />
            <button data-bind="click: updateUserPermissions" id="btnSavePermission">Update User Permissions</button>
            <button class="btnCancel">Cancel</button>

        </div>
                   
        <% } %>
    </div>
</asp:Content>
