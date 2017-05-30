<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditGroups.ascx.cs" Inherits="ACG.EA.AppCentre.Web.EditGroups" %>

<div style="margin-top: 75px">
    <span>View/Edit Group Permissions</span>
    <%--<div><span>Application: &nbsp;</span>
        <select id="fldEditAppGroups" data-bind="options: allApps, optionsText: 'application_name', optionsValue: 'application_id',
        optionsCaption: 'Select App....', event: { change: getApplicationGroups('edit') },
        value: applicationId, valueUpdate: 'keyup'">
                    </select>
    </div>--%>
    <div><span>Group: &nbsp;</span>
        <select id="fldEditGrpPermssions" data-bind="options: appGroups, optionsCaption: 'Select Group...', event: { change: setGroupPermissions }">
                    </select>
    </div>
    <div>
        <span id="selGrpPermissions" ></span><span> Permissions:</span>
        <div>
            <select data-bind="options: appPermissions, optionsCaption: 'Choose...'" id="fldPermissions" multiple="multiple"></select>
        </div>
    </div>
    <br />
    <button data-bind="click: addGrpPermission" id="btnAddGrpPermission">Save Group Permission</button>
</div>
