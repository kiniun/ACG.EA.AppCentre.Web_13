<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="groupManagement.ascx.cs" Inherits="ACG.EA.AppCentre.Web.groupManagement" %>
    <div id="divGroups">
        <table>
            <thead>
                <tr>
                    <th style="padding-right: 10px;">Group Name</th>
                    <th>Group Description</th>
                </tr>
            </thead>
            <tbody data-bind="foreach: appGroups">
                <tr>
                    <td><span data-bind="text: group_Id, click: $parent.showGroupDetails" style="cursor:pointer; text-decoration: underline; padding-right: 10px; color: blue"></span>  
                        <%--<span data-bind="text: groupDescription" style="display:none"></span>--%>
                    </td>
                    <td data-bind="text: groupDescription"></td>
                </tr>
            </tbody>
        </table>
    </div>
    <div id="divGroupDetails" style="display: none">
        <span id="selectedGrp" style="display: none"></span>
        <p id="selGrpHeader"></p>
        <table>
            <thead>
                <tr>
                    <th style="padding-right: 20px">Permissions</th>
                    <th style="padding-right: 10px"></th>
                </tr>
            </thead>
            <tbody data-bind="foreach: grpOwnPermissions">
                <tr>
                    <td style="padding-right: 20px">
                        <input type="checkbox" name="chkPermission" class="chkPermission" data-bind="checked: isGranted"/>
                        <span style="display: none" data-bind="text: PERMISSION_ID"></span>
                        <span style="padding-left: 5px" data-bind="text: PERMISSION_NAME"></span>
                        <span style="display: none" data-bind="text: GROUP_PERMISSSION_ID" id="fldgrpPermissionId"></span>
                    </td>
                    <td><span data-bind="text: TARGET_VALUE"></span>
                        <span style="display: none" data-bind="text: TARGET_VALUE_ID"></span>
                    </td>
                </tr>
            </tbody>
        </table><br />
        <button data-bind="click: updateApplicationGrp" id="btnSaveGrpDetails">Update Group</button>
        <button data-bind="click: updateApplicationGrp" id="btnDelGroup">Delete Group</button>
    </div>
    <br />
    <%--<span id="btnBack" style="display: none; cursor: pointer; text-decoration: underline">Back to Groups</span>--%>
    <hr />
    <span style="font:bold 13px arial; cursor:default; text-decoration:underline" id="btnNewGroup">Add New Group</span>
    <div id="bxNewGroup" style="display:none">
        <span>New </span><span data-bind="text: applicationId"></span><span> Group</span>
        <div class="newGroup">
            <table>
                <thead>
                    <tr>
                        <td>Group Name</td>
                        <td>Group Description</td>
                        <%--<td>Set Permissions</td>--%>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td>
                            <input type="text" placeholder="group Id" name="groupId" data-bind="value: groupId, valueUpdate: 'input'" />
                        </td>
                        <td>
                            <input type="text" placeholder="group description" name="groupDesc" data-bind="value: groupName, valueUpdate: 'input'" />
                        </td>
                        <%--<td style="padding-right: 10px">
                            <input type="checkbox" name="permName" class="permName" data-bind="checked: setgrppermission, valueUpdate: 'keyup'" />
                        </td>--%>
                        <%--<td>
                        <select id="fldTargetValue" data-bind="options: appTargetValues,
    optionsText: 'target_name', optionsValue: 'target_Id', optionsCaption: 'Select Target...',
    value: targetId, valueUpdate: 'change'">
                        </select>
                    </td>--%>
                    </tr>
                </tbody>
            </table>
            <br />
            <div class="groupButtons">
                <button data-bind="click: addApplicationGrp" id="btnAddAppGroup">Save</button>
                <button id="btnCancelAppGrp">Cancel</button>
            </div>
        </div>
    </div>
    <script>
        $(function () {
            $("input[type=submit], button")
            .button();
            $('#btnNewGroup').click(function () {
                $('#bxNewGroup').show();
                $('#btnNewGroup').hide();
            });
            $('#btnCancelAppGrp').click(function () {
                $('#bxNewGroup').hide();
                $('#btnNewGroup').show();
                $("input[type=text]").val("");
                //$('#btnBack').hide();
            });
        });
    </script>