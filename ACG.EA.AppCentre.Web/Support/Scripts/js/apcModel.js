var ApplicationsModel = function () {
    var self = this;

    self.id = ko.observable("");
    self.name = ko.observable("");
    self.uri = ko.observable("");
    self.description = ko.observable("");
    self.selectedApp = ko.observableArray([]);
    self.permId = ko.observable("");
    self.applicationId = ko.observable("");
    self.permName = ko.observable("");
    self.targetId = ko.observable("");
    self.selectedGroup = ko.observable("");
    self.appPermissions = ko.observableArray([]);
    self.grantPermission = ko.observable(true);
    self.appPermissions = ko.observableArray([]);
    self.searchTxt = ko.observable("");
    self.userGroups = ko.observableArray([]);
    self.userPermissions = ko.observableArray([]);
    self.adminApps = ko.observableArray([]);
    self.applicationTargets = ko.observableArray([]);
    self.appTargetValues = ko.observableArray();
    self.searchedUsersApps = ko.observableArray([]);
    self.appGroups = ko.observableArray([]);
    self.catalogApps = ko.observableArray([]);
    self.grpOwnPermissions = ko.observableArray([]);
    //to add a new application group to the store.    
    self.groupId = ko.observable("");
    self.groupName = ko.observable("");
    var isError = false;
    //self.setgrppermission = ko.observable(false);

    function permModel() {
        this.applicationId = "";
        this.groupId = "";
        this.permissions = [];
        this.grpPermId = [];
        this.isGranted = [];
    }

    $.ajax({
        url: Main.url + "getAppsByCatalog/" + $("#fldCurrentUser").text(),
        success: function (data) {
            self.catalogApps([]);
            $.each(data, function (key, value) {
                self.catalogApps.push(value);
            });
            setTimeout(function () {
                $('div.catMenu ul').hide();
                $('div.catMenu ul.Default').show();
                $('div.catMenu div span.Default').removeClass('ui-icon-triangle-1-e');
                $('div.catMenu div span.Default').addClass('ui-icon-triangle-1-se'); 
            }, 100);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus !== null) {
                //alert(errorThrown + "\n" + textStatus);
            }
        }
    });

    self.CancelAppEdit = function () {
        $("div#divAppDetails td input[type=text]").val("");
        $('#divAppDetails').hide();
        $('.lblSuccessText').text("");
    }

    self.CancelAddNewApp = function () {
        $("div#newApp td input[type=text]").val("");
        $('#newApp').hide();
        $('#btnnewApplication').show();
        $('.lblSuccessText').text("");
    }

    $.ajax({
        url: Main.url + "getadminApps/" + $("#fldCurrentUser").text(),
        success: function (data) {
            $.each(data, function (key, value) {
                self.adminApps.push(value);
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (textStatus !== null) {
                //alert(errorThrown + "\n" + textStatus);
            }
        }
    });

    self.searchUser = function () {
        var searchBy = $('#fldColumnSearch option:selected').val();
        Main.getFilteredUsers(self.searchTxt(), searchBy);
    }

    // add new application to AppCentre
    self.addNewApp = function (event) {
        var app = { application_id: self.id(), application_name: self.name(), application_uri: self.uri(), application_desc: self.description(), catalog_id: $('#fldCatalogId option:selected').val() };

        if (app.application_id == "") {
            $('#lblAppId').show();
            return;
        }
        if (app.application_name == "") {
            $('#lblAppName').show();
            return;
        }
//        if (app.application_uri == "") {
//            $('#lblAppUri').show();
//            return;
//        }
//        if (app.application_desc == "") {
//            $('#lblAppDesc').show();
//            return;
//        }
        $.ajax({
            dataType: "html",
            contentType: "application/json",
            type: "get",
            url: Main.url + "saveNewApplication/",
            data: app,
            success: function (data, textStatus, jqXHR) {
                jqXHR.statusCode();
                $('.lblSuccessText').text("New " + app.application_id + " application added successfully");
                $('.lblSuccessText').show().fadeOut(5000);
                $("input[type=text]").val("");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (textStatus !== null) {
                    alert(errorThrown + "\n" + textStatus);
                }
            }
        });
        $('#lblAppId').hide();
        $('#lblAppName').hide();
//        $('#lblAppUri').hide();
//        $('#lblAppDesc').hide();
//        $('#newApp').hide();
//        $('#btnnewApplication').show();
    }
        
    self.getUserGroups = function () {
        $('#fldGroupUpdate').text("");
        var app = $("#fldApplicationId option:selected").val();
        Main.permissionChecked = false;
        if (app) {
            $.ajax({
                dataType: "json",
                url: Main.url + "getUserAppGrp/" + self.selectUserId + "/" + app,
                success: function (data) {
                    if (data) {
                        bindUserGroupField(data, app);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (textStatus !== null) {
                        alert(errorThrown + "\n" + textStatus);
                    }
                }
            });
        }
    }

    self.getUserPermissions = function () {
        $('#fldPermissionsUpdate').text("");
        var app = $("#fldApplicationId option:selected").val();
        if (app) {
            $.ajax({
                dataType: "json",
                url: Main.url + "getUserPermissions/" + self.selectUserId + "/" + app,
                success: function (data) {
                    if (data) {
                        self.userPermissions([]);
                        $.each(data, function (key, value) {
                            self.userPermissions.push(value);
                        });
                        hideOrshowPermRow();
                        $('#bxAddPermissions').hide();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (textStatus !== null) {
                        alert(errorThrown + "\n" + textStatus);
                    }
                }
            });
        }
    }

    self.updateUserGroups = function (event) {
        var group = { applicationId: "", userId: "", groups: [], usrAppGrpIds: [], isMember: [] };
        group.applicationId = self.applicationId();
        group.userId = self.selectUserId;
        var btnText = $('#btnSaveUserGroups').text();
        $('#btnSaveUserGroups').text("Updating user groups");
        $('input[type=submit], button').attr('disabled', 'disabled');
        if (event) {
            var ch = $('input.memberOf');
            var grpId;
            if (ch.length > 0) {
                for (var i = 0; i < ch.length; i++) {
                    group.groups.push($('span.fldUserGroups')[i].innerHTML);
                    grpId = $('span.fldUserGroupId')[i].innerHTML;
                    grpId = grpId === "" ? 0 : parseInt(grpId);
                    group.usrAppGrpIds.push(grpId);
                    group.isMember.push(ch[i].checked);
                }
            }
            $.ajax({
                dataType: "html",
                type: "get",
                contentType: "application/json",
                data: group,
                url: Main.url + "syncUserGroups",
                success: function (data, textStatus, jqXHR) {
                    jqXHR.statusCode();
                    //alert(textStatus + "\n" + jqXHR.getResponseHeader());
                    isError = false;
                    //$('#fldGroupUpdate').show().fadeOut(10000);
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    if (textStatus !== null) {
                        alert(errorThrown + "\n" + textStatus); isError = true;
                    }
                }
            });
            var updateTxt = "";
            if (!isError)
                updateTxt = self.applicationId() + " groups updated for " + self.selectUserName;
            else
                updateTxt = "Error updating " + self.applicationId() + " group permissions for " + self.selectUserName + ". Contact sys admin.";
            isError = false;
            setTimeout(function () {
                self.getUserGroups();
                self.getUserPermissions();
                $('#fldGroupUpdate').show().text(updateTxt).fadeOut(5000);
                $('#btnSaveUserGroups').text(btnText);
                $('input[type=submit], button').removeAttr('disabled');
            }, 1000);
        }
    }

    self.updateUserPermissions = function (event) {
        //var perm = { applicationId: "", userId: "", permissions: [], usrPermId: [], targets: [], isGranted: [] };
        var perm = { applicationId: "", userId: "", permissions: [], usrPermId: [], usrGrpPermId: [], targets: [], permState: [] };
        perm.applicationId = self.applicationId();
        perm.userId = self.selectUserId;
        //$('#btnSavePermission').attr('disabled', 'disabled');
        $('input[type=submit], button').attr('disabled', 'disabled');
        var btnText = $('#btnSavePermission').text();
        $('#btnSavePermission').text("Updating user permissions");
        if (event) {
            //var ch = $('input.hasPermssion');
            //var ch = $('div:visible.chkBxImage');
            var ch = $('div.chkBxImage');
            var usrPermId;
            var brk = 0, rmd = ch.length % 10;
            var sections = ch.length > 10 ? Math.floor(ch.length / 10) : 0;
            var track = 0;
            if (ch.length > 0) {
                for (var i = 0; i < ch.length; i++) {
                    if (ch[i].parentNode.nextElementSibling != undefined) {
                        perm.permissions.push(ch[i].parentNode.nextElementSibling.children[0].innerHTML);
                        usrPermId = ch[i].parentNode.nextElementSibling.children[2].innerHTML;
                        usrGrpPermId = ch[i].parentNode.nextElementSibling.children[3].innerHTML;
                    }
                    else {
                        perm.permissions.push(ch[i].parentNode.nextSibling.children[0].innerHTML);
                        usrPermId = ch[i].parentNode.nextSibling.children[2].innerHTML;
                        usrGrpPermId = ch[i].parentNode.nextSibling.children[3].innerHTML;;
                    }
                    usrPermId = usrPermId === "" ? 0 : parseInt(usrPermId);
                    perm.usrPermId.push(usrPermId);
                    usrGrpPermId = usrGrpPermId === "" ? 0 : parseInt(usrGrpPermId);
                    perm.usrGrpPermId.push(usrGrpPermId);
                    if (ch[i].style.backgroundImage.indexOf("chkbox3_on") != -1)
                        perm.permState.push("explicit");
                    if (ch[i].style.backgroundImage.indexOf("chkbox2_on") != -1)
                        perm.permState.push("inherited");
                    if (ch[i].style.backgroundImage.indexOf("chkbox1_off") != -1)
                        perm.permState.push("none");
                    perm.targets.push($('span.fldPermTargetValue')[i].innerHTML == undefined ? null : $('span.fldPermTargetValue')[i].innerHTML == "" ? 0 : parseInt($('span.fldPermTargetValue')[i].innerHTML));
                    if (ch.length > 10) {
                        brk++;
                        if (brk == 10) {
                            track++;
                            sendUserPermissionUpdate(perm);
                        }
                        else if (track >= sections) {
                            if (brk == rmd) {
                                sendUserPermissionUpdate(perm);
                            }
                        }
                        continue;
                    }
                    else {
                        continue;
                    }
                }
                sendUserPermissionUpdate(perm);
            }
            var updateTxt = "";
            if (!isError)
                updateTxt = self.applicationId() + " permissions updated for " + self.selectUserName;
            else
                updateTxt = "Error updating " + self.applicationId() + " permissions for " + self.selectUserName + ". Contact sys admin.";
            isError = false;
            setTimeout(function () {
                self.getUserPermissions();
                self.getUserGroups();
                $('#fldPermissionsUpdate').show().text(updateTxt).fadeOut(5000);
                $('#btnSavePermission').text(btnText);
                $('input[type=submit], button').removeAttr('disabled');
            }, 1000);
        }
    }

    function sendUserPermissionUpdate(perm) {
        $.ajax({
            dataType: "html",
            type: "get",
            data: perm,
            url: Main.url + "syncUserPermissions",
            success: function (data, textStatus, jqXHR) {
                jqXHR.statusCode();
                isError = false;
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (textStatus !== null) {
                    isError = true;
                }
            }
        });
        perm.permissions = [];
        perm.usrPermId = [];
        perm.isGranted = [];
        perm.targets = [];
        brk = 0;
    }
    
    self.getAppTargets = function () {
        $.ajax({
            url: Main.url + "getApplicationTargets/" + $('#fldApplicationId option:selected').val(),
            success: function (data) {
                self.applicationTargets([]);
                $.each(data, function (key, value) {
                    self.applicationTargets.push(value);
                });
            }
        });
    }

    self.getAppTargetValues = function () {
        $.ajax({
            url: Main.url + "getAppTargetValues/" + $('#fldApplicationId option:selected').val(),
            success: function (data) {
                self.appTargetValues([]);
                $.each(data, function (key, value) {
                    self.appTargetValues.push(value);
                });
            }
        });
    }

    self.showGroupDetails = function (event) {
        var appGroup = { applicationId: "", groupId: "" };
        $('#divGroups').hide();
        $('#divGroupDetails').show();
        appGroup.applicationId = self.applicationId();
        appGroup.groupId = event.group_Id;
        $.ajax({
            dataType: "json",
            contentType: "application/json",
            data: appGroup,
            url: Main.url + "getGrpPermissionsV1/",
            success: function (data) {
                if (data) {
                    self.grpOwnPermissions([]);
                    $.each(data, function (key, value) {
                        $('#selGrpHeader').text(self.applicationId() + " - " + event.groupDescription + " Group Details");
                        $('#selectedGrp').val(event.group_Id);
                        self.grpOwnPermissions.push(value);
                        //$('#btnBack').show();
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (textStatus !== null) {
                    alert(errorThrown + "\n" + textStatus);
                }
            }
        });

    }

    self.getAppGroupsAndPermissions = function () {        
        if ($('#fldApplicationId option:selected').val() != "") {
            self.getApplicationGroups();
            self.getAppTargets();
            self.getAppTargetValues();
            $('#divGroups').show();
            $('#divGroupDetails').hide();
            $('.newAppPermission').hide();
            $('.newPermissions').show();
        }
    }

    //retrieves the current groups for the selected application
    self.getApplicationGroups = function () {
        debugger;
        $.ajax({
            url: Main.url + "getApplicationGroups/" + $('#fldApplicationId option:selected').val(),
            success: function (data) {
                self.appGroups([]);
                $.each(data, function (key, value) {
                    self.appGroups.push(value);
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (textStatus !== null) {
                    alert(errorThrown + "\n" + textStatus);
                }
            }
        });
    }

    self.loadReportAppGroups = function () {
        $.ajax({
            url: Main.url + "getApplicationGroups/" + $('#fldApplicationId option:selected').val(),
            success: function (data) {
                self.appGroups([]);
                var all = { group_Id: "All Groups", groupDescription: "All Groups" };
                self.appGroups.push(all);
                $.each(data, function (key, value) {
                    self.appGroups.push(value);
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (textStatus !== null) {
                    //alert(errorThrown + "\n" + textStatus);
                }
            }
        });
    }

    self.showUserAppGroups = function () {
        var grp = $('#fldAppGroupRpt option:selected').val();
        var appId = self.applicationId();
        if (appId && grp) {
            Main.showAppUserGroupPermissions(appId, grp);
        }
        else
            alert("Select application and target to get users!");
    }

    self.showAppGroupPermissions = function () {
        var grp = $('#fldAppGroupRpt option:selected').val();
        var appId = $('#fldApplicationId option:selected').val();
        if (appId && grp) {
            Main.showAppGroupPermissions(appId, grp);
        }
        else
            alert("Select application and group!");
    }

    self.loadSearchedUserApps = function () {
        var sText = $('#fldSearchTxt').val();
        var sch = document.forms[0].fldSearchby;
        var txt = "";
        var i;
        for (i = 0; i < sch.length; i++) {
            if (sch[i].checked) {
                txt = sch[i].value;
            }
        }
        if (txt == "" && sText == "") {
            alert("please enter a search term")
            return;
        }
        $.ajax({
            dataType: "json",
            type: "get",
            url: Main.url + "searchUsersApplications/" + $("#fldCurrentUser").text() + "/" +
                txt + "/" + sText,
            success: function (data) {
                self.searchedUsersApps([]);
                $.each(data, function (key, value) {
                    self.searchedUsersApps.push(value);
                });
            }
        });
    }

    self.searchApps = function () {
        var srchBy = $('#fldColumnSearch option:selected').val();
        Main.getApplications(self.searchTxt(), srchBy);
    }

    self.updateApplicationGrp = function (event) {
        var where, action, gId = $('#selectedGrp').val();
        var btnText = $('#updateApplicationGrp').text();
        $('#updateApplicationGrp').text("Updating groups");
        $('input[type=submit], button').attr('disabled', 'disabled');
        if (event.updateApplicationGrp.arguments[1].currentTarget.id === "btnSaveGrpDetails") {
            var perm = { applicationId: "", groupId: "", permissions: [], grpPermId: [], isGranted: [], targets: [] };
            perm.applicationId = self.applicationId();
            perm.groupId = gId;
            if (event) {
                var ch = $('input.chkPermission');
                var grpId;
                var brk = 0, rmd = ch.length % 10;
                var sections = Math.floor(ch.length / 10);
                var track = 0;
                if (ch.length > 0) {
                    for (var i = 0; i < ch.length; i++) {
                        if (ch[i].nextElementSibling != undefined) {
                            perm.permissions.push(ch[i].nextElementSibling.innerHTML);
                            if (ch[i].nextElementSibling.nextElementSibling.nextElementSibling != undefined) {
                                grpId = ch[i].nextElementSibling.nextElementSibling.nextElementSibling.innerHTML;
                            }
                            perm.targets.push(ch[i].parentElement.nextElementSibling.children[1].innerHTML);
                        }
                        else {
                            perm.permissions.push(ch[i].parentElement.children[1].innerHTML);
                            grpId = ch[i].parentElement.children[3].innerHTML;
                            perm.targets.push(ch[i].parentElement.nextSibling.children[1].innerHTML);
                        }
                        grpId = grpId === "" ? 0 : parseInt(grpId);
                        perm.grpPermId.push(grpId);
                        perm.isGranted.push(ch[i].checked);
                        brk++;
                        if (brk == 10) {
                            track++;
                            $.ajax({
                                type: "get",
                                data: perm,
                                url: Main.url + "updateGroupPermissions",
                                success: function (data, textStatus, jqXHR) {
                                    jqXHR.statusCode();
                                    isError = false;
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    if (textStatus !== null) {
                                        isError = true;
                                    }
                                }
                            });
                            perm.permissions = [];
                            perm.grpPermId = [];
                            perm.isGranted = [];
                            perm.targets = [];
                            brk = 0;
                        }
                        else if (track >= sections) {
                            if (brk == rmd) {
                                $.ajax({
                                    type: "get",
                                    data: perm,
                                    url: Main.url + "updateGroupPermissions",
                                    success: function (data, textStatus, jqXHR) {
                                        jqXHR.statusCode();
                                    },
                                    error: function (jqXHR, textStatus, errorThrown) {
                                        if (textStatus !== null) {
                                            isError = true;
                                        }
                                    }
                                });
                            }
                        }
                    }
                }
                var updateTxt = "";
                if (!isError)
                    updateTxt = "Updated group, " + gId + ", permissions for " + self.applicationId();
                else
                    updateTxt = "Error updating permission group, " + gId + ", from " + self.applicationId() + ". Contact sys admin.";
                isError = false;
                setTimeout(function () {
                    $('#bxNewGroup').hide();
                    $('#btnNewGroup').show();
                    self.getAppGroupsAndPermissions();
                    $('.lblSuccessText').show().text(updateTxt).fadeOut(5000);
                    $('#updateApplicationGrp').text(btnText);
                    $('input[type=submit], button').removeAttr('disabled');
                }, 1000);
            }
        }
        else if (event.updateApplicationGrp.arguments[1].currentTarget.id === "btnDelGroup") {
            if (confirm("This action will remove all related items. Continue?")) {
                var appGroup = { applicationId: "", groupId: "" };
                appGroup.applicationId = self.applicationId();
                appGroup.groupId = gId;
                $.ajax({
                    dataType: "html",
                    type: "get",
                    data: appGroup,
                    url: Main.url + "removeAppGroup",
                    success: function (data, textStatus, jqXHR) {
                        jqXHR.statusCode();
                        $('.lblSuccessText').text("Removed group, " + gId + ", from " + self.applicationId());
                        $('.lblSuccessText').show().fadeOut(5000);
                        $('#bxNewGroup').hide();
                        $('#btnNewGroup').show();
                        self.getApplicationGroups(); $('#divGroups').show();
                        $('input[type=submit], button').removeAttr('disabled');
                        $('#divGroupDetails').hide();
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (textStatus !== null) {
                            alert(errorThrown + "\n" + textStatus + "\n" + jqXHR.getResponseHeader());
                            $('#bxNewGroup').hide();
                            $('#btnNewGroup').show();
                            self.getAppGroupsAndPermissions();
                            $('.lblSuccessText').text("Error removing group, " + gId + ", from " + self.applicationId());
                            $('.lblSuccessText').show().fadeOut(5000);
                        }
                    }
                });
            }
        }
    }

    self.addApplicationGrp = function () {
        //var t = $('#fldPermissionTarget option:selected').val() || self.targetId();
        //t = t == "" ? "Allstate" : t;
        var appGrp = { Application_Id: "", Group_Id: "", Group_Name: "", setPermission: false };

        if (!self.applicationId()) {
            alert("select an application above to continue!");
            return;
        }
        if (!self.groupId()) {
            alert("specify a group to add!");
            return;
        }
        appGrp.Application_Id = self.applicationId();
        appGrp.Group_Id = self.groupId();
        if (self.groupId().toLowerCase() == "admin")
            appGrp.setPermission = true;
        appGrp.Group_Name = self.groupName();
        //appGrp.setPermission = self.setgrppermission();

        $.ajax({
            dataType: "text",
            type: "get",
            contentType: "application/json",
            data: appGrp,
            url: Main.url + "createApplicationGroup",
            success: function (data) {
                self.getAppGroupsAndPermissions();
                $("input[type=text]").val("");
                $('.lblSuccessText').text("Added new group, " + self.groupId() + ", for " + self.applicationId());
                $('.lblSuccessText').show().fadeOut(5000);
                $('#bxNewGroup').hide();
                $('#btnNewGroup').show();
                self.getApplicationGroups();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (textStatus !== null) {
                    //alert(errorThrown + "\n" + textStatus);
                    $('.lblSuccessText').text("There was an error adding a new group for " + self.applicationId());
                    $('.lblSuccessText').show().fadeOut(5000);
                    $('#bxNewGroup').hide();
                    $('#btnNewGroup').show();
                    self.getAppGroupsAndPermissions();
                }
            }
        });
    }

    self.clearSearch = function () {
        $("#tblApplications").jqGrid("GridUnload");
        $('.lblSuccessText').text("");
    }

    self.clearSearchUser = function () {
        if ($("#userView").jqGrid() !== undefined) {
            $("#userView").jqGrid("GridUnload");
        }
        if ($("#userPermissions").jqGrid() !== undefined) {
            $("#userPermissions").jqGrid("GridUnload");
        }
        //$("input[type=text]").val("");
        Main.cancel();
    }

    function hideOrshowPermRow() {
        var j = 0, perms = self.userPermissions();
        for (var i = 0; i < perms.length; i++) {
            if (perms[i].grant == 'Group' && perms[i].groupEnabled) {
                j++;
            }
        }
        if (j == perms.length) {
            $('#divPermissions').hide();
        }
        else
            $('#divPermissions').show();
    }

    function bindUserGroupField(data, app) {
        self.userGroups([]);
        $.each(data, function (key, value) {
            self.userGroups.push(value);
        });
        $('#divNewGroup').show();
        $('#bxNewGroup').hide();

    }

    self.searchUserPermission = function () {
        self.userGroups([]);
        self.userPermissions([]);
        self.selectUserId = null;
        $('#divPermissions').hide();
        $('#bxAddPermissions').show();
        $('#divNewGroup').hide();
        $('#bxNewGroup').show();
        $("#userPermissions").jqGrid("GridUnload");
        var searchBy = $('#fldColumnSearch option:selected').val();
        var schText = self.searchTxt();
        if (schText == "")
            schText = "all";
        $("#userPermissions").jqGrid({
            datatype: "json",
            loadonce: true,
            url: Main.url + "getAllAppUsers/" + searchBy + "/" + schText,
            colNames: Main.users.colNames,
            colModel: Main.users.colModel,
            height: "100%",
            caption: "Users Profile",
            postData: "",
            viewrecords: true,
            cursor: "pointer",
            pager: '#userViewEdit',
            ajaxGridOptions: { cache: false },
            onSelectRow: function () {
                self.selectUserId = $(this).jqGrid('getRowData', $(this).getGridParam('selrow'))['User_Id'];
                self.selectUserName = $(this).jqGrid('getRowData', $(this).getGridParam('selrow'))['User_Name'];
                if (self.selectUserId && self.applicationId()) {
                    self.getUserGroups();
                    self.getUserPermissions();
                }
                else {
                    alert("Select application and user!");
                    return;
                }
            },
            ondblClickRow: function () {
            }
        });
        $("#userPermissions").jqGrid('navGrid', '#userViewEdit', { edit: false, add: false, del: false, refresh: true, search: false }, {}, {}, {}, { multipleSearch: false, multipleGroup: false });
    }

    self.refreshUserDetails = function () {
        self.userGroups([]);
        self.userPermissions([]);
        if (self.selectUserId && self.applicationId()) {
            self.getUserPermissions();
            self.getUserGroups();
        }
    }

    self.saveAppDetails = function saveAppDetails() {
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
                //$.each(self.catalogApps(), function (key, value) {
                //    //self.catalogApps.push(value);
                //    $.each(value, function (k, val) {
                //        debugger;
                //        if (appDetails.application_id == val.application_name)
                //        {
                //            var app 
                //        }
                //        alert(val.application_name);
                //    });
                //});
                $.ajax({
                    url: Main.url + "getAppsByCatalog/" + $("#fldCurrentUser").text(),
                    success: function (data) {
                        self.catalogApps([]);
                        $.each(data, function (key, value) {
                            self.catalogApps.push(value);
                        });
                        setTimeout(function () {
                            $('div.catMenu ul').hide();
                            $('div.catMenu ul.Default').show();
                            $('div.catMenu div span.Default').removeClass('ui-icon-triangle-1-e');
                            $('div.catMenu div span.Default').addClass('ui-icon-triangle-1-se');
                        }, 100);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        if (textStatus !== null) {
                            //alert(errorThrown + "\n" + textStatus);
                        }
                    }
                });
            },
            error: function (jqXHR, textStatus, errorThrown) {
                if (textStatus !== null) {
                    $("#lblUserContextSuccess").val(errorThrown + "\n" + textStatus);
                }
            }
        });
    }
}