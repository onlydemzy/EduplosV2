viewModel = function () {
    var self = this;
    
    self.SessionId = ko.observable();
    self.SemesterId = ko.observable();
    self.sessions = ko.observableArray([]);
    self.semesters = ko.observableArray([]);
    
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


    //Fetch BroadSheet
    self.fetch = function () {
        if (self.SemesterId() == null) {
            alert("A required Field is missing. Please try again")
            return false;
        }

        $.ajax({
            type: 'Post',

            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                window.open('/Student/MyResult?semesterId=' +self.SemesterId());
            }
        });
    };

};
ko.applyBindings(new viewModel());