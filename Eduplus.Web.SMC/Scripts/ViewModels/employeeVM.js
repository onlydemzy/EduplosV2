
function employeeVM() {
    var self = this;

    self.Surname = ko.observable("").extend({ required: { message: 'Please supply your surname' }});
    self.Firstname = ko.observable("").extend({ required: { message: 'Please supply your firstname' } });
    self.Middlename = ko.observable("").extend({ required: { message: 'Please supply your middle name' } });
    self.Title = ko.observable().extend({ required: true });
    self.Sex = ko.observable().extend({ required: true });
    self.ResidentialAddress = ko.observable();
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' },email:true });
    self.Phone = ko.observable("");
    self.NextKin = ko.observable("");
    self.KinAddress = ko.observable("");
    self.KinPhone = ko.observable("");
    self.KinMail = ko.observable("").extend({ email: true });
    self.Country = ko.observable().extend({ required: { message: 'Please supply your country' } });
    self.State = ko.observable().extend({ required: { message: 'Please supply your state' } });
    self.Lg = ko.observable().extend({ required: { message: 'Please supply your lga' } });
    self.MaritalStatus = ko.observable();
    self.DateOfBirth = ko.observable();
    self.DateEmployeed = ko.observable();
    self.Category = ko.observable();
    self.Designation = ko.observable();
    self.Unit = ko.observable();
    self.Building = ko.observable();
    self.Floor = ko.observable();
    self.Room = ko.observable()
    self.RefereeAddress = ko.observable();
    self.Referee = ko.observable();
    self.RefereePhone = ko.observable();
    self.RefereeMail = ko.observable();
    self.IDType = ko.observable();
    self.ids = ko.observableArray(['Drivers License', 'NIN', 'International Passport']);
    self.IDNumber = ko.observable();
    self.DepartmentCode = ko.observable();
    self.ProgrammeCode = ko.observable();
    self.Designation=ko.observable();
    self.Titles = ko.observableArray(['Mr', 'Mrs', 'Ms','Dr.','Prof','Engr']);
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    
    self.Countries = ko.observableArray([]);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);
    self.Programmes = ko.observableArray([]);
    self.Departments = ko.observableArray([]);
    
    self.Sexes = ko.observableArray(['Male', 'Female']);
    
    self.modelErrors = ko.validation.group(self);

    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
            
        }
       
        if (isValid)
        {
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/HRM/CreateStaff',
                
                data: ko.toJSON(self),
               
                success: function (data) {
                    if (data === "1") {
                        swal({
                            title: "Error",
                            text: "Something went wrong, try again",
                            type: "error"
                        },
                            function() {
                                window.location.href = '/HRM/Employee';
                            }

                        );
                        
                        
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
    //Populate Countries
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        url: '/HelperService/PopulateCountry',
        cache:false,
        success: function (data) {
            self.Countries(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }

    });
    //Populate State
    self.Country.subscribe(function (country) {
        self.State(undefined);
        $.ajax({
            type: 'get',
            data: { _country: country },
            contentType: 'application/json; charset:utf-8',
            url: '/HelperService/PopulateState',
            cache: false,
            success: function (data) {
                self.States(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });

    //Populate Lgs
    self.State.subscribe(function (state) {
        self.Lg(undefined);
        $.ajax({
            type: 'get',
            data: { _state: state },
            contentType: 'application/json; charset:utf-8',
            url: '/HelperService/PopulateLga',
            cache: false,
            success: function (data) {
                self.Lgs(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });
};
$(document).ready(function () {
    ko.applyBindings(new employeeVM());
});