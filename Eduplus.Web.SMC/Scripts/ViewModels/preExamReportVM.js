//var currentLocation = window.location;
//alert(currentLocation);

viewModel = function () {
    var self = this;
    self.disabled = ko.observable(false);
    self.SessionId = ko.observable();
    self.SemesterId = ko.observable();
    self.ProgramCode = ko.observable();
    self.courseId = ko.observable();

    self.sessions = ko.observableArray([]);
    self.semesters = ko.observableArray([]);
    self.Programs = ko.observableArray([]);
    self.courses = ko.observableArray([]);

    self.reportTypes = ko.observableArray(['Attendance By Course','Attendance By Programme','Score Sheet']);
    self.Type = ko.observable();
   

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

    self.Type.subscribe(function () {
        if (self.Type == 'Attendance By Course') {
            self.disabled(true);
        }
        else { self.disabled(false); }
    });
    //Populate programes
    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/LoadProgrammes',
        dataType: 'json',
        success: function (data) {
            self.Programs(data);
        }
    });

    //Populate Courses
    self.ProgramCode.subscribe(function (programCode) {
        if (self.disabled() != true)
        {
            self.courseId(undefined);
            $.ajax({
                type: 'Get',
                contentyType: 'application/json;charset=utf-8',
                data:{programmeCode:programCode},
                url: '/HelperService/CoursesByProgramme',
                dataType: 'json',
                success: function (data) {
                    self.courses(data);
                }
            })
        }
        
    })

    //Fetch BroadSheet
    self.fetch = function () {
        if (self.SemesterId() == null || self.ProgramCode() == null||self.Type()==null) {
            alert("A required Field is missing. Please try again")
            return false;
        }
        
        switch(self.Type())
        {
            case 'Attendance By Course':
                window.open('/AcademicAffairs/ExamsAttendanceByCourse?semesterId=' + self.SemesterId() + '&courseId=' + self.courseId());
                break;
            case 'Attendance By Programme':
                window.open('/AcademicAffairs/ExamsAttendanceByProgramme?semesterId=' + self.SemesterId() + '&progCode=' + self.ProgramCode());
                break;
            case 'Score Sheet':
                window.open('/AcademicAffairs/ExamsScoreSheet?semesterId=' + self.SemesterId() + '&progCode=' + self.ProgramCode());
                break;
                    
        }
        
    };

};
ko.applyBindings(new viewModel());