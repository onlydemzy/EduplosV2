var grades;
var progTypeMax;
var course = function (data) {
    var self = this;
    self.CourseId = ko.observable(data ? data.CourseId : '');
    self.Course = ko.observable(data ? data.CourseCode + '-' + data.Title : '');
}
student = function (data) {
    var self = this;

    self.RegNo = ko.observable(data.RegNo);
    self.CA1 = ko.observable(0);
    self.CA2 = ko.observable(0);
    self.Exam = ko.observable(0);
    self.RegistrationId = ko.observable(data.RegistrationId);
    self.CourseId = ko.observable(data.CourseId);
    self.StudentId = ko.observable(data.StudentId);
    self.SemesterId = ko.observable(data.SemesterId);
    self.SessionId = ko.observable(data.SessionId);
    self.IsIR = ko.observable(false);
    self.TScore = ko.pureComputed(function () {
        if (self.CA1() !='-'||self.Exam()!='-' ||self.CA2()!='-')
        {
            var a = parseInt(self.CA1(), 10);
            var b = parseInt(self.CA2(), 10);
            var c = parseInt(self.Exam(), 10);
            if (a > progTypeMax.MaxCA1 || b > progTypeMax.MaxCA2|| c > progTypeMax.MaxExam) {

                return 'Error';
            } else
                return a + b + c;
        }
        else {
            return 0;
        }
       
    });

    self.Grade = ko.pureComputed(function () {

        if (self.CA1() == '-'||self.Exam()=='-'||self.CA2()=='-') {
            self.IsIR(true);
            return 'I';
        }
        else {
            var t = self.TScore();
            if (t!=NaN) {

                var gr = grades.find(function (g) {
                     
                    return (t >= g.Low && t <= g.High);
                });
                if (gr == undefined)
                {
                    return 'Invalid';
                }
                else {
                    return gr.Grade;
                }
                  
            }
            else {
                return 'Error';
            }
             
        }
        
    });

    self.modelError = ko.validation.group(self);

};

