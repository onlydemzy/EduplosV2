
function userVM() {
    var self = this;

    self.PersonId = ko.observable().extend({ required: { message: 'Please supply User fullname' } });;
    self.UserName = ko.observable().extend({ required: { message: 'please supply username' },maxLength:80 });
    self.Password = ko.observable().extend({ required: { message: 'Please supply password' } });
    self.ConfirmPassword = ko.observable().extend({ required:{message:'Confirm password'},equal: { params: self.Password, message: 'Password do not match' }});
    self.DepartmentCode = ko.observable();
    self.ProgrammeCode = ko.observable();
    
    self.Role = ko.observable().extend({ required: true });
    
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' },email:true });
    
    self.Programmes = ko.observableArray([]);
    self.staff=ko.observableArray([]);
    self.Departments = ko.observableArray([]);
    self.Roles = ko.observableArray([]);
    self.selectedStaff = ko.observable();
    self.modelErrors = ko.validation.group(self);

    self.disableButton = ko.observable(false);

    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
            
        }
       
        if (isValid)
        {
            self.disableButton(true);
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Accounts/CreateUser',
                
                data: ko.toJSON(self),
                async:false,
                success: function (data) {
                    if (data === "Exist") {
                        swal({
                            title: "Error",
                            text: "User already exist in the database",
                            type: "error"
                        });
                        self.disableButton(false);
                    }
                    else {
                        swal({
                            title: "Success",
                            text: "Operation completed Successfully",
                        },
                        function () {
                            window.location.href = '/Home/Index';
                        });
                        
                    }
                    
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
        }
        
    };
   
    
    //=============================================Populate Controls======================================
    //Populate Controls
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        url: '/HelperService/PopulateDepartment',
        success: function (data) {
            self.Departments(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }

    });
    self.DepartmentCode.subscribe(function (departmentCode) {
        self.ProgrammeCode(undefined);
        $.ajax({
            type: 'get',
            data: { _departmentCode: departmentCode },
            contentType: 'application/json; charset=utf-8',
            url: '/HelperService/PopulateProgramme',
            success: function (data) {
                self.Programmes(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });
    //Populate Roles
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        url: '/HelperService/PopulateRoles',
        cache:false,
        success: function (data) {
            self.Roles(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }

    });
   

    //Populate Staff
    self.DepartmentCode.subscribe(function (departmentCode) {
        self.PersonId(undefined);
        $.ajax({
            type: 'get',
            data: { _departmentCode: departmentCode },
            contentType: 'application/json; charset=utf-8',
            url: '/HelperService/PopulateStaffUser',
            success: function (data) {
                self.staff(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });

};

$(document).ready(function () {

    
    ko.applyBindings(new userVM());
});

