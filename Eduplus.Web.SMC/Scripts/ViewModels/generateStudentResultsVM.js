//var currentLocation = window.location;
//alert(currentLocation);

viewModel = function () {
    var self = this;
    self.IsAllowed = ko.observable(false);
    self.SessionId = ko.observable();
    self.SemesterId = ko.observable();
    self.Level = ko.observable();
    self.ProgramCode = ko.observable();
    self.Levels = ko.observableArray([100, 200, 300, 400, 500, 600, 700, 800, 900]);
    self.sessions = ko.observableArray([]);
    self.semesters = ko.observableArray([]);
    self.programmes = ko.observableArray([]);
    self.showC = ko.observable(false);
    self.ResultTypes = ko.observableArray(['Broad Sheet', 'Individual Results','Semester Course Form For Filing', 'Students on Probation','Result Scores Sheet']);
    self.Type = ko.observable();
    self.expTo = ko.observable();
    self.exports = ko.observableArray(['PDF', 'Excel']);

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
    self.Type.subscribe(function (r) {
        if (r == 'Broad Sheet') { self.showC(true); }
        else { self.showC(false);}
    });
    
    //Fetch BroadSheet
    self.fetch = function () {
        if (self.Type()==null||self.Type()==undefined) {
            alert("A required Field is missing. Please try again")
            return false;
        }
        
        switch(self.Type())
        {
            case 'Broad Sheet':
                if (self.expTo() == undefined) {
                    alert("Select the file format to export report to")
                    return;
                } else {
                    window.open('/Result/StudentBroadSheet?sessionId=' + self.SessionId() + '&semesterId=' + self.SemesterId()
                + '&level=' + self.Level() + '&progCode=' + self.ProgramCode() +'&rptType='+self.expTo());
                }
                
                break;

            case 'Individual Results':
                window.open('/Result/SemesterResultForFile?sessionId=' + self.SessionId() + '&semesterId=' + self.SemesterId()
                + '&level=' + self.Level() + '&progCode=' + self.ProgramCode());
                break; 
            case 'Semester Course Form For Filing':
                window.open('/Result/SemesterCourseFormForFile?sessionId=' + self.SessionId() + '&semesterId=' + self.SemesterId()
               + '&level=' + self.Level() + '&progCode=' + self.ProgramCode());
                break

            case 'Students on Probation':
                window.open('/Result/ProbationList?sessionId=' + self.SessionId() + '&progCode=' + self.ProgramCode());
                break;
            case 'Result Scores Sheet':
                window.open('/AcademicAffairs/InputtedResultSheet?&semesterId=' + self.SemesterId() + '&progCode=' + self.ProgramCode());
                break;
            default: ''
                break;
        }
       
    };

};
ko.applyBindings(new viewModel());