function getUrlParameter(name1) {
    name1 = name1.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name1 + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

var data;

var roleId = getUrlParameter('roleId');
var role = getUrlParameter('role');

var permission = function (data) {
    var self = this;
    self.PermissionId = ko.observable(data ? data.PermissionId : '');
    self.Controller = ko.observable(data ? data.Controller : '');
    self.Action = ko.observable(data ? data.Action : '');
    self.Remove = ko.observable(false);
    self.RoleId = ko.observable(data ? data.RoleId : 0);
    self.Activity = ko.observable(data ? data.Activity : '');
    
}
var viewModel = function () {
    var self = this;
    self.permissions = ko.observableArray([]);
    self.RoleId = ko.observable(roleId);
    self.Role = ko.observable(role);
    self.npermission = ko.observable();
    self.allpermissions = ko.observableArray();
    //get permissions
    $.ajax({
        type: 'get',
        data: { roleId: roleId },
        url: '/Accounts/GetRolePermissions',
        contentType: 'Application/json; charset=utf-8',
        success: function (res) {
            ko.utils.arrayForEach(res, function (data) {
                self.permissions.push(new permission(data));
            });
        }
    });

    $.ajax({
        type: 'get',
        data: { roleId: 0 },
        url: '/Accounts/GetRolePermissions',
        contentType: 'Application/json; charset=utf-8',
        success: function (res) {
            ko.utils.arrayForEach(res, function (data) {
                self.allpermissions.push(new permission(data));
            });
        }
    });

    self.remove = function (perm) {
        self.permissions.remove(perm);
        $.ajax({
            type: 'post',
            url: '/Accounts/RemovePermissionFromRole',
            contentType: 'Application/json; charset=utf-8',
            dataType: 'json',
            data: ko.toJSON(perm),

            success: function (result) {
                alert("result");
            }
        });

    }
    self.newPermission = function () {
        self.npermission().RoleId=roleId;
       
        $.ajax({
            type: 'post',
            url: '/Accounts/AddPermissiontoRole',
            contentType: 'Application/json; charset=utf-8',
            dataType:'json',
            data: ko.toJSON(self.npermission),
            
            success: function (result) {
                alert(result);
            }
        });
        self.permissions.push(self.npermission())
    }
};
ko.applyBindings(new viewModel);