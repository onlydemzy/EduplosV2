viewModel = function () {
    var self = this;
    self.MatricNumber = ko.observable('');
    self.SessionId = ko.observable();
    self.SemesterId = ko.observable();
    self.sessions = ko.observableArray([]);
    self.semesters = ko.observableArray([]);
    self.Programs = ko.observableArray([]);
    self.resultType = ko.observable();
    self.visibleC = ko.observable(false);
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
    self.resultType.subscribe(function (val) {
        if (val == 'profile') {
            self.visibleC(false);
        }
        if (val == 'semester') {
            self.visibleC(true);
        }
    });
        
 
    //Fetch BroadSheet
    self.fetch = function () {
        if(self.MatricNumber()=='')
        {
            alert("Provide a matricnumber");
            return false;
        }

        if (self.resultType() == 'semester' && self.SemesterId() > 0) {

            $.ajax({
                type: 'Post',

                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    window.location.href = '/Result/StudentSemesterResult?semesterId=' + self.SemesterId() + '&matricNumber=' + self.MatricNumber();
                }
            });
           
        }
        

        if(self.resultType()=='profile')
        {
            $.ajax({
                type: 'Post',

                contentType: 'application/json; charset=utf-8',
                success: function (data) {
                    window.location.href = '/Result/ViewStudentProfile?regNo=' + self.MatricNumber();
                }
            });
        }


        
    };

    
};
ko.applyBindings(new viewModel());