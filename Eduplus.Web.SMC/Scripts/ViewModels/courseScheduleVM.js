schedule = function () {
    self.CourseId = ko.observable();
    self.CourseCode = ko.observable();
    self.CourseTitle = ko.observable();
    self.Semester = ko.observable();
    self.Session = ko.observable();
    self.DepartmentCode = ko.observable();
    self.ProgrammeCode = ko.observable();
    self.LecturerId = ko.observable();
    self.SemesterId = ko.observable();
    self.LecturerName = ko.observable();
    self.Title = ko.observable();
   
}
viewModel = function () {
    var self = this;

    self.lecturer = ko.observable();
    self.course = ko.observable();
    self.semester = ko.observable();
    self.session = ko.observable();
    self.programme = ko.observable();
    self.department = ko.observable();
    self.IsVisible = ko.observable(false);
    self.level = ko.observable();
    self.dirty = ko.observable(false);
    self.schedule=ko.observable();
    self.departments = ko.observableArray();
    self.programmes = ko.observableArray();
    self.semesters = ko.observableArray();
    self.sessions = ko.observableArray();
    self.lecturers = ko.observableArray();
    self.courses = ko.observableArray();
    self.levels = ko.observableArray([100, 200, 300, 400, 500, 600]);
    self.schedules=ko.observableArray();

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
    self.session.subscribe(function (ses) {
        self.semesters(undefined);
        //self.spinar(true);
        
        $.ajax({
            type: 'Get',
            data: { sessionId: ses.SessionId },
            contentyType: 'application/json;charset=utf-8',
            url: '/HelperService/SemesterBySessionList',
            success: function (data) {
                self.semesters(data);
            }
        });
        //self.spinar(false);
    });

    //Populate department
    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/AcademicAffairs/DepartmentalProgrammes',
        dataType: 'json',
        success: function (data) {
            self.programmes(data);
        }
    });

    //Populate courses
    self.programme.subscribe(function (prog) {

        if (self.semester() == null || self.semester() == "undefined")
        {
            alert("Choose a semester first");
            return 0;
        }
        self.courses();

        $.ajax({
            type: 'Get',
            data: { semester: self.semester().Title,progCode: prog.ProgrammeCode },
            contentyType: 'application/json;charset=utf-8',
            url: '/AcademicAffairs/CoursesForSchedule',
            success: function (data) {
                self.courses(data);
            }
        });
        //Populate lecturers
        self.lecturers(undefined);
        $.ajax({
            type: 'Get',
            data: { deptCode: prog.DepartmentCode },
            contentyType: 'application/json;charset=utf-8',
            url: '/AcademicAffairs/GetDeptLecturers',
            dataType: 'json',
            success: function (data) {
                self.lecturers(data);
            }
        });
    });

     
    //Buton Controlls
    self.addschedule = function () {
        if (self.course() == null||self.courses().length==0)
        {
            alert("No course selected");
            return 0;
        }
        if (self.lecturer() == null||self.lecturers().length==0)
        {
            alert("No lecturer selected");
            return 0;
        }
        var sch = new schedule();
        sch.LecturerId = self.lecturer().PersonId;
        sch.LecturerName = self.lecturer().Name;
        sch.CourseId = self.course().CourseId;
        sch.CourseCode = self.course().CourseCode;
        sch.CourseTitle = self.course().Title;
        sch.Semester = self.semester().Title;
        sch.SemesterId = self.semester().SemesterId;
        sch.ProgrammeCode = self.programme().ProgrammeCode;
        sch.DepartmentCode = self.programme().DepartmentCode;
        sch.Title = self.course().CourseCode + " - " + self.lecturer().Name;
        //Add schedules
        //Check If schedule already added

        var dul = ko.utils.arrayFirst(self.schedules(), function (item) {
            return item.Title == sch.Title;
        });
        if (dul != null)
        {
            alert("Selected schedule already added");
            return 0;
        }
        self.schedules.push(sch);
        self.dirty(true);
        //send to server
        $.ajax({
            type: "Post",
            url: '/AcademicAffairs/SaveSchedule',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: ko.toJSON(sch),
            success: function (data) {
                if (data != "00") { alert(data); }
            },

            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
        //self.courses.remove(self.course());

    }
    self.removeschedule = function (item) {
        if (self.schedule() == null || self.schedule() == "undefined")
        {
            alert("Select schedule to remove");
            return 0;
        }
        self.schedules.remove(self.schedule());
        //Remove from course
        //self.courses.remove(self.course());

    }

    self.saveSchedule=function()
    {
        $.ajax({
            type: "Post",
            url: '/AcademicAffairs/SaveSchedule',
            contentType: "application/json; charset=utf-8",
            datatype: 'json',
            data: ko.toJSON(self.schedules),
            success: function (data) {
                swal({
                    title: "Course Schedule",
                    text: data,
                    type: "success"
                }, function () {
                    window.location.href = '/Home/Index';//self.courses[0].Level();
                });

            },

            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
    }

    self.course.subscribe(function (course) {
        
        //self.spinar(false);
    });
    
};
ko.applyBindings(new viewModel());