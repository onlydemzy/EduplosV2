var userole = function (data) {
    var self = this;
    self.UserId = ko.observable(data?data.UserId:'');
    self.UserName = ko.observable(data?data.UserName:'');
    self.FullName = ko.observable(data ? data.FullName : '');
    self.NRoleID = ko.observable(data ? data.NRoleID : '');
}
var viewModel = function () {
    var self = this;
    var kr;
    self.role = ko.observable();
    self.selected = ko.observable();
    self.roles = ko.observableArray();
    self.selectedItem = ko.observable();
    self.roleUsers = ko.observableArray();
    self.allUsers = ko.observableArray();
    self.nroleUser = ko.observable();
    self.nRole = ko.observable();
    self.saveUrl ='/Accounts/AddRole';
   
    $.ajax({
        type: 'get',
        url: '/Accounts/GetRoles',
        contentType: 'application/json; charset=utf-8',
        success: function (res) {
            self.roles(res)
        }
    });


    //Get all users
   $.ajax({
        type: 'get',
        url: '/Accounts/AllUsers',
        contentType: 'application/json; charset=utf-8',
        success: function (res) {
            self.allUsers(res)
        }
    });

    self.deleteRow = function (item) {
        $.ajax({
            type: 'post',
            data: ko.toJSON(item),
            url: '/Accounts/DeleteRole',
            contentType: 'application/json; charset=utf-8',
            success: function (res) {
                alert(res);
                self.roles.remove(item);
            }
        });
    }

    self.addRole = function () {
        var newRole = self.nRole;
        $.ajax({
            type: 'get',
            data: { newRole: self.nRole },
            url: '/Accounts/AddRole',
            contentType: 'application/json; charset=utf-8',
            success: function (res) {
                alert(res);
                 
            }
        });

    }

    self.getRoleUsers = function (role) {
        self.roleUsers([]);
        kr = role.RoleId
        self.selected(role.RoleName);
        $.ajax({
            type: 'get',
            url: '/Accounts/GetRoleUsers',
            data:{roleId:kr},
            contentType: 'application/json; charset=utf-8',
            success: function (res) {
                ko.utils.arrayForEach(res, function (data) {
                    self.roleUsers.push(new userole(data));
                });
            }
        });
    }

    self.addUserToRole = function () {
        self.roleUsers.push(self.nroleUser());
        self.nroleUser().NRoleID = kr;

        $.ajax({
            type: 'post',
            url: '/Accounts/AddUserToRole',
            contentType: 'Application/json; charset=utf-8',
            dataType: 'json',
            data: ko.toJSON(self.nroleUser),

            success: function (result) {
                alert(result);
            }
        });
    }
    self.removeUserFromRole = function (roleUser) {
        roleUser.NRoleID = kr;
       $.ajax({
            type: 'post',
            data: ko.toJSON(roleUser),
            url: '/Accounts/RemoveUserFromRole',
            contentType: 'application/json; charset=utf-8',
            success: function (res) {
                alert(res);

            }
       });
       self.roleUsers.remove(roleUser);
    }

    self.viewPermissions = function (item) {
        window.open('/Accounts/RolePermissions?roleId=' + item.RoleId+'&role='+item.RoleName);
    }
}
ko.applyBindings(new viewModel());