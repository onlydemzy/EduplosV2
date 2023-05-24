//Fetch querystring
function getUrlParameter(name1) {
    name1 = name1.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name1 + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

var data;

var studentId = getUrlParameter('st');

//Fetch student


function viewModel() {
    var self = this;



    //fetch student data


    self.Surname = ko.observable("").extend({ required: { message: 'Please supply your surname' } });
    self.Firstname = ko.observable("").extend({ required: { message: 'Please supply your firstname' } });
    self.Middlename = ko.observable("");
    self.Title = ko.observable('').extend({ required: true });
    self.Sex = ko.observable('').extend({ required: true });
    self.ResidentialAddress = ko.observable('');
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' }, email: true });
    self.Phone = ko.observable("");
    self.NextKin = ko.observable("");
    self.KinAddress = ko.observable("");
    self.KinPhone = ko.observable("");
    self.KinMail = ko.observable("").extend({ email: true });
    self.Country = ko.observable('').extend({ required: { message: 'Please supply your country' } });
    self.State = ko.observable('').extend({ required: { message: 'Please supply your state' } });
    self.Lg = ko.observable('').extend({ required: { message: 'Please supply your lga' } });
    self.MaritalStatus = ko.observable('');
    self.DateOfBirth = ko.observable('');
    self.RefereeAddress = ko.observable('');
    self.Referee = ko.observable('');
    self.RefereePhone = ko.observable('');
    self.RefereeMail = ko.observable('');
    self.HomeTown = ko.observable();
    self.PermanentHomeAdd = ko.observable();
    self.SpouseName = ko.observable();
    self.SpouseAddress = ko.observable();
    self.EntryMode = ko.observable('').extend({ required: true });
    self.FacultyCode = ko.observable('').extend({ required: true });
    self.DepartmentCode = ko.observable("").extend({ required: true });
    self.ProgrammeType = ko.observable('').extend({ required: { message: 'Please supply programmeType' } });
    self.ProgrammeCode = ko.observable('');

    self.ProgrammeType = ko.observable('').extend({ required: { message: 'Please supply programmeType' } });;

    self.JambRegNumber = ko.observable('').extend({ required: true });
    self.MatricNumber = ko.observable('');
    self.YearAdmitted = ko.observable().extend({ required: true });

    self.Titles = ko.observableArray(['Mr', 'Mrs', 'Ms']);
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    self.Countries = ko.observableArray([]);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);
    self.Faculties = ko.observableArray([]);
    self.Departments = ko.observableArray([]);
    self.Programmes = ko.observableArray([]);
    self.Sexes = ko.observableArray(['Male', 'Female']);
    self.EntryModes = ko.observableArray(['PUME', 'Direct Entry', 'Transfer'])
    self.Types = ko.observableArray();
    self.sessions = ko.observableArray([]);
    self.Kill = ko.observable(false);
    self.modelErrors = ko.validation.group(self);

    //=============================================Populate Controls======================================
    //Populate Controls
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/ProgrammeTypes',
        success: function (data) {
            self.Types(data);
        }
    });
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
    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
        }
    });

    $.ajax({
        type: 'Get',
        data: { studentId: studentId },
        contentType: 'Application/Json; charset=utf-8',
        url: '/Student/FetchStudentForEditing1',
        success: function (result) {
            self.Surname(result.Surname);
            self.Firstname(result.Firstname);
            self.Middlename(result.Middlename);
            self.Title(result.Title);
            self.FacultyCode(result.FacultyCode);
            self.DepartmentCode(result.DepartmentCode);
            self.ProgrammeType(result.ProgrammeType);
            self.ProgrammeCode(result.ProgrammeCode);
            self.Country(result.Country);
            self.State(result.State);
            self.Lg(result.Lg);
            self.Sex(result.Sex);
            self.Email(result.Email);
            self.Phone(result.Phone);
            self.ResidentialAddress(result.ResidentialAddress);
            self.MatricNumber(result.MatricNumber);
            self.JambRegNumber(result.JambRegNumber);
            self.EntryMode(result.EntryMode);
            self.MaritalStatus(result.MaritalStatus);
            self.YearAdmitted(result.YearAdmitted);
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }
    });

    //Populate Controls


   
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

    self.FacultyCode.subscribe(function (facultyCode) {
        self.DepartmentCode(undefined);
        $.ajax({
            type: 'get',
            data: { _facultyCode: self.FacultyCode() },
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



};
$(document).ready(function () {
ko.applyBindings(new viewModel());
})
    
