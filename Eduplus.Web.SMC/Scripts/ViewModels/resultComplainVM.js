var detail = function (data) {
    
     
}

var viewModel = function () {
    var self = this;
    self.SessionId = ko.observable();
    self.SemesterId = ko.observable();
    self.StudentId = ko.observable();
    self.MatricNumber = ko.observable();
    self.LecturerId = ko.observable();
    self.Complain = ko.observable();
    self.OldCA1 = ko.observable();
    self.OldCA2 = ko.observable();
    self.NewCA1 = ko.observable();
    self.NewCA2 = ko.observable();
    self.OldExam = ko.observable();
    self.NewExam = ko.observable();
    self.Grade = ko.observable();
    self.StudentId = ko.observable();
    self.MatricNumber = ko.observable();
    self.GradePoint = ko.observable();
    self.RegistrationId = ko.observable();
    self.CourseId = ko.observable();
    self.sessions = ko.observableArray();
    self.semesters = ko.observableArray();
    self.Programme = ko.observable();
    self.programmes = ko.observableArray();
    self.courses = ko.observableArray();
    self.dept = ko.observable();
    self.depts = ko.observableArray();
    self.lecturer = ko.observable().extend({required:true});
    self.lecturers = ko.observableArray();
    self.Complain = ko.observable();
    //Populate Courses based on Programme
    self.Kill = ko.observable(false);
    self.spinar = ko.observable(false);
    self.show = ko.observable(false);
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
            //Populate depts/Programme
            self.show(true);
            $.ajax({
                type: 'Get',
                contentyType: 'application/json;charset=utf-8',
                url: '/HelperService/PopulateAcademicDepartment',
                dataType: 'json',
                success: function (data) {
                    self.depts(data);
                }
            });         

        }
        else {
            //Populate Course
            self.show(false);
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

    self.dept.subscribe(function (d) {
        $.ajax({
            type: 'Get',
            data:{deptCode:self.dept},
            contentyType: 'application/json;charset=utf-8',
            url: '/HelperService/PopulateDeptProgrammes',
            dataType: 'json',
            success: function (data) {
                self.programmes(data);
            }
        });
    });

    self.Programme.subscribe(function (prog) {
        self.courses([]);
        grades = [];
        $.ajax({
            type: 'Get',
            contentyType: 'application/json;charset=utf-8',
            data: { programmeCode: prog.ProgrammeCode },
            url: '/HelperService/CoursesByProgramme',
            dataType: 'json',
            success: function (data) {
                self.courses(data);
            }
        });
        //populate grades
        $.ajax({
            type: 'Get',
            contentyType: 'application/json;charset=utf-8',
            data: { progType: prog.ProgrammeType },
            url: '/HelperService/GetGradesByProgrammeType',
            dataType: 'json',
            success: function (data) {
                grades = data;
            }
        });

    });

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
        self.spinar(false);
    });
    //populate staff>>> This will work and needs be changed once course allocation is active
    self.CourseId.subscribe(function (cs) {
        self.lecturer(undefined);

        $.ajax({
            type: 'Get',
            data: { deptCode: sessionId },
            contentyType: 'application/json;charset=utf-8',
            url: '/ResultIssues/FetchCourseLecturer',
            success: function (data) {
                self.semesters(data);
            }
        });
        self.spinar(false);
    });
    self.fetchStudentScore = function () {
          
        $.ajax({
            type: 'Get',
            data:{semesterId:self.SemesterId(),matricNumber:self.MatricNumber,courseId:self.CourseId},
            contentyType: 'application/json;charset=utf-8',
            url: '/ResultIssues/FetchSingleScore',
            dataType: 'json',
            success: function (data) {
                self.OldCA1(data.CA1);  
                self.OldCA2(data.CA2);
                self.OldExam(data.Exam);
                self.Grade(data.Grade); 
                
            }
        });
    }
}
ko.applyBindings(new viewModel());