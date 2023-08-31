$(document).ready(function () {
    $('#btnSkip').hide();
});
function step1VM() {
    var self = this;

    
    self.Surname = ko.observable("").extend({ required: { message: 'Please supply your surname' } });
    self.Firstname = ko.observable("").extend({ required: { message: 'Please supply your firstname' } });
    self.Middlename = ko.observable("");
    self.Title = ko.observable();
    self.StudentId = ko.observable();
    self.Sex = ko.observable().extend({ required: true });
    self.ResidentialAddress = ko.observable().extend({ required: true });
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' },email:true });
     
    self.Phone = ko.observable("").extend({
        required: true,
        pattern: {
            message: 'Invalid phone number',
            params: /^\d{11}$/
        }
    });
    self.NextKin = ko.observable("");
    self.KinAddress = ko.observable("");
    self.KinPhone = ko.observable("").extend({
        required: true,
        pattern: {
            message: 'Invalid phone number',
            params: /^\d{11}$/
        }
    });
    self.KinMail = ko.observable("").extend({ email: true });
    self.Country = ko.observable().extend({ required: { message: 'Please supply your country' } });
    self.State = ko.observable().extend({ required: { message: 'Please supply your state' } });
    self.Lg = ko.observable().extend({ required: { message: 'Please supply your lga' } });
    self.MaritalStatus = ko.observable().extend({required:true});
    self.BDay = ko.observable().extend({ required: true });
    self.BMonth = ko.observable().extend({ required: true });
    self.BYear = ko.observable().extend({ required: true });
    self.RefereeAddress = ko.observable().extend({ required: true });
    self.Referee = ko.observable().extend({ required: true });
    self.RefereePhone = ko.observable().extend({ required: true });;
    self.RefereeMail = ko.observable().extend({ required: true,email:true });
    self.EntryMode = ko.observable().extend({ required: true });
    self.Programme = ko.observable();
    self.Titles = ko.observableArray(['Mr', 'Mrs', 'Ms']);
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    self.Faculty = ko.observable();
    self.StudyMode = ko.observable().extend({ required: true });
    self.Duration = ko.observable().extend({ required: true });
    /*self.JambYear = ko.observable().extend({
        required: true,
        pattern: {
            message: 'Invalid JAMB Year inputted',
            params: /^[0-9]{4}$/
        }
    });//^[0-9]{4}$ */
    self.IsHandicapped = ko.observable().extend({ required: true });
    self.Countries = ko.observableArray([]);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);
    self.Department = ko.observable();
    self.Status = ko.observable();
    self.WhyUs = ko.observable();
    
   // self.JambRegNumber = ko.observable().extend({required:true});
    self.Sexes = ko.observableArray(['Male', 'Female']);
    self.EntryModes = ko.observableArray(['PUME', 'Direct Entry', 'Transfer'])
    self.months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'];
    self.days = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31];
    self.yrs = ko.observableArray();
    self.durations = ko.observableArray([1, 2, 3, 4, 5]);
    self.modelErrors = ko.validation.group(self);
    self.Departments = ko.observableArray();
    self.progTypes = ko.observableArray();
    self.Faculties = ko.observableArray();
    self.HomeTown=ko.observable();
    self.PermanentHomeAdd=ko.observable();
    self.SpouseName=ko.observable();
    self.SpouseAddress = ko.observable();
    self.ProgrammeType = ko.observable();
    self.completeS = ko.observable();
    self.AddmissionCompleteStage = ko.observable();
    self.Programmes = ko.observableArray();
    self.studymodes = ko.observableArray(["Full Time", "Part Time", "Sandwich"]);
    self.handies = ko.observableArray(["No", "Yes"])
    self.kill = ko.observable(false);
    self.hide = ko.observable(false);
    self.stat = ko.observable();

    //Useer stat
    $.ajax({
        type: 'get',
        contentType: 'Application /Json; charset=utf8',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            if (data == 1 || data == 2) {
                self.hide(false);
            }
            else { self.hide(true) };
        }
    });

    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/ProgrammeTypes',
        success: function (data) {
            self.progTypes(data);
        }
    });
   
    $.ajax({
        type: 'get',
        contentType: 'Application /Json; charset=utf8',
        url: '/HelperService/GenerateYrs',
        dataType: 'json',
        success: function (data) {
            self.yrs(data);
        }
    })

    $.ajax({
        type: 'get',
        url: '/HelperService/MaxAddmissionstage',
        dataType: 'json',
        success: function (data) {
            self.completeS(data);
        }
    })
    //Get Initial Inputted Values
    $.ajax({
        type:'get',
        contentType:'Application /Json; charset=utf8',
        url: '/Admission_Center/GetStudentStep1',
        dataType:'json',
        success: function (data) {
            self.Firstname(data.Firstname);
            self.Middlename(data.Middlename);
            self.Surname(data.Surname);
            self.Title(data.Title);
            self.Email(data.Email);
            self.Phone(data.Phone);
            self.StudentId(data.StudentId);
            self.Department(data.Department);
            self.ProgrammeType(data.ProgrammeType);
            self.Faculty(data.Faculty);
            self.Programme(data.Programme);
            self.ResidentialAddress(data.ResidentialAddress);
            self.Duration(data.Duration);
            self.EntryMode(data.EntryMode);
            self.Referee(data.Referee);
            self.RefereeAddress(data.RefereeAddress);
            self.RefereeMail(data.RefereeMail);
            self.RefereePhone(data.RefereePhone);
             
            
            self.StudyMode(data.StudyMode);
            self.WhyUs(data.WhyUs);
            self.NextKin(data.NextKin);
            self.KinAddress(data.KinAddress);
            self.KinPhone(data.KinPhone);
            self.KinMail(data.KinMail);
            self.BDay(data.BDay);
            self.BMonth(data.BMonth);
            self.BYear(data.BYear);
            self.MaritalStatus(data.MaritalStatus);
            self.Sex(data.Sex);
            self.IsHandicapped(data.IsHandicapped);
            self.HomeTown(data.HomeTown);
            self.PermanentHomeAdd(data.PermanentHomeAdd);
            self.SpouseName(data.SpouseName);
            self.SpouseAddress(data.SpouseAddress);
            self.Status(data.Status);
            if (data.AddmissionCompleteStage > 2) {
                self.AddmissionCompleteStage(data.AddmissionCompleteStage);
                $('#btnSkip').show();
            }
            

             
        }
    })
    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
            
        }
       
        if (isValid)
        {
            self.kill(true);
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Admission_Center/Submit_Addmissions_Step1',
                
                data: ko.toJSON(self),
                
                success: function (data) {
                    if (data != "00") {
                        swal({
                            title: "Error",
                            text: data,
                            type: "error"
                        },
                            function() {
                                window.location.reload();
                            }

                        );
                        
                        
                    }
                    else {
                        swal({
                            title: "Success",
                            text: "Profile successfully updated",
                            type:"success",
                        },
                        function () {
                            if (self.completeS() == self.AddmissionCompleteStage() && self.Status()=='Prospective')
                            {
                                window.open('/Admission_Center/Student_Application_Summary');
                                
                            }
                            else {
                                window.location.href = '/Admission_Center/UploadPassport';
                            }
                            
                        });
                        
                    }
                    
                },
                complete:function(){
                    self.kill(false);
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                    self.kill(false);
                }
            });
        }
        
    };
    self.skip = function () {
        window.location.href = '/Admission_Center/UploadPassport';
    }
    
    //=============================================Populate Controls======================================
 
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
    ko.applyBindings(new step1VM());
});