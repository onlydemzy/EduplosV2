//var currentLocation = window.location;
//alert(currentLocation);

viewModel = function () {
    var self = this;
    self.IsAllowed = ko.observable(false);
    self.SessionId = ko.observable();
    self.SemesterId = ko.observable();
    self.gradYr=ko.observable();
    self.admitYr = ko.observable();
    self.Level = ko.observable();
    self.ProgramCode = ko.observable();
    self.Levels = ko.observableArray([100, 200, 300, 400, 500, 600, 700, 800, 900]);
    self.batches = ko.observableArray(['Batch A', 'Batch B', 'Batch C']);
    self.batch = ko.observable();
    self.sessions = ko.observableArray([]);
    self.semesters = ko.observableArray([]);
    self.Programs = ko.observableArray([]);
    
    $.ajax({
        type: 'Get',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            if (data == 0 || data == 2)
            { self.IsAllowed(false); }
            else {
                self.IsAllowed(true);
            }
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

    //Allowed n disallowed controlls
    
    //Fetch BroadSheet
    self.fetch = function () {
        
        if (self.ProgramCode() == null)
        { self.ProgramCode(''); }
        window.open('/Result/GraduatedStudentBroadSheet?sessionId=' + self.SessionId() + '&semesterId=' + self.SemesterId()
                + '&level=' + self.Level() + '&progCode=' + self.ProgramCode()+'&gradYr='+self.gradYr()+'&admitYr='+self.admitYr()+
            '&batch='+self.batch());
       
    };

};
ko.applyBindings(new viewModel());