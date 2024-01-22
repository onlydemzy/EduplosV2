$(document).ready(function () {

    $('#t1').hide();
    
});
//Model


course = function (data) {
    var self = this;
    self.CourseId = ko.observable(data ? data.CourseId : '');
    self.CourseCode = ko.observable(data.CourseCode);
    self.Title = ko.observable(data.Title);
    self.CreditHour = ko.observable(data ? data.CreditHour : 0);
    self.Level = ko.observable(data.Level);
    self.Type = ko.observable(data.Type);
    self.StudentId = ko.observable(data.StudentId);
    self.SemesterId = ko.observable(data.SemesterId);
    self.SessionId = ko.observable(data.SessionId);
    self.ProgrammeCode = ko.observable(data.ProgrammeCode);

    self.IsOutStanding = ko.observable(data.IsOutStanding);
    self.JustAdded = ko.observable(false);

};
//Populate courses

//view model
viewModel = function () {
    
    var self = this;
    self.Kill = ko.observable(false);
    self.programme = ko.observable();
    self.level = ko.observable();
    self.addedCourse = ko.observable();
    
    self.selectedItem = ko.observable();

    self.studentId = ko.observable();
    self.students=ko.observableArray([]);
    self.Levels = ko.observableArray([100, 200, 300, 400, 500, 600, 700, 800]);
    self.courses = ko.observableArray([]);
    self.programmes = ko.observableArray([]);
    self.show = ko.observable(false);
    self.sessionId = ko.observable();
    self.semesterId = ko.observable();
    self.semesters=ko.observable();
    self.sessions=ko.observableArray([]);
    self.additionalCourses = ko.observableArray([]);
    self.removedCourses = ko.observableArray([]);
    self.spinar = ko.observable(false);
    self.tUnit = ko.observable(0)
    self.allowedCreditHours=ko.observable();
    self.totalUnit = ko.pureComputed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.courses(), function (course) { total += parseInt("0" + course.CreditHour(), 10) })
        self.tUnit(total);
        return total;
    });

    //Check if user is Admin
    $.ajax({
        type: 'Get',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            UserRole(data);
        }
    });

    //Check if user is admin
    function UserRole(chk) {
        if (chk == 1 )//User is admin Populate Programe
        {
            //Populate Programme
            self.show(true);
            $.ajax({
                type: 'Get',
                contentyType: 'application/json;charset=utf-8',
                url: '/HelperService/LoadProgrammes',
                dataType: 'json',
                success: function (data) {
                    self.programmes(data);
                    self.students([]);
                }
            });

        }
        else {
            //Populate Course
            self.show(false);
            self.programmes([]);
            //Populate Students
            PopulateStudent('');
        }
    };

    
    self.programme.subscribe(function (prog) {
        if (prog != undefined && self.show() == true) {
            PopulateStudent(prog);
        }
    });
    self.studentId.subscribe(function (id) {
        
        $.ajax({
            type: 'Get',
            data: { semesterId: self.semesterId(), studentId: id },
            contentyType: 'application/json;charset=utf-8',
            url: '/Student/CheckIfQualifiedToRegister',
            success: function (data) {
                if (data.value != 0) {
                    alert(data.message);
                    window.location.reload();
                }
            }
        });

        
        $.ajax({
            type: 'get',
            data:{studentId:id},
            url: '/HelperService/GetMaxCreditHours',
            dataType: 'json',
            success: function (result) {
                self.allowedCreditHours(result);
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
        self.spinar(false);
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
            data: { sessionId: self.sessionId() },
            contentyType: 'application/json;charset=utf-8',
            url: '/HelperService/SemesterBySessionList',
            success: function (data) {
                self.semesters(data);
            }
        });
        self.spinar(false);
    });

   
   
    //Populate courses
    self.fetchCourse = function () {
        self.courses([]);
        self.spinar(true);

        $.ajax({
            type: 'get',
            data: { level: self.level(), semesterId: self.semesterId(),studentId:self.studentId() },
            contentType: 'application/json; charset:utf-8',
            url: '/Student/CoursesToRegister',
            dataType: 'json',
            success: function (result) {
                ko.utils.arrayForEach(result, function (data) {
                    self.courses.push(new course(data));
                });
                workcourses = self.courses;
                
            },
            complete:function(){
                self.spinar(false);
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
    };
    
    self.removeCourse = function (course) {

        self.courses.remove(course);
        if (course.JustAdded() == false)
        {
            self.removedCourses.push(course);
        }
        
        
    }
    self.reset = function () {
        self.courses([]);
        self.removedCourses([]);
        //self.level("undefined");
        self.addedCourse();
    }

    self.addAdditionalCourses = function () {
        if (self.level() == "") {
            alert("Please select your current level");
            return 0;
        }
        $('#t1').show();
        self.addAdditionalCourses([]);
        $.ajax({
            type: 'Get',
            contentType: 'Application/Json; charset=utf-8',
            data: { lvl: self.level(),progCode:self.programme(),studentId:self.studentId(),semesterId:self.semesterId() },
            url: '/Student/AdditionalCourses',
            success: function (result) {
                ko.utils.arrayForEach(result, function (data) {
                    self.additionalCourses.push(new course(data));
                });
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    };


    //==========================================================================Populate Added course controlls
    
    self.addCourse = function () {
        //var newItem = new course();
        self.addedCourse().JustAdded(true);
        self.courses.push(self.addedCourse());


    };

    self.register = function () {
        
        //Check if list is empty
        if (self.tUnit() == 0 || self.level() == 0) {
            swal({
                title: "Course registration error",
                text: "Total credit units/level must be greater than 0",
                type: "error"
            });
            
        }
        else if (self.tUnit() > self.allowedCreditHours()) {
            swal({
                title: "Course registration error",
                text: "Total credit units must not exceed 24",
                type: "error"
            });
             
        }
        else {
            //CheckIfExist(self.level());
            data={
                RegCourses:self.courses(),
                RemovedCourses:self.removedCourses()
            };
            self.spinar(true);
            $.ajax({
                type: "Post",
                url: '/Student/SubmitRegistration',
                contentType: "application/json; charset=utf-8",
                datatype: 'json',

                data: ko.toJSON(data),
                traditional: true,
                success: function (data) {
                    swal({
                        title: "Course Registration",
                        text: data,
                        type: "success"
                    }, function () {
                        window.location.href = '/Student/MySemesterRegistration?studentId=' + self.studentId() + '&semesterId=' + self.semesterId();//self.courses[0].Level();
                    });

                },
                complete:function(){
                    self.kil(false);
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }

            });
        }
    };

    function PopulateStudent(progCode) {
        $.ajax({
            type: 'Get',
            data:{programmeCode:progCode},
            contentyType: 'application/json;charset=utf-8',
            url: '/Student/FetchStudentsForRegistration',
            dataType: 'json',
            success: function (data) {
                self.students(data);
            }
        });
    }
};
ko.applyBindings(new viewModel());