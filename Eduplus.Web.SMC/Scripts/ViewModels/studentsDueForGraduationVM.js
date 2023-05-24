student = function (data) {
    var self = this;

    self.Name = ko.observable(data.Name);
    self.RegNo = ko.observable(data.RegNo);
    self.CGPA = ko.observable(data.CGPA);
    self.Qualification = ko.observable(data.Qualification);
    self.MatricNumber = ko.observable(data.MatricNumber);
    self.Count = ko.observable(data.Count);
    self.Status=ko.observable(data.Status)
    self.Qualified = ko.observable(data.Qualified);
    self.Graduate = ko.observable(false);
    self.StudentId = ko.observable(data.StudentId);
};

viewModel = function () {
    var self = this;
    self.ProgrammeCode = ko.observable().extend({ required: true });
    self.SessionId = ko.observable().extend({ required: true });
    self.batch = ko.observable().extend({required:true});
    self.Kill = ko.observable(false);
    self.modelErrors = ko.validation.group(self);
    self.programmes = ko.observableArray([]);
    self.sessions = ko.observableArray([]);
    self.list = ko.observableArray([]);
    self.batches = ko.observableArray(['Batch A', 'Batch B', 'Batch C']);

    //Check if user is admin
    $.ajax({
        type: 'Get',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            
                LoadProgramme(data);

            }
        
    });

    self.fetch = function () {
        self.list([]);
        $.ajax({
            type: 'Get',
            data: { sessionId: self.SessionId().SessionId, programmeCode: self.ProgrammeCode() },
            url: '/Result/StudentsDueForGraduation',
            contentType: 'application/json; charset:utf-8',
            dataType: 'json',
            success: function (data) {

                if (data == null) {
                    swal({
                        title: "Students",
                        text: "No records found for choosen period",
                        type: "error"
                    });
                }
                ko.utils.arrayForEach(data, function (data) {
                    self.list.push(new student(data));

                });
            }
        });
    };

    //Populate Session
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/PopulateSession',
        success: function (data) {
            self.sessions(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }
    });

    //Populate Programmes LoadProgrammes
    LoadProgramme = function (val) {
        if (val == 1)
        {
            $.ajax({
                type: 'get',
                contentType: 'application/json; charset=utf-8',
                url: '/HelperService/LoadProgrammes',
                success: function (data) {
                    self.programmes(data)
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
        }
        else
        {
            $.ajax({
                type: 'get',
                contentType: 'application/json; charset=utf-8',
                url: '/HelperService/PopulateDeptProgrammes',
                success: function (data) {
                    self.programmes(data)
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
        }
        
    };

    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length > 0)
        {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if (isValid) {
            data = {
                students: self.list(),
                sessionId: self.SessionId().Title,
                batch: self.batch()
            };
            $.ajax({
                type: 'post',
                contentType: 'application/json; charset=utf-8',
                url: '/Result/GraduateStudent',
                data: ko.toJSON(data),
                success: function (data) {
                    alert(data);
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
        }
       
    }

    self.viewProfile = function (item) {
        window.open('/Result/ViewStudentProfile?regNo=' + item.MatricNumber());
    };
    
};

ko.applyBindings(new viewModel());