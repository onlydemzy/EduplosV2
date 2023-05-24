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
    self.Title = ko.observable().extend({ required: true });
    self.StudentId = ko.observable();
    self.Sex = ko.observable().extend({ required: true });
    self.ResidentialAddress = ko.observable().extend({ required: true });
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' }, email: true });
    self.Phone = ko.observable("").extend({ required: true });
     
    self.Country = ko.observable().extend({ required: { message: 'Please supply your country' } });
    self.State = ko.observable().extend({ required: { message: 'Please supply your state' } });
    self.Lg = ko.observable().extend({ required: { message: 'Please supply your lga' } });
    self.MaritalStatus = ko.observable();
    self.BDay = ko.observable().extend({ required: true });
    self.BMonth = ko.observable().extend({ required: true });
    self.BYear = ko.observable().extend({ required: true });
     
    self.EntryMode = ko.observable().extend({ required: true });
    self.Programme = ko.observable();
    self.ProgrammeCode = ko.observable();
    self.MatricNumber = ko.observable().extend({ required: true });
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
    self.YearAdmitted = ko.observable();
    self.GradYear = ko.observable().extend({ required: true });
    self.Sexes = ko.observableArray(['Male', 'Female']);
    self.EntryModes = ko.observableArray(['PUME', 'Direct Entry', 'Transfer'])
    self.months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'];
    self.days = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];
    self.yrs = ko.observableArray();
    self.durations = ko.observableArray([1, 2, 3, 4, 5]);
    self.sessions = ko.observable();
    self.Prog = ko.observable();
    self.CurrentLevel = ko.observable();
    self.lvls = ko.observableArray([0, 100, 200, 300, 400, 500]);
    self.progTypes = ko.observableArray(["Degree"]);
    self.DepartmentCode = ko.observable();
    self.ProgrammeType = ko.observable();
    self.progs = ko.observableArray();
    self.pTypes = ko.observableArray();
    self.studymodes = ko.observableArray(["Full Time", "Part Time", "Sandwich"]);
    self.handies = ko.observableArray(["No", "Yes"])
    self.kill = ko.observable(false);

    self.Password = ko.observable().extend({ required: { message: 'Please supply password' } });
    self.ConfirmPassword = ko.observable().extend({ required: { message: 'Confirm password' }, equal: { params: self.Password, message: 'Password do not match' } });
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
    
        url: '/HelperService/PopulateCountry',
        dataType: 'json',
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
                url: '/Alumni/SubmitNewAlumnus',

                data: ko.toJSON(self),
                success: function (data) {
                    if (data == 1) {
                        swal({
                            title:"New Profile",
                            text:  "Profile successfully created, Login using your Matric Number (in caps) as username to continue with other processes. Check your e-mail for more details. Click Ok to Proceed...",
                            type: "success"
                        },
                         function () {
                             window.location.href = '/Accounts/Login';
                         }
                     );
                    }
                    if (data == 0) {
                        swal({
                            title: "Email address or Matricnumber already exist in the database",
                            text: data,
                            type: "success"
                        },
                         function () { return;}
                    );
                       
                    }

                },
                
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
        }

    };

    self.skip = function () {
        window.location.href = '/Student/EditProfile2';
    }
};

$(document).ready(function () {
    
    ko.applyBindings(new viewModel());
   // $('#matNumber').prop('disabled', true);
});