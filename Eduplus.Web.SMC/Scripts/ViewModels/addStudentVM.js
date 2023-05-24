
function viewModel() {
    var self = this;

    self.Surname = ko.observable("").extend({ required: { message: 'Please supply your surname' } });
    self.Firstname = ko.observable("").extend({ required: { message: 'Please supply your firstname' } });
    self.Middlename = ko.observable("").extend({ required: { message: 'Please supply your middle name' } });
    self.Title = ko.observable().extend({ required: true });
    self.Sex = ko.observable().extend({ required: true });
    self.ResidentialAddress = ko.observable();
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' }, email: true });
    self.Phone = ko.observable("");
    self.NextKin = ko.observable("");
    self.KinAddress = ko.observable("");
    self.KinPhone = ko.observable("");
    self.KinMail = ko.observable("").extend({ email: true });
    self.Country = ko.observable().extend({ required: { message: 'Please supply your country' } });
    self.State = ko.observable().extend({ required: { message: 'Please supply your state' } });
    self.Lg = ko.observable().extend({ required: { message: 'Please supply your lga' } });
    self.MaritalStatus = ko.observable();
    self.BDay = ko.observable();
    self.BMonth = ko.observable();
    self.BYear = ko.observable();
    self.RefereeAddress = ko.observable();
    self.Referee = ko.observable();
    self.RefereePhone = ko.observable();
    self.RefereeMail = ko.observable();
    self.EntryMode = ko.observable().extend({ required: true });
    self.DepartmentCode = ko.observable("").extend({ required: true });
    self.ProgrammeType = ko.observable().extend({ required: { message: 'Please supply programmeType' } });;
    self.ProgrammeCode = ko.observable();
    self.Titles = ko.observableArray(['Mr', 'Mrs', 'Ms']);
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    self.FacultyCode = ko.observable().extend({ required: true });
    self.JambRegNumber = ko.observable().extend({ required: true });
    self.ProgrammeType = ko.observable().extend({ required: true });
    self.SessionId = ko.observable().extend({ required: true });
    self.CurrentLevel = ko.observable().extend({ required: true });

    self.Levels = ko.observableArray([100, 200, 300, 400]);
    self.Countries = ko.observableArray([]);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);
    self.Faculties = ko.observableArray([]);
    self.Departments = ko.observableArray([]);
    self.Programmes = ko.observableArray([]);
    self.Sexes = ko.observableArray(['Male', 'Female']);
    self.EntryModes = ko.observableArray(['PUME', 'Direct Entry', 'Transfer'])
    self.Types = ko.observableArray(['Degree', 'HND Conversion']);
    self.sessions = ko.observableArray([]);
    self.Duration = ko.observable().extend({ required: true });
    self.months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'];
    self.days = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];
    self.yrs = ko.observableArray();
    self.durations = ko.observableArray([1, 2, 3, 4, 5]);
    self.Kill = ko.observable(false);

    self.modelErrors = ko.validation.group(self);

    $.ajax({
        type: 'get',
        contentType: 'Application /Json; charset=utf8',
        url: '/HelperService/GenerateYrs',
        dataType: 'json',
        success: function (data) {
            self.yrs(data);
        }
    })

    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;

        }

        if (isValid) {
            var data = {
                student: self,
                sessionId: self.SessionId
            };
            self.Kill(true);
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Admission_Center/SubmitNewStudent',

                data: ko.toJSON(data),
                success: function (data) {

                    swal({
                        title: "Student Admissions",
                        text: data,
                        type: "success"
                    },
                        function () {
                            window.location.href = '/Admission_Center/AddStudent';
                        }

                    );


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
        url: '/HelperService/PopulateFaculty',
        success: function (data) {
            self.Faculties(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }

    });
    //Populate Department
    self.FacultyCode.subscribe(function (facultyCode) {
        self.DepartmentCode(undefined);
        $.ajax({
            type: 'get',
            data: { _facultyCode: facultyCode },
            contentType: 'application/json; charset:utf-8',
            url: '/HelperService/PopulateDepartment1',
            success: function (data) {
                self.Departments(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });

    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
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
        cache: false,
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
    ko.applyBindings(new viewModel());
});