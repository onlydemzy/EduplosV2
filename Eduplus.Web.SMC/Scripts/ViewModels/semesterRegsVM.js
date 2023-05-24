
viewModel = function () {
    var self = this;


    self.programCode = ko.observable();
    self.session = ko.observable();
    self.semester = ko.observable().extend({ required: true });

    self.sessions = ko.observableArray();
    self.semesters = ko.observableArray();
    self.programmes = ko.observableArray();
    self.students = ko.observableArray();
    self.busy = ko.observable(false);
    self.adminUser = ko.observable();
    self.see = ko.observable(false);
    
    //Check if user is admin
    $.ajax({
        type: 'Get',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            PopulateProgsCourse(data);
        }
    });
    

    function PopulateProgsCourse(chk) {
        if (chk == 1 || chk == 2)//User is admin Populate Programe
        {
            //Populate Programme
            $.ajax({
                type: 'Get',
                contentyType: 'application/json;charset=utf-8',
                url: '/HelperService/LoadProgrammes',
                dataType: 'json',
                success: function (data) {
                    self.programmes(data);
                }
            });

        }
        else {
            //Populate Course

            $.ajax({
                type: 'Get',
                contentyType: 'application/json;charset=utf-8',
                url: '/HelperService/PopulateDeptProgrammes',
                dataType: 'json',
                success: function (data) {
                    self.programmes(data);
                }
            });
        }
    }

    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/LoadProgrammes',
        dataType: 'json',
        success: function (data) {
            self.programmes(data);
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
    self.session.subscribe(function (id) {
        self.semester(undefined);
        self.busy(true);
        $.ajax({
            type: 'Get',
            data: { sessionId: id },
            contentyType: 'application/json;charset=utf-8',
            url: '/HelperService/SemesterBySessionList',
            success: function (data) {
                self.semesters(data);
            },
            complete: function () {
                self.busy(false);
            }
        });

    });

    self.fetch = function () {
        if (self.semester() == undefined)
        {
            alert('Please choose a semester');
            return 0;
        }
        window.open('/AcademicAffairs/SemesterRegistrations?semesterId='+self.semester()+'&progCode='+self.programCode());
            
    };
    
}
ko.applyBindings(new viewModel());