viewModel = function () {
    var self = this;
    self.SessionId = ko.observable();
    self.SemesterId = ko.observable();
     
    self.Programme = ko.observable();
    self.Course = ko.observable();
    self.Unit = ko.observable();
    self.CourseId = ko.observable();
    self.spinar = ko.observable(false);
    self.Kill = ko.observable(false);
     
    self.programmes = ko.observableArray([]);
    self.ca1 = ko.observable();
    self.ca2 = ko.observable();
    self.exam = ko.observable();
     
    self.students = ko.observableArray([]);
    self.sessions = ko.observableArray([]);
    self.semesters = ko.observableArray([]);
    self.courses = ko.observableArray([]);
    self.startIndex = ko.observable(0);
    self.display = ko.observable();
    self.displayIndex = ko.observable(1);
    self.listCount = ko.observable(0);
    self.selectedItem = ko.observable();
    self.inputtedScores = ko.observableArray([]);
    self.data1 = ko.observableArray([]);
    var count = 0;
    //Populate Programmes  

    $.ajax({
        type: 'Get',
        data:{action:'Write'},
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/ScoresEntryProgrammes',
        dataType: 'json',
        success: function (data) {
            if (data.length == 0)
            {
                alert('You are not authorised for this operation. Action cancelled');
                window.location.href='/Home/Index';
            }
            else {
                self.programmes(data);
            }
            
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
    self.SessionId.subscribe(function (sessionId) {
        self.SemesterId(undefined);
        self.spinar(true);
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

    //Populate Courses based on Programme
    self.Programme.subscribe(function (prog) {
        self.courses([]);
        grades = [];
        self.spinar(true);
       
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
        //Populate Max input values
        progTypeMax = null;
        $.ajax({
            type: 'Get',
            contentyType: 'application/json;charset=utf-8',
            data: { progType: prog.ProgrammeType },
            url: '/HelperService/ProgrammeTypeMax',
            dataType: 'json',
            success: function (data) {
                progTypeMax = data;
                self.ca1(data.MaxCA1);
                self.ca2(data.MaxCA2);
                self.exam(data.MaxExam);
            }
        });

        //populate courses

        $.ajax({
            type: 'Get',
            contentyType: 'application/json;charset=utf-8',
            data: { programmeCode: prog.ProgrammeCode },
            url: '/HelperService/CoursesForScoresEntry',
            dataType: 'json',
            success: function (data) {
                ko.utils.arrayForEach(data, function (d) {
                    self.courses.push(new course(d));
                });
            },
            complete: function () {
                self.spinar(false);
            }
        });
    });



    //Populate Registered Students
    self.CourseId.subscribe(function (corseId) {
        if (self.SessionId() == null || self.SemesterId() == null) {
            alert("Select a Session and or Semester first");
            return false;
        }
        self.students([]);
        self.inputtedScores([]);
        self.selectedItem();
        self.startIndex(0);
        self.displayIndex(1);
        self.Kill(true);
        self.spinar(true);
        $.ajax({
            type: 'Get',
            contentyType: 'application/json;charset=utf-8',
            data: { courseId: corseId, semesterId: self.SemesterId(), flag: 1 },
            url: '/Result/FetchStudentsForScoresEntry',
            dataType: 'json',
            success: function (data) {
                switch (data) {
                    case 0:
                        alert("No Course Registration record found for selected Semester and Course");

                        break;
                    case 1:
                        alert("Scores already entered for selected Semester and Course");

                        break;
                    default:
                        
                        ko.utils.arrayForEach(data, function (response) {
                            self.students.push(new student(response));
                        });
                        self.listCount(data.length);
                        self.selectedItem(self.students()[0]);
                        var dis = self.displayIndex() + ' of ' + self.listCount();
                        self.display(dis);
                        break;
                }
            },
            complete: function () {
                self.Kill(false);
                self.spinar(false);
            }
        });

    });

    //Button Controls
    

    //================================Pagination==============================================
    self.skip = function () {
        if (confirm('Student will fail this course if skipped. Sure to skip entering score for this student? ')) {
            var ad = self.selectedItem();
            
            ind++
            if (ind < self.listCount()) {
                self.selectedItem(self.students()[ind]);

                self.startIndex(ind);
                di++;

                self.displayIndex(di);
                var dis = di + ' of ' + self.listCount();
                self.display(dis);
            }
            else {
                alert('End of List reached');
                self.selectedItem();
                self.inputtedScores([]);
                self.students([]);
                self.CourseId(undefined);

            }
        }
    }
    
    self.next = function () {
        //check for error
        if (self.selectedItem().CA1() == 0 && self.selectedItem().CA2() == 0 && self.selectedItem().Exam() == 0) {
            if(confirm('Sure to submit without inputting scores? Student will fail this course'))
            {
                submitscore();
            }
            return 0;
        }
        if (self.selectedItem().Grade() == 'Invalid' || self.selectedItem().Grade() == 'Error')
        {
            alert('Invalid CA or Exam score inputted. Please confirm and try again');
            return 0;
        }
        else {
            submitscore();
        }
        //Move to next
       
        
    };
    submitscore = function () {
        var ind = self.startIndex();
        var di = self.displayIndex();

        //Save Score
        self.spinar(true);
        self.Kill(false);
        $.ajax({
            type: 'Post',
            url: '/Result/SubmitScore',
            contentType: 'application/Json;charset=utf-8',
            data: ko.toJSON(self.selectedItem()),
            datatype: 'json',
            success: function (data) {
                alert(data);

            },
            complete: function () {
                self.spinar(false);
                self.Kill(true);
                var ad = self.selectedItem();
                self.inputtedScores.push(ad);
                ind++
                if (ind < self.listCount()) {
                    self.selectedItem(self.students()[ind]);

                    self.startIndex(ind);
                    di++;

                    self.displayIndex(di);
                    var dis = di + ' of ' + self.listCount();
                    self.display(dis);
                }
                else {
                    alert('End of List reached');
                    self.selectedItem();
                    self.inputtedScores([]);
                    self.students([]);
                    self.CourseId(undefined);
                    self.startIndex(0);
                    self.displayIndex(1);
                    ind = self.startIndex();
                    di = self.displayIndex();

                }
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            },

        });
    }
    self.previous=function () {
        var ind = self.startIndex();
        var di = self.displayIndex();
        ind--;
        if (ind >=0) {
            self.selectedItem(self.students()[ind]);
            self.startIndex(ind);
            di--;
            self.displayIndex(di);
            var dis = di + ' of ' + self.listCount();
            self.display(dis);
        }
        else {
            alert('Start of List reached');
        }
    };
    
    //=========================================END=============================================
    //download template
    self.dwnLoad = function () {
        if (self.CourseId() == null || self.SemesterId() == null) {
            alert("Course or Semester missing");
            return;
        }
        else { window.location.href = '/Result/ScoresEntryTemplate?courseId=' + self.CourseId() + '&semesterId=' + self.SemesterId(); }
        
    }
};
ko.applyBindings(new viewModel());
