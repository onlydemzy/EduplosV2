course = function (data) {
    var self = this;
    self.RegistrationId = ko.observable(data.RegistrationId);
    self.CourseId = ko.observable(data.CourseId);
    self.CourseCode = ko.observable(data.CourseCode);
    self.Title = ko.observable(data.Title);
    self.CreditHour = ko.observable(data.CreditHour);
    self.Level = ko.observable(data.Level);
    self.Type = ko.observable(data.Type);
    self.vcourse = ko.observable(data.CourseCode + '-' + data.Title);
    self.StudentId = ko.observable(data.StudentId);
    self.SemesterId = ko.observable(data.SemesterId);
    self.SessionId = ko.observable(data.SessionId);
    self.ProgrammeCode = ko.observable(data.ProgrammeCode);

};
var viewModel=function(){
    var self=this;
    self.sessionId=ko.observable().extend({required:true});
    self.semesterId=ko.observable().extend({required:true});
    self.regNo=ko.observable().extend({required:true});
    self.regCourses = ko.observableArray();
    self.addedCourse = ko.observable();
    self.addCourses = ko.observableArray();
    self.studentId = ko.observable();
    self.lvl = ko.observable();
    self.progCode = ko.observable();
    self.data1 = ko.observableArray();
    self.tUnit = ko.observable(0);
    self.totalUnit = ko.pureComputed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.regCourses(), function (course) { total += parseInt("0" + course.CreditHour(), 10); })
        self.tUnit(total);
        return total;
    });
    self.sessions = ko.observableArray();
    self.semesters = ko.observableArray();
    self.spinar = ko.observable(false);
    //Populate session
    $.ajax({
        type: 'Get',

        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
        }
    });
    self.sessionId.subscribe(function (id) {
        self.semesterId(undefined);
        self.spinar(true);
        $.ajax({
            type: 'Get',
            data: { sessionId: self.sessionId() },
            contentyType: 'application/json;charset=utf-8',
            url: '/HelperService/SemesterBySessionList',
            success: function (data) {
                self.semesters(data);
            }
        });
        self.spinar(false);
    });
    self.fetch = function () {
        self.regCourses([]);
        self.spinar(true);
        self.progCode(undefined);
        self.lvl(undefined);
        $.ajax({
            type: 'Get',
            data: { semesterId: self.semesterId,regNo:self.regNo },
            contentyType: 'application/json;charset=utf-8',
            url: '/AcademicAffairs/FetchRegistedCoursesByStudent',
            success: function (data) {
                if (data.length== 0)
                {
                    alert('No records found');
                    self.spinar(false);
                    return;
                }
                else {
                    ko.utils.arrayForEach(data, function (d) {
                        self.regCourses.push(new course(d));
                    });

                    self.progCode(self.regCourses()[0].ProgrammeCode);
                    self.lvl(self.regCourses()[0].Level);
                    self.studentId(self.regCourses()[0].StudentId);
                    GetAddedCourses(self.lvl(), self.progCode());
                    self.spinar(false);
                }
                
            }
        });
        function GetAddedCourses(lv, prog) {
            self.addCourses([]);
            $.ajax({
                type: 'Get',
                contentType: 'Application/Json; charset=utf-8',
                data: { lvl: lv, progCode: prog,semesterId:self.semesterId(),studentId:self.studentId()},
                url: '/Student/AdditionalCourses',
                success: function (result) {
                    ko.utils.arrayForEach(result, function (data) {
                        self.addCourses.push(new course(data));
                    });
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
            
        }
        }
        

    self.addCourse = function () {
        //var newItem = new course();
        self.addedCourse().ProgrammeCode(self.progCode);
        self.addedCourse().Level(self.lvl);
        self.addedCourse().StudentId(self.studentId());
 
        //self.regCourses.push(self.addedCourse());
        if (confirm('Sure to add '+self.addedCourse().CourseCode()+' '+self.addedCourse().Title()+'?')) {
            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/AcademicAffairs/AddCourseToRegistration',
                data: ko.toJSON(self.addedCourse()),
                success: function (message) {
                    if (message == "Ok") {
                        alert("Course successfully added");
                        self.regCourses.push(new course(self.addedCourse()));
                    }
                    else {
                        alert(message);
                    }
                },
                complete: function () {
                    self.spinar(false);

                }

            });
        }
        

    };
    self.removeCourse = function (course) {
        if(confirm('Sure to remove this course?')){
            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/AcademicAffairs/DeleteCourseFromRegistration',
                data: ko.toJSON(course),
                success: function (message) {
                    if(message=="Ok")
                    {
                        alert("Course successfully removed");
                        self.regCourses.remove(course);
                    }
                    else {
                        alert(message);
                    }
                },
                complete: function () {
                    self.spinar(false);

                }

            });
        }
    }

    self.deleteRegistration = function () {
        if (confirm('Sure to delete this course regisration?')) {
            self.data1.push(self.semesterId());
            self.data1.push(self.studentId());

            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/AcademicAffairs/DeleteCourseRegistration',
                data: ko.toJSON(self.data1),
                success: function (message) {
                    
                        alert(message);
                    
                },
                complete: function () {
                    self.spinar(false);

                }

            });
        }
    }
}
ko.applyBindings(new viewModel());