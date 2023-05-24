//global variables
$(document).ready(function () {
    $("#bim").prop("disabled", true);
})
ko.bindingHandlers.filePreview = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        var allBindings = allBindingsAccessor()
        if (!!FileReader && valueAccessor() && element.files.length) {
            var reader = new FileReader();
            reader.onload = function (event) {
                var dataUri = event.target.result
                allBindings.imagePreview(dataUri)
            }
            reader.onerror = function (e) {
                console.log("error", stuff)
            }
            reader.readAsDataURL(element.files[0])
        }
    }
};
$(document).ready(function () {
    $('#second-sitting').prop('disabled', true);

});
function JambResult() {
    var self = this;
    self.Subject = ko.observable();
    self.Score = ko.observable(0);

};
//Work on Image




function OlevelResult() {
    var self = this;
    self.Subject = ko.observable();
    self.Grade = ko.observable();
    self.SitAttempt = ko.observable();
    self.Grades = ko.observableArray(['A1', 'B2', 'B3', 'C4', 'C5', 'C6', 'D7', 'E8', 'F9']);
};
function applicationVM() {

    var self = this;

    self.Surname = ko.observable("").extend({ required: true });
    self.Othernames = ko.observable("").extend({ required: true });
    self.Title = ko.observable();
    self.Sex = ko.observable("").extend({ required: true });
    self.ResidentialAddress = ko.observable();
    self.Email = ko.observable("").extend({ required: true });
    self.Phone = ko.observable("").extend({ required: true });
    self.NextKin = ko.observable("").extend({ required: true });
    self.KinAddress = ko.observable("").extend({ required: true });
    self.KinPhone = ko.observable("").extend({ required: true });
    self.KinMail = ko.observable("").extend({ required: true });
    self.DepartmentCode = ko.observable("").extend({ required: true });
    self.BirthDay = ko.observable("").extend({ required: true });
    self.BirthMonth = ko.observable("").extend({ required: true });
    self.ProgrammeCode = ko.observable("");
    self.FacultyCode = ko.observable().extend({ required: true });
    self.StudyMode = ko.observable("").extend({ required: true });
    self.OlevelSit1Venue = ko.observable();
    self.Photo = ko.observable();
    self.OlevelSit1ExamNumber = ko.observable();
    self.OlevelSit1Year = ko.observable();
    self.OlevelSit1ExamType = ko.observable();
    self.OlevelSit2Venue = ko.observable();
    self.OlevelSit2ExamNumber = ko.observable();
    self.OlevelSit2Year = ko.observable();
    self.OlevelSit2ExamType = ko.observable();
    self.ReasonForTransfer = ko.observable();
    self.JambRegNumber = ko.observable();
    self.Country = ko.observable();
    self.State = ko.observable();
    self.LGA = ko.observable();
    self.MaritalStatus = ko.observable();
    self.JambResults = ko.observableArray([]);
    self.OlevelResults1 = ko.observableArray([]);
    self.OlevelResults2 = ko.observableArray([]);
    self.Titles = ko.observableArray(['Mr', 'Mrs', 'Ms']);
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    self.Faculties = ko.observableArray([]);
    self.Departments = ko.observableArray([]);
    self.Countries = ko.observableArray([]);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);
    self.ExamTypes = ko.observableArray(['WAEC', 'NECO', 'NABTEB']);
    self.Sexes = ko.observableArray(['Male', 'Female']);

    //====================================populate Controlls===================
    //Populate Faculty
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        url: '/addmissions/PopulateFaculty',
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
            url: '/Addmissions/PopulateDepartment',
            success: function (data) {
                self.Departments(data)
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
        url: '/addmissions/PopulateCountry',
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
            url: '/Addmissions/PopulateState',
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
        self.LGA(undefined);
        $.ajax({
            type: 'get',
            data: { _state: state },
            contentType: 'application/json; charset:utf-8',
            url: '/Addmissions/PopulateLga',
            success: function (data) {
                self.Lgs(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });


    //Olevel First Sitting Scores

    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));
    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));
    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));
    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));
    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));
    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));
    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));
    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));
    self.OlevelResults1.push(new OlevelResult().Subject("").Grade("").SitAttempt(1));

    //Olevel second Sitting Scores
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));
    self.OlevelResults2.push(new OlevelResult().Subject("").Grade("").SitAttempt(2));

    //Manipulate JambResult
    /*self.JambResults.push(new JambResult({ Subject: 'Use of English', Score: 0 }));
    self.JambResults.push(new JambResult({ Subject: '', Score: 0 }));
    self.JambResults.push(new JambResult({ Subject: '', Score: 0 }));
    self.JambResults.push(new JambResult({ Subject: '', Score: 0 }));*/

    self.JambResults.push(new JambResult().Subject('Use of English').Score(0));
    self.JambResults.push(new JambResult().Subject('Use of English').Score(0));
    self.JambResults.push(new JambResult().Subject('Use of English').Score(0));
    self.JambResults.push(new JambResult().Subject('Use of English').Score(0));
    
    //Display uploaded image
    self.loadfile = function (event) {
        var output = $.document.getElementById('#output');
        output.src = URL.createObjectURL(event.target.files[0]);
    };

    

    self.submit = function () {
        
        $.ajax({
            type: 'post',
            url: '/Addmissions/SubmitApplication',
            dataType: 'json',
            contentType: 'application/json; charset:utf-8',
            data: ko.toJSON(self),
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    };
};

$(document).ready(function () {
   
    ko.applyBindings(new applicationVM());
    
});