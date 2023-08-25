function getUrlParameter(name1) {
    name1 = name1.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name1 + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};

var data;

var studentId = getUrlParameter('studentId');

function displayele(value, element) {
    var ele;
    if (value == true) {
        ele = document.querySelector(element);
        ele.style.display = 'inline';
    }
    else {
        ele = document.querySelector(element);
        ele.style.display = 'none';
    }

}
displayele(false, ('#sp'));

function viewModel() {
    var self = this;
    //Initial Variables
    self.Surname = ko.observable("").extend({ required: { message: 'Please supply your surname' } });
    self.Firstname = ko.observable("").extend({ required: { message: 'Please supply your firstname' } });
    self.Middlename = ko.observable("");
    self.Title = ko.observable();
    self.StudentId = ko.observable();
    self.Sex = ko.observable().extend({ required: true });
    self.ResidentialAddress = ko.observable().extend({ required: true });
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' }, email: true });
    self.Phone = ko.observable("").extend({ required: true });
    self.NextKin = ko.observable("").extend({ required: true });
    self.KinAddress = ko.observable("").extend({ required: true });
    self.KinPhone = ko.observable("").extend({ required: true });
    self.KinMail = ko.observable("").extend({ email: true });
    self.Country = ko.observable().extend({ required: { message: 'Please supply your country' } });
    self.State = ko.observable().extend({ required: { message: 'Please supply your state' } });
    self.Lg = ko.observable().extend({ required: { message: 'Please supply your lga' } });
    self.MaritalStatus = ko.observable();
    self.BDay = ko.observable().extend({ required: true });
    self.BMonth = ko.observable().extend({ required: true });
    self.BYear = ko.observable().extend({ required: true });
    self.RefereeAddress = ko.observable().extend({ required: true });
    self.Referee = ko.observable().extend({ required: true });
    self.RefereePhone = ko.observable().extend({ required: true });;
    self.RefereeMail = ko.observable().extend({ required: true, email: true });
    self.EntryMode = ko.observable().extend({ required: true });
    self.Programme = ko.observable();
    self.ProgrammeCode = ko.observable();
    self.MatricNumber = ko.observable();
    self.Titles = ko.observableArray(['Mr', 'Mrs', 'Ms']);
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    self.Faculty = ko.observable();
    self.StudyMode = ko.observable().extend({ required: true });
    self.Duration = ko.observable().extend({ required: true });
    self.IsHandicapped = ko.observable().extend({ required: true });
    self.Countries = ko.observableArray([]);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);
    self.Department = ko.observable();
    self.YearAdmitted = ko.observable().extend({required:true});
    
    self.Sexes = ko.observableArray(['Male', 'Female']);
    self.EntryModes = ko.observableArray(['PUME', 'Direct Entry', 'Transfer'])
    self.months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'];
    self.days = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];
    self.yrs = ko.observableArray();
    self.durations = ko.observableArray([1, 2, 3, 4, 5]);
    self.sessions = ko.observable();
    self.Prog = ko.observable().extend({required:true});
    self.Status = ko.observable().extend({ required: true });
    self.statues = ko.observableArray(['Active','About to Graduate', 'Graduated','Probation','Prospective','Suspend']);
    self.CurrentLevel = ko.observable();
    self.lvls = ko.observableArray([0, 100, 200, 300, 400, 500]);
    self.progTypes = ko.observableArray();
    self.DepartmentCode = ko.observable();
    
    self.ProgrammeType = ko.observable().extend({required:true});
    self.progs = ko.observableArray();
    self.pTypes = ko.observableArray();
    self.studymodes = ko.observableArray(["Full Time", "Part Time", "Sandwich"]);
    self.handies = ko.observableArray(["No", "Yes"])
    self.kill = ko.observable(false);
    self.modelErrors = ko.validation.group(self);

    //Populate Controls

    $.ajax({
        type: 'get',
         
        url: '/HelperService/GenerateYrs',
        dataType: 'json',
        success: function (data) {
            self.yrs(data);
        }
    });

    $.ajax({
        type: 'Get',
         
        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
        }
    });


    
    $.ajax({
        type: 'get',
         
        dataType: 'json',
        url: '/HelperService/ProgrammeTypes',
        cache: false,
        success: function (data) {
            self.progTypes(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }

    });
    $.ajax({
        type: 'get',

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
   
    //Populate Initial Prog
   
    //Populate student
    $.ajax({
        type: 'Get',
        data: { studentId: studentId },
         
        url: '/Student/FetchStudentForEditing1',
        success: function (result) {
            self.StudentId(result.StudentId);
            self.Surname(result.Surname);
            self.Firstname(result.Firstname);
            self.Middlename(result.Middlename);
            self.Title(result.Title);
            self.Faculty(result.Faculty);
            self.Department(result.Department);
            self.ProgrammeType(result.ProgrammeType);
            self.Country(result.Country);
            self.State(result.State);
            self.Lg(result.Lg);
            self.Sex(result.Sex);
            self.Email(result.Email);
            self.Phone(result.Phone);
            self.ResidentialAddress(result.ResidentialAddress);
            self.MatricNumber(result.MatricNumber);
            self.EntryMode(result.EntryMode);
            self.MaritalStatus(result.MaritalStatus);
            self.YearAdmitted(result.YearAdmitted);
            self.NextKin(result.NextKin);
            self.KinPhone(result.KinPhone);
            self.KinMail(result.KinMail);
            self.KinAddress(result.KinAddress);
            self.Referee(result.Referee);
            self.RefereeMail(result.RefereeMail);
            self.RefereePhone(result.RefereePhone);
            self.RefereeAddress(result.RefereeAddress);
            self.Duration(result.Duration);
            self.CurrentLevel(result.CurrentLevel);
            self.IsHandicapped(result.IsHandicapped);
            self.ProgrammeCode(result.ProgrammeCode);
            self.DepartmentCode(result.DepartmentCode);
            self.BDay(result.BDay);
            self.BMonth(result.BMonth);
            self.BYear(result.BYear)
            self.StudyMode(result.StudyMode);
            self.JambRegNumber(result.JambRegNumber);
            self.JambYear(result.JambYear);
            self.ProgrammeType(result.ProgrammeType);
            self.Status(result.Status);
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }
    });

    $.ajax({
        type: 'get',
         
        url: '/HelperService/ProgrammeTypes',
        success: function (data) {
            self.pTypes(data);
        }
    });

    self.ProgrammeType.subscribe(function (typ) {
        displayele(true, '#sp');
        self.Prog(undefined);
        $.ajax({
            type: 'get',
            data: { programType: typ },
             
            url: '/HelperService/ProgrammesByType',
            success: function (data) {
                self.progs(data);
            },
            complete: function () {
                displayele(false, '#sp');
            }
        });
    })
    self.Prog.subscribe(function (prog) {
        if (prog != undefined) {
            self.Department(prog.Department);
            self.Faculty(prog.Faculty);
            self.ProgrammeCode(prog.ProgrammeCode);
            self.DepartmentCode(prog.DepartmentCode);
        }

    });
    self.Country.subscribe(function (country) {
        self.State(undefined);
        $.ajax({
            type: 'get',
            data: { _country: self.Country() },
             
            url: '/HelperService/PopulateState',
             
            success: function (data) {
                self.States(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });

    self.State.subscribe(function (state) {
        self.Lg(undefined);
        $.ajax({
            type: 'get',
            data: { _state: state },
            url: '/HelperService/PopulateLga',
            success: function (data) {
                self.Lgs(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });

    //After Update controlls
   
    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;

        }

        if (isValid) {
            
            self.kill(true);
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Student/SubmitStudentEdit',

                data: ko.toJSON(self),
                success: function (data) {

                    swal({
                        title: "Student Admissions",
                        text: data,
                        type: "success"
                    },
                        function () {
                            self.kill(false);
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
   // $('#matNumber').prop('disabled', true);
});