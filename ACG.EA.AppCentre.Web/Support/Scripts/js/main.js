(function (Main, _, $, undefined) {
    Main.url = "http://" + location.host + "/AppcentreApi/", user = new User(), Main.selectedApp;
    var isUpdate = false, isUserEditClear;
    $(document).ready(function () {
        ko.applyBindings(new ApplicationsModel());
        $(document).on('change', '.memberOf', vwSelectedGrpPermissions);
        $(document).on('click', '.chkBxImageShow', handleUserPermissions);
        $(document).on('click', '#saveUserDetails', saveUser);
        $(document).on('blur', '#fldUsername', checkIfExistUser);
        $(document).on('blur', '#fldappId', checkIfAppExists);
        $(document).on('click', '#btnDeleteUser', removeUser);
        //$(document).on('click', '#btnSaveAppDetails', saveAppDetails);
        $(document).on('click', '.btnCancel', Main.cancel);
        var links = $(".navsidebar > li > a").attr("html");
        $(document).on('change', '#fldActLogsAppId', Main.loadActivityLog);
        $(document).on('click', '#showAddUser', showAddUser);
    });

    function vwSelectedGrpPermissions() {
        var appGroup = { applicationId: "", groupId: "" };
        var grpParent, group;
        grpParent = this.parentElement;
        if (grpParent.previousElementSibling != undefined)
            group = grpParent.previousElementSibling.children[0].innerHTML;
        else {
            group = grpParent.previousSibling.children[0].innerHTML;
        }
        appGroup.applicationId = $('#fldApplicationId option:selected').val();
        //appGroup.groupId = encodeURIComponent(group);
        appGroup.groupId = group;
        var permFields = $('td.checkedGroupPermissions');
        var groupFields = $('span.fldUserGroups');
        var targetFields = $('td.checkedGroupPermTargets');
        var perms = "", tar = "";
        Main.permissionChecked = true;
        if (this.checked) {
            animatePermissions(this);
        }
        else {
            unsetGroupPermissions(this, permFields, targetFields);
        }
    }

    function setGroupPermissionsField(data, permField, targetField) {
        var perms = "", tar = "";
        $.each(data, function (key, value) {
            perms = perms == "" ? perms = value.PERMISSION_ID + ", " : perms = perms + value.PERMISSION_ID + ", ";
            tar = tar == "" ? tar = value.TARGET_ID + ", " : tar = tar + value.TARGET_ID + ", ";

        });
        permField.innerHTML = perms; targetField.innerHTML = tar;
    }

    function animatePermissions(sender) {
        var userPermissions = $('span.fldUserPermissions');
        var userPermTargets = $('span.fldPermTargetValue');
        var grpPermEnable = $('span.fldusrGrpPermEnable');
        var perm = "", tar = "";
        var grpPermissions = [], grpTargetValues = [];
        if (sender.parentElement.nextElementSibling != undefined) {
            perm = sender.parentElement.nextElementSibling.innerHTML;
            tar = sender.parentElement.nextElementSibling.nextElementSibling.innerHTML;
        }
        else {
            perm = sender.parentElement.nextSibling.innerHTML;
            tar = sender.parentElement.nextSibling.nextSibling.innerHTML;
        }
        if (perm.indexOf(",") == -1) {
            grpPermissions.push(perm);
        }
        else {
            grpPermissions = perm.split(",");
        }
        if (tar.indexOf(",") == -1) {
            grpTargetValues.push(tar);
        }
        else {
            grpTargetValues = tar.split(",");
        }
        grpPermissions.sort();
        grpTargetValues.sort();
        if (grpPermissions.length > 0) {
            for (var i = 0; i < grpPermissions.length; i++) {
                if (userPermissions.length > 0) {
                    for (var j = 0; j < userPermissions.length; j++) {
                        //var chkObj = userPermissions[j].parentNode.previousElementSibling || userPermissions[j].parentNode.previousSibling;
                        //var grpPermObj = userPermissions[j].nextElementSibling.nextElementSibling.nextElementSibling || userPermissions[j].parentNode.nextSibling.nextSibling.nextSibling;
                        var chkObj, grpPermObj;
                        if (userPermissions[j].parentNode.previousElementSibling != undefined)
                            chkObj = userPermissions[j].parentNode.previousElementSibling;
                        else
                            chkObj = userPermissions[j].parentNode.previousSibling;
                        if (userPermissions[j].nextElementSibling != undefined)
                            grpPermObj = userPermissions[j].nextElementSibling.nextElementSibling.nextElementSibling;
                        else
                            grpPermObj = userPermissions[j].nextSibling.nextSibling.nextSibling;

                        if (userPermissions[j].innerHTML == grpPermissions[i]) {
                            if (userPermTargets[j].innerHTML != "" && grpTargetValues[i]) { 
                                if (userPermTargets[j].innerHTML == grpTargetValues[i]) {
                                    if (chkObj.children[0].style.backgroundImage.indexOf("chkbox3_on") == -1) {
                                        if (chkObj.children[2].innerHTML == "off") {
                                            if (grpPermObj.innerHTML != "") {
                                                if (grpPermObj.innerHTML != 0)
                                                    chkObj.children[0].style.backgroundImage = grpPermDeny;
                                                else
                                                    chkObj.children[0].style.backgroundImage = byGroup;
                                            }
                                            else
                                                chkObj.children[0].style.backgroundImage = byGroup;
                                        }
                                        else {
                                            chkObj.children[0].style.backgroundImage = byGroup;
                                        }
                                        grpPermEnable[j].innerHTML = "enabled";
                                    }
                                }
                            }
                            else {
                                if (chkObj.children[0].style.backgroundImage.indexOf("chkbox3_on") == -1) {
                                    if (chkObj.children[2].innerHTML == "off") {
                                        if (grpPermObj.innerHTML != "") {
                                            if (grpPermObj.innerHTML != 0)
                                                chkObj.children[0].style.backgroundImage = grpPermDeny;
                                            else
                                                chkObj.children[0].style.backgroundImage = byGroup;
                                        }
                                        else
                                            chkObj.children[0].style.backgroundImage = byGroup;
                                    }
                                    else {
                                        chkObj.children[0].style.backgroundImage = byGroup;
                                    }
                                    grpPermEnable[j].innerHTML = "enabled";
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    function unsetGroupPermissions(currentObj, grpPermissions, grpTargets) {
        var permState = $('div.chkBxImageShow');
        var grpPermEnable = $('span.fldusrGrpPermEnable');
        var userPermissions = $('span.fldUserPermissions');
        var userPermTargets = $('span.fldPermTargetValue');
        //var grpPermissions = $('td.checkedGroupPermissions');
        //var grpTargets = $('td.checkedGroupPermTargets');
        var grpBoxes = $('input.memberOf');
        var thisGrpPermission = [], thisGrpPermTargets = [];
        var thisPermission = "", thisPermTarget = "";
        if (currentObj.parentNode.nextElementSibling != undefined) {
            thisPermission = currentObj.parentNode.nextElementSibling.innerHTML;
            thisPermTarget = currentObj.parentNode.nextElementSibling.nextElementSibling.innerHTML;
            if (thisPermission.indexOf(",") != -1) {
                thisGrpPermission = thisPermission.split(",");
                thisGrpPermTargets = thisPermTarget.split(",");
            }
            else {
                thisGrpPermission.push(thisPermission);
                thisGrpPermTargets.push(thisPermTarget);
            }
//            thisPermTarget = currentObj.parentNode.nextElementSibling.nextElementSibling.innerHTML;
//            if (thisPermission.indexOf(",") != -1) {
//                thisGrpPermTargets = thisPermTarget.split(",");
//            }
//            else
//                thisGrpPermTargets.push(thisPermTarget);
        }
        else {
            thisPermission = currentObj.parentNode.nextSibling.innerHTML;
            thisPermTarget = currentObj.parentNode.nextSibling.nextSibling.innerHTML;
            if (thisPermission.indexOf(",") != -1) {
                thisGrpPermission = thisPermission.split(",");
                thisGrpPermTargets = thisPermTarget.split(",");
            }
            else {
                thisGrpPermission.push(thisPermission);
                thisGrpPermTargets.push(thisPermTarget);
            }
//            if (thisPermission.indexOf(",") != -1) {
//                thisGrpPermTargets = thisPermTarget.split(",");
//            }
//            else
//                thisGrpPermTargets.push(thisPermTarget);
        }
        if (thisGrpPermission.length > 0) { //enumerate grpPermissions.length instead of thisGrpPermission.length - both refer permissions listed for groups
            var permToKeep = [], enumGrpPerm;
            //permToKeep: array of permissions that are still valid based groups that are currently checked
            //enumGrpPerm: a boolean that determines which group to enumerate
            for (var k = 0; k < grpPermissions.length; k++) {
                enumGrpPerm = true;
                if (grpPermissions[k].previousElementSibling != undefined) {
                    if (currentObj == grpPermissions[k].previousElementSibling.children[0])
                        enumGrpPerm = false;
                }
                else {
                    if (currentObj == grpPermissions[k].previousSibling.children[0])
                        enumGrpPerm = false;
                }
                if (enumGrpPerm) {
                    var currGrpPerms = [], currGrpTargts = [];
                    if (grpPermissions[k].innerHTML != "") {
                        if (grpPermissions[k].innerHTML.indexOf(",")) {
                            currGrpPerms = grpPermissions[k].innerHTML.split(",");
                        }
                        else
                            currGrpPerms.push(grpPermissions[k].innerHTML);
                    }
                    else
                        continue;
                    if (grpTargets[k]) {
                        if (grpTargets[k].innerHTML != "") {
                            if (grpTargets[k].innerHTML.indexOf(",")) {
                                currGrpTargts = grpTargets[k].innerHTML.split(",");
                            }
                            else
                                currGrpTargts.push(grpTargets[k]);
                        }
                    }
                    currGrpPerms.sort();
                    if (currGrpTargts.length > 0) {
                        currGrpTargts.sort();
                    }
                    if (grpBoxes[k].checked) {
                        for (var j = 0; j < userPermissions.length; j++) {
                            if (currGrpPerms.length > 0) {
                                for (var m = 0; m < currGrpPerms.length; m++) {                                    
                                    if (currGrpPerms[m] == userPermissions[j].innerHTML) { // check if the permission id for the current checked group matches that of the current (j) permission id
                                        var permTargetObj = { permissionId: "", targetValueId: "" };
                                        if (userPermTargets[j] && currGrpTargts[m]) {
//                                            if (currGrpTargts[m]) {
                                            if (userPermTargets[j].innerHTML == currGrpTargts[m]) { // check if the target value for the permission id of the current checked group matches the target value of the current (j) permission line
                                                permTargetObj.permissionId = currGrpPerms[m];
                                                if (currGrpTargts[m]) {
                                                    permTargetObj.targetValueId = currGrpTargts[m];
                                                }
                                            }
//                                            }
                                            else if (userPermTargets[j].innerHTML == "")
                                                permTargetObj.permissionId = userPermissions[j].innerHTML;
                                        }
                                        else {
                                            permTargetObj.permissionId = userPermissions[j].innerHTML;
                                        }
                                        if (permToKeep.length > 0) {
                                            for (var p = 0; p < permToKeep.length; p++) {
                                                if (permToKeep[p].permissionId != currGrpPerms[m]) {
                                                    if (currGrpTargts[m]) {
                                                        if (permToKeep[p].targetValueId != currGrpTargts[m]) {
                                                            permToKeep.push(permTargetObj);
                                                            break;
                                                        }
                                                    }
                                                    else {
                                                        permToKeep.push(permTargetObj);
                                                        break;
                                                    }
                                                }
                                                else
                                                {
                                                    if (currGrpTargts[m])
                                                    {
                                                        if (permToKeep[p].targetValueId != currGrpTargts[m])
                                                        {
                                                            if (permTargetObj.permissionId != "" && permTargetObj.targetValueId != "")
                                                            {
                                                                permToKeep.push(permTargetObj);
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else {
                                            if (permTargetObj.permissionId != "")
                                                permToKeep.push(permTargetObj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    continue;
            }
            var uncheckedPermissions = [];
            permToKeep.sort();
            if (permToKeep.length > 0) {
                for (var p = 0; p < thisGrpPermission.length; p++) {
                    var removeLine = false;
                    for (var i = 0; i < permToKeep.length; i++) {
                        var permTargetObj = { permissionId: "", targetValueId: "" };
                        if (thisGrpPermission[p] == permToKeep[i].permissionId) {
                            if (permToKeep[i].targetValueId != "") {
                                if (thisGrpPermTargets[p]) {
                                    if (permToKeep[i].targetValueId == thisGrpPermTargets[p]) {
                                        removeLine = false;
                                        break;
                                    }
                                    else
                                        removeLine = true;
                                }
                                else {
                                    removeLine = false;
                                    break;
                                }
                            }
                            else {
                                removeLine = false;
                                break;
                            }
                        }
                        else {
                            if (permToKeep[i].targetValueId != "") {
                                if (thisGrpPermTargets[p]) {
                                    if (permToKeep[i].targetValueId != thisGrpPermTargets[p]) {
                                        removeLine = true;
                                    }
                                }
                                else
                                    removeLine = true;
                            }
                            else
                                removeLine = true;
                        }
                    }
                    if (removeLine) {
                        permTargetObj.permissionId = thisGrpPermission[p];
                        permTargetObj.targetValueId = thisGrpPermTargets[p] ? thisGrpPermTargets[p] : "";
                        if (uncheckedPermissions.length > 0) {
                            for (var x = 0; x < uncheckedPermissions.length; x++) {
                                if (uncheckedPermissions[x].permissionId == thisGrpPermission[p]) {
                                    if (thisGrpPermTargets[p]) {
                                        if (uncheckedPermissions[x].targetValueId != thisGrpPermTargets[p]) {
                                            uncheckedPermissions.push(permTargetObj);
                                            break;
                                        }
                                    }
                                    else {
                                        uncheckedPermissions.push(permTargetObj);
                                        break;
                                    }
                                }
                                else {
                                    uncheckedPermissions.push(permTargetObj);
                                    break;
                                }
                            }
                        }
                        else
                            uncheckedPermissions.push(permTargetObj);
                    }
                }
            }
            else {
                thisGrpPermission.sort();
                for (var i = 0; i < thisGrpPermission.length; i++) {
                    var permTargetObj = { permissionId: "", targetValueId: "" };
                    permTargetObj.permissionId = thisGrpPermission[i];
                    if (thisGrpPermTargets[i]) {
                        permTargetObj.targetValueId = thisGrpPermTargets[i];
                        uncheckedPermissions.push(permTargetObj);
                    }
                    else {
                        uncheckedPermissions.push(permTargetObj);
                    }

                }
            }
            for (var l = 0; l < userPermissions.length; l++) {
                if (uncheckedPermissions.length > 0) {
                    for (var f = 0; f < uncheckedPermissions.length; f++) {
                        if (userPermissions[l].innerHTML == uncheckedPermissions[f].permissionId) {
                            if (userPermTargets[l]) {
                                if (uncheckedPermissions[f].targetValueId != "") {
                                    if (userPermTargets[l].innerHTML == uncheckedPermissions[f].targetValueId) {
                                        if (permState[l].style.backgroundImage.indexOf("chkbox3_on") == -1 && permState[l].style.backgroundImage.indexOf("chkbox4_on") == -1) {
                                            permState[l].style.backgroundImage = off;
                                            grpPermEnable[l].innerHTML = "disabled";
                                        }
                                        else {
                                            if (grpPermEnable[l].innerHTML == "enabled") {
                                                grpPermEnable[l].innerHTML = "disabled";
                                            }
                                        }
                                    }
                                }
                                else {
                                    if (permState[l].style.backgroundImage.indexOf("chkbox3_on") == -1 && permState[l].style.backgroundImage.indexOf("chkbox4_on") == -1) {
                                        permState[l].style.backgroundImage = off;
                                        grpPermEnable[l].innerHTML = "disabled";
                                    }
                                    else {
                                        if (grpPermEnable[l].innerHTML == "enabled") {
                                            grpPermEnable[l].innerHTML = "disabled";
                                        }
                                    }
                                }
                            }
                            else {
                                if (permState[l].style.backgroundImage.indexOf("chkbox3_on") == -1 && permState[l].style.backgroundImage.indexOf("chkbox4_on") == -1) {
                                    permState[l].style.backgroundImage = off;
                                    grpPermEnable[l].innerHTML = "disabled";
                                }
                                else {
                                    if (grpPermEnable[l].innerHTML == "enabled") {
                                        grpPermEnable[l].innerHTML = "disabled";
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        else
            return;
    }

    var off = 'url(../Support/Content/Images/chkbox1_off.gif)';
    var expl = 'url(../Support/Content/Images/chkbox3_on.gif)';
    var byGroup = 'url(../Support/Content/Images/chkbox2_on.gif)';
    var grpPermDeny = 'url(../Support/Content/Images/chkbox4_on.gif)';

    function handleUserPermissions() {
        var currentImage = this.style.backgroundImage;
        var usrGrpPermId = "", pEnable = "";
        //var bxOff = this.nextElementSibling.nextElementSibling || this.nextSibling.nextSibling;
        if (this.parentNode.nextElementSibling != undefined) {
            usrGrpPermId = this.parentNode.nextElementSibling.children[3].innerHTML;
            pEnable = this.parentNode.nextElementSibling.children[4].innerHTML;
        }
        else {
            usrGrpPermId = this.parentNode.nextSibling.children[3].innerHTML;
            pEnable = this.parentNode.nextSibling.children[4].innerHTML;
        }
        if (currentImage.indexOf("chkbox1_off") != -1) {
            if (usrGrpPermId != "") {
                if (usrGrpPermId != "0") {
                    if (pEnable != "disabled") {
                        this.style.backgroundImage = byGroup;
                        if (this.nextElementSibling != undefined) {
                            this.nextElementSibling.style.backgroundImage = byGroup;
                        }
                        else {
                            this.nextSibling.style.backgroundImage = byGroup;
                        }
                    }
                    else {
                        this.style.backgroundImage = expl;
                        if (this.nextElementSibling != undefined) {
                            this.nextElementSibling.style.backgroundImage = expl;
                        }
                        else {
                            this.nextSibling.style.backgroundImage = expl;
                        }
                    }
                }
                else {
                    if (pEnable == "enabled") {
                        this.style.backgroundImage = byGroup;
                        if (this.nextElementSibling != undefined) {
                            this.nextElementSibling.style.backgroundImage = byGroup;
                        }
                        else {
                            this.nextSibling.style.backgroundImage = byGroup;
                        }
                    }
                    else {
                        this.style.backgroundImage = expl;
                        if (this.nextElementSibling != undefined) {
                            this.nextElementSibling.style.backgroundImage = expl;
                        }
                        else {
                            this.nextSibling.style.backgroundImage = expl;
                        }
                    }
                }
            }
            else if (pEnable == "enabled") {
                this.style.backgroundImage = byGroup;
                if (this.nextElementSibling != undefined) {
                    this.nextElementSibling.style.backgroundImage = byGroup;
                }
                else {
                    this.nextSibling.style.backgroundImage = byGroup;
                }
            }
            else {
                this.style.backgroundImage = expl;
                if (this.nextElementSibling != undefined) {
                    this.nextElementSibling.style.backgroundImage = expl;
                }
                else {
                    this.nextSibling.style.backgroundImage = expl;
                }
            }
            //getPermissionTargets(this);
        }
        else if (currentImage.indexOf("chkbox2_on") != -1) {
            this.style.backgroundImage = expl;
            if (this.nextElementSibling != undefined) {
                this.nextElementSibling.style.backgroundImage = expl;
            }
            else {
                this.nextSibling.style.backgroundImage = expl;
            }
            //getPermissionTargets(this);
        }
        else if (currentImage.indexOf("chkbox3_on") != -1) {
            this.style.backgroundImage = off;
            if (this.nextElementSibling != undefined) {
                this.nextElementSibling.style.backgroundImage = off;
            }
            else {
                this.nextSibling.style.backgroundImage = off;
            }
            //obj.parentNode.nextElementSibling.nextElementSibling.nextElementSibling.children[0].str.display = 'none';
        }
        else if (currentImage.indexOf("chkbox4_on") != -1) {
            this.style.backgroundImage = byGroup;
            if (this.nextElementSibling != undefined) {
                this.nextElementSibling.style.backgroundImage = byGroup;
            }
            else {
                this.nextSibling.style.backgroundImage = byGroup;
            }
            //obj.parentNode.nextElementSibling.nextElementSibling.nextElementSibling.children[0].str.display = 'none';
        }
    }

    //activity log model
    var activity = {
        colNames: ["User", "Application", "Target", "Last Accessed"],
        colModel: [{ name: "USER_NAME", width: 100, align: 'center' },
            { name: "APPLICATION_NAME", width: 200, align: 'center' },
            { name: "TARGET_VALUE", width: 200, align: 'center' },
            { name: "ACTIVITY_DATE", width: 200, align: 'center' },
        ]
    };
    Main.loadActivityLog = function () {
        var app = $('#fldActLogsAppId').val();
        $("#tblActivityLog").jqGrid("GridUnload");
        $("#tblActivityLog").jqGrid({
            datatype: "json",
            loadonce: true,
            url: Main.url + "loadAppUsersActivityLog/" + app,
            colNames: activity.colNames,
            colModel: activity.colModel,
            height: "100%",
            postData: "",
            caption: (app ? app : "Select") + " Activity Table",
            viewrecords: true,
            ajaxGridOptions: { cache: false }
        });
    }

    //users model for jqGrid
    Main.users = {
        colNames: ["User Id", "User Name", "Title", "First Name", "Last Name", "Email", "Phone", "Date Created", "Last Modified", "Modified By"],
        colModel: [{ name: "User_Id", hidden: true },
            { name: "User_Name", align: 'center' },
            { name: "Title", align: 'center' },
            { name: "First_Name", align: 'center' },
            { name: "Last_Name", align: 'center' },
            { name: "Email", align: 'center' },
            { name: "Phone", align: 'center' },
            { name: "Create_Date", align: 'center', dateformat: 'dd-MM-yyyy' },
            { name: "Last_Modified", align: 'center', format: 'dd-MM-yyyy' },
            { name: "Modified_By", align: 'center' },
        ]
    };

    var appUserGroups = {
        colNames: ["User Name", "User Id", "First Name", "Last Name", "Application", "Group"],
        colModel: [{ name: "User_Name", align: 'center', sortable: true },
            { name: "User_Id", hidden: true, sortable: true },
            { name: "First_Name", align: 'center', sortable: true },
            { name: "Last_Name", align: 'center', sortable: true },
            { name: "application_Id", align: 'center', sortable: true },
            { name: "groupName", align: 'center', sortable: true },
        ]
    };
    var appGroupPermissions = {
        colNames: ["Application", "Group", "Group Permission Id", "Permission Id", "Permission", "Granted", "Target Value Id", "Target Value"],
        colModel: [{ name: "APPLICATION_ID", align: 'center', sortable: true },
            { name: "GROUP_ID", sortable: true },
            { name: "GROUP_PERMISSION_ID", align: 'center', sortable: true, hidden: true },
            { name: "PERMISSION_ID", align: 'center', sortable: true },
            { name: "PERMISSION_NAME", align: 'center', sortable: true },
            { name: "isGranted", align: 'center', sortable: true, hidden: true },
            { name: "TARGET_VALUE_ID", align: 'center', sortable: true, hidden: true },
            { name: "TARGET_VALUE", align: 'center', sortable: true },
        ]
    };
    //application model for jqGrid
    Main.apps = {
        colNames: ["Application Id", "Application Name", "Application Url", "Application Description", "Catalog Name"],
        colModel: [{ name: "application_id", width: 100, align: 'center' },
            { name: "application_name", width: 150, align: 'center' },
            { name: "application_uri", width: 250, align: 'center' },
            { name: "application_desc", width: 200, align: 'center' },
            { name: "catalog_id", width: 75, align: 'center' },
        ]
    };

    Main.getFilteredUsers = function (schText, searchBy) {
        isUserEditClear = false;
        Main.cancel();
        $("#userView").jqGrid("GridUnload");
        var search = false;
        if (schText != "")
            search = true;
        else if (searchBy == "All") {
            schText = "all";
            search = true;
        }
        else
            alert("enter a search request");
        if (search) {
            $("#userView").jqGrid({
                datatype: "json",
                loadonce: true,
                url: Main.url + "getAllAppUsers/" + searchBy + "/" + schText,
                colNames: Main.users.colNames,
                colModel: Main.users.colModel,
                height: "100%",
                autowidth: true,
                caption: "Users Profile",
                postData: "",
                viewrecords: true,
                pager: '#userViewEdit',
                cursor: "pointer",
                ajaxGridOptions: { cache: false },
                onSelectRow: function () {
                    Main.selectedRow = $(this).getGridParam('selrow');
                    Main.selectUserId = $(this).jqGrid('getRowData', Main.selectedRow)['User_Id'];
                    Main.selectedUserProfile = $(this).jqGrid('getRowData', Main.selectedRow);
                    editSelectUserProfile(Main.selectedUserProfile);
                    showAddUser(); $('p.lblSuccessText').text("");
                    isUserEditClear = true;
                },
                ondblClickRow: function () {
                },
                gridComplete: function () {
                    isUserEditClear = true;
                }
            });
        }
        $("#userView").jqGrid('navGrid', '#userViewEdit', { edit: false, add: false, del: false, refresh: true, search: false }, {}, {}, {}, { multipleSearch: false, multipleGroup: false });
        $("#userView").jqGrid('inlineNav', '#userViewEdit',
            {
                edit: false, editicon: "ui-icon-pencil",
                add: false, addicon: "ui-icon-plus",
                save: false, saveicon: "ui-icon-disk",
                cancel: false, cancelicon: "ui-icon-cancel"
            }
        );
    }

    Main.getAllUsers = function () {
        $("#allUsers").jqGrid("GridUnload");
        $("#allUsers").jqGrid({
            datatype: "json",
            loadonce: true,
            url: Main.url + "getAllUsers/",
            colNames: Main.users.colNames,
            colModel: Main.users.colModel,
            height: "100%",
            caption: "App Users Table",
            postData: "",
            viewrecords: true,
            cursor: "pointer",
            ajaxGridOptions: { cache: false },
            onSelectRow: function () {
                Main.selectUserId = $(this).jqGrid('getRowData', $(this).getGridParam('selrow'))['User_Id'];
                //$('#fldSelectedUser').val($(this).jqGrid('getRowData', $(this).getGridParam('selrow'))['User_Name']);
                Main.selectedRow = $(this).getGridParam('selrow');
                Main.selectUserId = $(this).jqGrid('getRowData', Main.selectedRow)['User_Id'];
                Main.selectedUserProfile = $(this).jqGrid('getRowData', Main.selectedRow);
                editSelectUserProfile(Main.selectedUserProfile);
                showAddUser();
            },
            ondblClickRow: function () {
            }
        });
    }

    Main.getApplications = function (schText, srchBy) {
        
        //$('.lblSuccessText').text("");
        if (srchBy && schText) {
            $("#tblApplications").jqGrid("GridUnload");
            $("#tblApplications").jqGrid({
                datatype: "json",
                loadonce: true,
                url: Main.url + "filterApplications/" + srchBy + "/" + schText,
                colNames: Main.apps.colNames,
                colModel: Main.apps.colModel,
                width: 900,
                caption: "Applications Table",
                ajaxGridOptions: { cache: false },
                rowList: [10, 20, 30],
                pager: '#pgrps',
                sortname: 'application_id',
                viewrecords: true,
                sortorder: "desc",
                jsonReader: { repeatitems: false },
                height: '100%',
                onSelectRow: function () {
                    selectedApp = $(this).jqGrid('getRowData', $(this).getGridParam('selrow'));
                    setApplicationEdit();
                },
                //ondblClickRow: function () {
                //    selectedApp = $(this).jqGrid('getRowData', $(this).getGridParam('selrow'));
                //    setApplicationEdit();
                //}
            });
        }
        $("#tblApplications").jqGrid('navGrid', '#pgrps', { edit: false, add: false, del: false, refresh: false, search: false }, {}, {}, {}, { multipleSearch: false, multipleGroup: false });
    }

    Main.showAppUserGroupPermissions = function (appId, grp) {
        var grid = $("#tblAppUserPermissions");
        grid.jqGrid("GridUnload");
        //Main.cancel;
        var appGroup = { applicationId: appId, groupId: grp };
        $("#tblAppUserPermissions").jqGrid({
            datatype: "json",
            loadonce: true,
            url: Main.url + "getApplicationUsers/",
            colNames: appUserGroups.colNames,
            colModel: appUserGroups.colModel,
            autowidth: true,
            height: "100%",
            caption: appId + " Users Table",
            postData: appGroup,
            viewrecords: true,
            gridview: true,
            rownumbers: false,
            rowList: [10, 20, 30],
            sortable: true,
            pager: '#pager',
            ajaxGridOptions: { cache: true }
        });
        grid = $("#tblAppUserPermissions");
        $("#tblAppUserPermissions").navGrid('#pager', {
            search: false,
            add: false,
            del: false,
            refresh: true
        }, {}, // default settings for edit
        {}, // default settings for add
        {}, // delete
        {
            closeOnEscape: true, closeAfterSearch: true, ignoreCase: true, multipleSearch: false, multipleGroup: false, showQuery: false,
            sopt: ['cn', 'eq', 'ne'],
            defaultSearch: 'cn'
        }).navButtonAdd('#pager', {
            caption: "Export to Excel",
            buttonicon: "ui-icon-disk",
            onClickButton: function () {
                //ExportDataToExcel(grid, appId + "_UserPermissionsReport.xlsx");
                ExportJQGridData(grid, appId + "_UserPermissionsReport.xlsx");
            },
            position: "last"
        }).navButtonAdd('#pager', {
            caption: "Export to PDF",
            buttonicon: "ui-icon-disk",
            onClickButton: function () {
                ExportJQGridData(grid, appId + "_UserPermissionsReport.pdf");
            },
            position: "last"
        });
    }

    Main.showAppGroupPermissions = function (appId, grp) {
        var grid = $("#tblAppGrpPermissions");
        $("#tblAppGrpPermissions").jqGrid("GridUnload");
        var appGroup = { applicationId: appId, groupId: grp };
        $("#tblAppGrpPermissions").jqGrid({
            datatype: "json",
            loadonce: true,
            url: Main.url + "loadAppGroupPermissions/",
            colNames: appGroupPermissions.colNames,
            colModel: appGroupPermissions.colModel,
            autowidth: true,
            height: "100%",
            caption: appId + " Group Permissions",
            postData: appGroup,
            viewrecords: true,
            gridview: true,
            rownumbers: false,
            rowList: [10, 20, 30],
            sortable: true,
            pager: '#pager'
        });
        grid = $("#tblAppGrpPermissions");
        $("#tblAppGrpPermissions").navGrid('#pager', {
            search: false, edit: false, add: false, del: false, refresh: true
        }, {}, {}, {},
        {
            closeOnEscape: true, closeAfterSearch: true, ignoreCase: true, multipleSearch: false, multipleGroup: false, showQuery: false,
            sopt: ['cn', 'eq', 'ne'],
            defaultSearch: 'cn'
        }).navButtonAdd('#pager', {
            caption: "Export to Excel", buttonicon: "ui-icon-disk",
            onClickButton: function () {
                //ExportDataToExcel(grid, appId + "_GroupPermissionsReport.xlsx");
                ExportJQGridData(grid, appId + "_GroupPermissionsReport.xlsx");
            },
            position: "last"
        }).navButtonAdd('#pager', {
            caption: "Export to PDF",
            buttonicon: "ui-icon-disk",
            onClickButton: function () {
                ExportJQGridData(grid, appId + "_GroupPermissionsReport.pdf");
            },
            position: "last"
        });
    }

    function saveUser() {
        if (isUpdate)
            updateUser();
        else
            addnewUser();
    }

    function addnewUser() {
        var nUser = {
            User_Id: "", User_Name: $('#fldUsername').val(), Title: $('#fldTitle').val(),
            First_Name: $('#fldFirstname').val(), Last_Name: $('#fldLastname').val(),
            Email: $('#fldEmail').val(), Phone: $('#fldPhoneNumber').val(), Modified_by: $('#fldCurrentUser').text(), Create_Date: null, Last_Modified: null
        };
        nUser.User_Id = "";
        if (!userExists) {
            $.ajax({
                type: "get",
                contentType: "application/json",
                dataType: "html",
                data: nUser,
                url: Main.url + "addUser/",
                success: function (data) {
                    var usr = $("#userView");
                    if (usr) {
                        var searchBy = "User_Name";$("#userView").jqGrid("GridUnload");
                        Main.getFilteredUsers($('#fldUsername').val(), searchBy);
                    }
                    Main.cancel();
                    $('p.lblSuccessText').text("New user details added").fadeOut(10000);
                    userExists = false;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (textStatus !== null) {
                        //alert(errorThrown + "\n" + textStatus);
                        $('p.lblSuccessText').text("Error adding new user \\n" + errorThrown).fadeOut(5000);
                    }
                }
            });
        }
    }

    function updateUser() {
        var nUser = {
            User_Id: Main.selectUserId, User_Name: $('#fldUsername').val(), Title: $('#fldTitle').val(),
            First_Name: $('#fldFirstname').val(), Last_Name: $('#fldLastname').val(),
            Email: $('#fldEmail').val(), Phone: $('#fldPhoneNumber').val(), Modified_by: $('#fldCurrentUser').text(), Create_Date: null, Last_Modified: null
        };
        if (!userExists) {
            $.ajax({
                type: "get",
                dataType: "html",
                data: user,
                contentType: "application.json",
                url: Main.url + "addUser/",
                data: nUser,
                success: function (data) {
                    Main.selectedUserProfile = null;
                    var usr = $("#userView");
                    if (usr) {
                        //usr.resetSelection();
                        //usr.setGridParam('loadonce', false);
                        //usr.trigger('reloadGrid');
                        var searchBy = "User_Name";$("#userView").jqGrid("GridUnload");
                        Main.getFilteredUsers($('#fldUsername').val(), searchBy);
                    }
                    Main.cancel(); $('#divNewUser').hide();
                    $('p.lblSuccessText').text("User details updated").fadeOut(10000);
                    userExists = false;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (textStatus !== null) {
                        //alert(errorThrown + "\n" + textStatus);
                        $('p.lblSuccessText').text(errorThrown + "\\n" + "User details update failed").fadeOut(10000);
                    }
                }
            });
        }
        isUpdate = false;
    }

    var userExists = false;

    function checkIfExistUser() {
        var userName = $('#fldUsername').val();
        var validateObj = $('#fldUserValidation');
        validateObj.text("");
        if (userName != "") {
            $.ajax({
                type: "get",
                dataType: "text",
                contentType: "application.json",
                url: Main.url + "checkIfUserExists/" + userName,
                success: function (data) {
                    var d = parseInt(data);
                    validateObj.css("display", "inline-block");
                    validateObj.css("width", "150px");
                        validateObj.css("padding-left", "12px");
                    if (d == 1) {
                        if (!isUpdate || (isUpdate && Main.selectedUserProfile['User_Name'] != $('#fldUsername').val())) {
                            validateObj.css({ 'background-image': 'url(../Support/Content/Images/cancel.png)', 'background-repeat': 'no-repeat', 'background-position': '50 0' });
                            validateObj.text("Username exists");
                            validateObj.css("color", "red");
                            validateObj.show("medium");
                            $('#fldUsername').focus();
                            userExists = true;
                        }
                    }
                    else if (d == -1) {
                        validateObj.text("Error validating user").fadeOut(5000);
                        validateObj.show("fast");
                    }
                    else {
                        //validateObj.text("");
                        validateObj.css({ 'background-image': 'url(../Support/Content/Images/apply.png)', 'background-repeat': 'no-repeat', 'background-position': '0 0' });
                        validateObj.css("color", "green");
                        validateObj.text("User available");
                        validateObj.fadeOut(10000);
                        userExists = false;
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
    }

    function removeUser() {
        if (Main.selectUserId) {
            //return;
            if (confirm("This action will delete all the references to this user. Do you want to continue?")) {
                $.ajax({
                    type: "get",
                    dataType: "text",
                    url: Main.url + "removeUser/" + Main.selectUserId,
                    success: function (data) {
                        var usr = $("#userView");
                        var searchBy = "All";$("#userView").jqGrid("GridUnload");
                        Main.getFilteredUsers("", searchBy); Main.cancel();
                        $('p.lblSuccessText').text("User and related items successfully deleted").fadeOut(10000);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (textStatus !== null) {
                            //alert(errorThrown + "\n" + textStatus);
                            $('p.lblSuccessText').text(errorThrown + "\\n" + "There was an error. Failed to delete user").fadeOut(10000);
                        }
                    }
                });
            }
        }
        else
            alert("Select a user....");
        clearUserDetails();
    }

    function setApplicationEdit() {
        $('#divAppDetails').show();
        $('#fldAppId').val(selectedApp['application_id']);
        $('#fldAppName').val(selectedApp['application_name']);
        $('#fldAppUrl').val(selectedApp['application_uri']);
        $('#fldAppDescription').val(selectedApp['application_desc']);
        $("div#newApp td input[type=text]").val("");
        $('#newApp').hide();
        $('#btnnewApplication').show();
    }

    function saveAppDetails() {
        var appDetails = {
            application_id: $('#fldAppId').val(), application_name: $('#fldAppName').val(), application_uri: $('#fldAppUrl').val(),
            application_desc: $('#fldAppDescription').val(), catalog: selectedApp['catalog_id']
        };
        $.ajax({
            type: "get",
            contentType: "application/json",
            dataType: "html",
            data: appDetails,
            url: Main.url + "updateApplication/",
            success: function (data) {
                $('#divAppDetails').hide();
                var appId = $('#fldAppId').val();
                Main.getApplications(appId, "id");
                $('.lblSuccessText').text(appId + " updated successfully");
                $('.lblSuccessText').show().fadeOut(5000);
                $("input[type=text]").val("");
                
                //setTimeout(function () { location.reload(); }, 2000);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (textStatus !== null) {
                    $("#lblUserContextSuccess").val(errorThrown + "\n" + textStatus);
                }
            }
        });
    }

    function checkIfAppExists() {
        $.ajax({
            type: "get",
            dataType: "text",
            contentType: "application.json",
            url: Main.url + "checkIfAppExists/" + $('#fldappId').val(),
            success: function (data) {
                var d = parseInt(data);
                var validateObj = $('#lblAppId');
                if (d == 1) {
                    validateObj.text("** App exists");
                    validateObj.show("fast");
                    $('#fldappId').focus();
                    appExists = true;
                }
                else if (d == -1) {
                    validateObj.text("Error validating app").fadeOut(5000);
                    validateObj.show("fast");
                }
                else {
                    validateObj.text("");
                    validateObj.hide();
                    userExists = false;
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
            }
        });
    }

    function setUserPermsEditBox() {
        $('#fldUsername').val(appUserProfile['User_Name']);
        $('#fldFirstname').val(appUserProfile['First_Name']);
        $('#fldLastname').val(appUserProfile['Last_Name']);
        $('#fldPermission').val(appUserProfile['permission_Id']);
        $('#fldGrant').prop('checked', appUserProfile['grant'] === 'false' ? false : appUserProfile['grant'] === 'true' ? true : false);

        $('#divPermissions').show();
        //$('#divAddPermissions').hide();
    }

    function editSelectUserProfile(usrProfile) {
        $('#fldUsername').val(usrProfile['User_Name']);
        $('#fldFirstname').val(usrProfile['First_Name']);
        $('#fldLastname').val(usrProfile['Last_Name']);
        $('#fldPhoneNumber').val(usrProfile['Phone']);
        $('#fldEmail').val(usrProfile['Email']);
        $('#fldTitle').val(usrProfile['Title']);
        $('#btnDeleteUser').show();
        isUpdate = true;

        $('#addNewUser').val("Update User");
    }

    Main.cancel = function () {
        if (isUserEditClear)
            $('#userView').resetSelection();
        isUpdate = false;
        $('#divNewUser').hide(); $('#divNewGroup').hide(); $('#divPermissions').hide();
        $('#divAddPermissions').hide();
        $('#showAddUser > span').show();
        $('#bxNewGroup > span').show();
        $('#bxAddPermissions > span').show(); $('#btnDeleteUser').hide();
        $('#fldPermissionsUpdate').text("");
        $('#fldGroupUpdate').text("");
        $('p.lblSuccessText').text("");
        userExists = false;
        $('#fldUserValidation').text("");
        clearUserDetails();
    }

    function clearUserDetails() {
        $('#fldPermission').val("");
        $('#fldGrant').prop('checked', false);
        isUpdate = false;
        $('#btnDeleteUser').hide();
        Main.selectUserId = null;
        $("input[type=text]").val("");
        $('#userPermissions').resetSelection();
    }

    function showAddUser() {
        //if ($('#divNewUser').is('hidden'))
        $('p.lblSuccessText').text("");
        if (isUpdate) {
            if ($('#divNewUser').css('display') == 'none') {
                $('#divNewUser').show(); $('#divNewGroup').hide();
                $('#showAddUser > span').hide(); $('#bxNewGroup > span').show();
            }
        }
        else {
            if ($('#divNewUser').css('display') == 'none') {
                $('#divNewUser').show(); $('#divNewGroup').hide();
                $('#showAddUser > span').hide();
                $('#bxNewGroup > span').show();
            }
            else {
                $('#divNewUser').hide();
                $('#showAddUser > span').show();
            }
        }
    }

    function showAddUserGroup() {
        if ($('#divNewGroup').css('display') == 'none') {
            $('#divNewGroup').show(); $('#divNewUser').hide();
            $('#bxNewGroup > span').hide();
            $('#showAddUser > span').show();
        }
        else {
            $('#divNewGroup').hide();
            $('#bxNewGroup > span').show();
        }
    }

    function showAddUserPermissions() {
        if ($('#divAddPermissions').css('display') == 'none') {
            $('#divAddPermissions').show();
            $('#bxAddPermissions > span').hide();
        }
        else {
            $('#divAddPermissions').hide();
            $('#bxAddPermissions > span').show();
        }
    }

    function IsNullOrEmpty(str) {
        var l;
        if (str != null)
            l = str.length;
        if (l > 0 && typeof (str) == String) {
            if (str != "")
                return false;
            else
                return true;
        }
    }

    function User(field) {
        this.User_Name = ko.observable("");
        this.Title = ko.observable("");
        this.First_Name = ko.observable("");
        this.Last_Name = ko.observable("");
        this.Email = ko.observable("");
        this.Phone = ko.observable("");
        this.Create_date = null;
        this.Last_modified = null;
        this.Modified_by = ko.observable("");

        this.setField = function (field) {
            if (field == this.user_Name)
                return "user_Name";
            if (field == title)
                return "title";
            if (field == first_name)
                return "first_name";
            if (field == last_name)
                return "last_name";
            if (field == email)
                return "email";
        }
    }

})(window.Main = window.Main || {}, document, jQuery);