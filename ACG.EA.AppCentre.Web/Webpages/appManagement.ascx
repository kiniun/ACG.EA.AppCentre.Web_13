<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="appManagement.ascx.cs" Inherits="ACG.EA.AppCentre.Web.appManagement" %>
<h3 class="page-header">Application Management</h3>
    <%if((bool)Session["IsAppCentreAdmin"]) { %>
     <%--<div>
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
    <br />--%>
    
    <div style="display: none" id="divAppDetails">
    <hr />
        <div id="lblUserContextSuccess" style="display: none"></div>
        <div style="cursor: default; margin: 20px 0; font: bold 14px arial, verdana">Editing Application</div>
            <table>
                <tr>
                    <td>Application Id:</td>
                    <td><input type="text" name="fldAppId" id="fldAppId" maxlength="15"/></td>
                </tr>
                <tr>
                    <td>Application Name:</td>
                    <td><input type="text" name="fldAppName" id="fldAppName" maxlength="50"/></td>
                </tr>
                <tr>
                    <td>Uri:</td>
                    <%--<td><input type="text" name="fldAppUrl" id="fldAppUrl"/></td>--%>
                    <td><textarea name="fldAppUrl" id="fldAppUrl" maxlength="215" rows="2" cols="30"></textarea></td>
                </tr>
                <tr>
                    <td>Description:</td>
                    <%--<td><input type="text" name="fldAppDescription" id="fldAppDescription"/></td>--%>
                    <td><textarea name="fldAppDescription" id="fldAppDescription" rows="3" cols="30"></textarea></td>
                </tr>
            </table><br />
            <button id="btnSaveAppDetails">Save Details</button>
            <%--<button style="display: none" id="btnDeleteApp">Delete App</button>--%>
            <button class="btnCancelEdit" data-bind="click: CancelAppEdit">Cancel</button>
    </div>

    <hr />
        <%--<a href="newApplication.aspx" title="Add Application">Add New Application</a>--%>
        <a id="btnnewApplication">Add New Application</a>
    <div id="newApp" style="margin-top: 40px; display: none">
    <h4>New Application</h4>
    <table>
        <tbody style="margin-bottom:5px; display: grid">
            <tr>
                <td class="appHdrs">
                    Application Id:
                </td>
                <td>
                    <input type="text" maxlength="15" id="fldappId" placeholder="Id" name="appId" data-bind="value: id, valueUpdate: 'keyup'" /><label id="lblAppId" style="color:red; display: none">* Required</label>
                </td>
            </tr>
            <tr>
                <td class="appHdrs">
                    Name:
                </td>
                <td>
                    <input type="text" maxlength="50" id="fldappName" placeholder="Name" name="appName" data-bind="value: name, valueUpdate: 'keyup'" />
                    <label id="lblAppName" style="color:red; display: none">* Required</label>
                </td>
            </tr>
            <tr>
                <td class="appHdrs">
                    Url:
                </td>
                <td>
                    <%--<input maxlength="215" type="text" id="fldappUri" placeholder="Url" name="appUrl" data-bind="value: uri, valueUpdate: 'keyup'" />--%>
                    <textarea name="fldAppDescription" maxlength="215" id="fldappUri" placeholder="uri" rows="2" cols="30" data-bind="value: uri, valueUpdate: 'keyup'"></textarea>
                    <label id="lblAppUri" style="color:red; display: none">* Required</label>
                </td>
            </tr>
            <tr>
                <td class="appHdrs">
                    <span>Description:</span>
                </td>
                <td>
                    <%--<input type="text" id="fldappDesc" placeholder="Description" name="appDesc" data-bind="value: description, valueUpdate: 'input'" />--%>
                    <textarea name="fldAppDescription" id="fldappDesc" placeholder="description" rows="3" cols="30" data-bind="value: description, valueUpdate: 'keyup'"></textarea>
                    <label id="lblAppDesc" style="color:red; display: none">* Required</label>
                </td>
            </tr>
        </tbody>        
    </table><br />
    <button data-bind="click: addNewApp" id="addNewApp">Add New Application</button>
    <button class="btnCancelAdd" data-bind="click: CancelAddNewApp">Cancel</button>
    </div>
    <% } %>