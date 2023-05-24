var viewModel = function () {
    var self = this;
    self.sessionId = ko.observable();
    self.semesterId = ko.observable();
    self.matNo = ko.observable();
    self.sessions = ko.observableArray();
    self.semesters = ko.observableArray();
    self.kill = ko.observable(false);
    self.data1 = ko.observableArray([]);
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
    self.sessionId.subscribe(function (sessionId) {
        self.semesterId(undefined);

        $.ajax({
            type: 'Get',
            data: { sessionId: self.sessionId() },
            contentyType: 'application/json;charset=utf-8',
            url: '/HelperService/SemesterBySessionList',
            success: function (data) {
                self.semesters(data);
            }
        });

    });


    self.allow = function () {
        self.kill(true);
        self.data1.push(self.semesterId());
        self.data1.push(self.matNo());
       
        $.ajax({
            type: 'get',
            data:{semesterId:self.semesterId(),regNo:self.matNo},
            contentyType: 'application/json; charset=utf-8',
            url: '/AcademicAffairs/SubmitRegPermission',
            dataType: 'json',
            success: function (rep) {
                alert(rep);
            },
            complete: function () {
                self.kill(false);
            }

        })
    }
}
ko.applyBindings(new viewModel());