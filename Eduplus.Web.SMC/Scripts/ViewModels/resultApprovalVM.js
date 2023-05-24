var viewModel = function () {
    var self = this;
    self.semesterId = ko.observable();
    self.sessionId = ko.observable();
    self.prog = ko.observable();
    self.progType = ko.observable();
    self.apOption = ko.observable();
    self.lvl = ko.observable();
    self.semesters = ko.observableArray();
    self.sessions = ko.observableArray();
    self.progTypes = ko.observableArray();
    self.apOptions = ko.observableArray(['By Level', 'Entire Semester']);
    self.lvls = ko.observableArray([100, 200, 300, 400, 500, 600, 700]);
    self.progs = ko.observableArray();
    self.spinar = ko.observable(false);
    self.eC = ko.observable(false);
    $.ajax({
        type: 'Get',
        url: '/HelperService/ProgrammeTypes',
        dataType: 'json',
        success: function (data) {
            self.progTypes(data);
        }
    });

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
    self.sessionId.subscribe(function (sessionId) {
        self.semesterId(undefined);
        self.spinar(true);
        $.ajax({
            type: 'Get',
            data: { sessionId: sessionId},
            contentyType: 'application/json;charset=utf-8',
            url: '/HelperService/SemesterBySessionList',
            success: function (data) {
                self.semesters(data);
            }
        });
        self.spinar(false);
    });
    self.progType.subscribe(function (p) {
        
        //Populate grades
        $.ajax({
            type: 'Get',
            data: { programType: self.progType() },
            url: '/HelperService/ProgrammesByType',
            dataType: 'json',
            success: function (data) {
                self.progs(data);
            }
        });
    });

    self.apOption.subscribe(function (p) {
        if (p == 'By Level')
        {
            self.eC(true);
        }
        else {
            self.eC(false);
        }
  
    });

    self.approve = function () {
        var data1 = [];
        data1.push(self.semesterId());
        data1.push(self.prog());
        data1.push(self.apOption());
        var lv;
        
        if (self.apOption == "By Level"&&self.lvl==undefined)
        {
            alert("Select a level");
            return 0;
        }
        if (self.apOption() == "Entire Semester") { lv = 0; }
        else { lv = self.lvl();}
        data1.push(lv);
        $.ajax({
            type: 'Post',
            url: '/Result/SubmitResultApproval',
            contentType: 'application/Json;charset=utf-8',
            data: ko.toJSON(data1),
            datatype: 'json',
            success: function (data) {
                alert(data);
                 
            },
            complete: function () {
                self.spinar(false);
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    }

}
ko.applyBindings(new viewModel());