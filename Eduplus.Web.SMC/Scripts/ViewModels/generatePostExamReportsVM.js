//var currentLocation = window.location;
//alert(currentLocation);
var course = function (data) {
    var self = this;
    self.CourseId = ko.observable(data.CourseId);
    self.Title = ko.observable(data.CourseCode + '-' + data.Title);
    
}
viewModel = function () {
    var self = this;
     
    self.SessionId = ko.observable().extend({required:true});
    self.SemesterId = ko.observable().extend({ required: true });
    self.ProgramCode = ko.observable().extend({ required: true });
    self.courseId = ko.observable().extend({ required: true });
    self.modelError = ko.validation.group(self);
    self.spinar = ko.observable(false);
    self.sessions = ko.observableArray([]);
    self.semesters = ko.observableArray([]);
    self.programmes = ko.observableArray([]);
    self.courses = ko.observableArray();
    self.ResultTypes = ko.observableArray(['Broad Sheet', 'Individual Results','Semester Course Form For Filing', 'Students on Probation','Result Scores Sheet']);
    self.Type = ko.observable();

    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/ScoresEntryProgrammes',
        dataType: 'json',
        success: function (data) {
            self.programmes(data);
        }
    });

   

    //Populate Controllers
    //Populate Session
    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
        }
    });

    //Populate Semester
    self.SessionId.subscribe(function (sessionId) {
        self.SemesterId(undefined);

        $.ajax({
            type: 'Get',
            data: { sessionId: sessionId },
            contentyType: 'application/json;charset=utf-8',
            url: '/HelperService/SemesterBySessionList',
            success: function (data) {
                self.semesters(data);
            }
        });

    });

    self.ProgramCode.subscribe(function (code) {
        self.courses([]);
        self.spinar(true);

        $.ajax({
            type: 'Get',
            contentyType: 'application/json;charset=utf-8',
            data: { programmeCode: code },
            url: '/HelperService/CoursesForScoresEntry',
            dataType: 'json',
            success: function (data) {
                ko.utils.arrayForEach(data, function (c) {
                    self.courses.push(new course(c));
                })
                 
            },
            complete: function () {
                self.spinar(false);
            }
        });
    })
    
    //Fetch BroadSheet
    self.fetch = function () {
        var isvalid = true;
        if (self.modelError().length > 0)
        {
            self.modelErrors.showAllMessages();
            isvalid = false;
        }
        if (isvalid)
        {
            window.open('/AcademicAffairs/InputtedResultSheet?semesterId=' + self.SemesterId() + '&courseId=' + self.courseId());
        }
      
    };

};
ko.applyBindings(new viewModel());