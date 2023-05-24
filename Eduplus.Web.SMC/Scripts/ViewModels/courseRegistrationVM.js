

$(document).ready(function () {

    $('#t1').hide();
    $('#lvl').hide();
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        data: { semesterId: 0 },
        url: '/Student/CheckIfQualifiedToRegister',
        success: function (msg) {
            if (msg.value != 0) {
                swal({
                    title: "Message Alert",
                    text: msg.message,
                    type: "error"
                },
                            function () {
                                window.location.href = '/Student/Index';
                            }

                        );
            }
            else {
                $('#lvl').show();
            }

        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }

    });


});
var allowedCreditHours;
$.ajax({
    type: 'get',
    url: '/HelperService/GetMaxCreditHours',
    dataType: 'json',
    success: function (result) {
        allowedCreditHours = result;
    },
    error: function (err) {
        alert(err.status + " : " + err.statusText);
    }

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
    self.vcourse = ko.observable(data.CourseCode + '-' + data.Title);
    self.IsOutStanding = ko.observable(data.IsOutStanding);
    self.JustAdded = ko.observable(false);


};
//Populate courses

//view model
viewModel = function () {

    var self = this;
    self.Kill = ko.observable(false);
    self.Levels = ko.observableArray([100, 200, 300, 400, 500, 600, 700, 800]);
    self.level = ko.observable();
    self.addedCourse = ko.observable();

    self.selectedItem = ko.observable();

    self.courses = ko.observableArray([]);
    self.additionalCourses = ko.observableArray([]);
    self.removedCourses = ko.observableArray();
    self.spinar = ko.observable(false);
    self.tUnit = ko.observable(0);
    self.totalUnit = ko.pureComputed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.courses(), function (course) { total += parseInt("0" + course.CreditHour(), 10); course.Level(self.level()) })
        self.tUnit(total);
        return total;
    });
    //Populate courses
    self.level.subscribe(function (lvl) {
        self.courses([]);
        self.Kill(true);

        $.ajax({
            type: 'get',
            data: { level: lvl },
            contentType: 'application/json; charset:utf-8',
            url: '/Student/CoursesToRegister',
            dataType: 'json',
            success: function (result) {
                ko.utils.arrayForEach(result, function (data) {
                    self.courses.push(new course(data));
                });
                workcourses = self.courses;
                self.spinar(false);
            },
            complete: function () {
                self.Kill(false);
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
        //}

    });



    self.removeCourse = function (course) {

        self.courses.remove(course);
        if (course.JustAdded() == false) {
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

        $.ajax({
            type: 'Get',
            contentType: 'Application/Json; charset=utf-8',
            data: { lvl: self.level() },
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
        self.Kill(true);
        //Check if list is empty
        if (self.tUnit() == 0 || self.level() == 0) {
            swal({
                title: "Course registration error",
                text: "Total credit units/level must be greater than 0",
                type: "error"
            });
            self.Kill(false);
        }
        else if (self.tUnit() > 28) {
            swal({
                title: "Course registration error",
                text: "Total credit units must not exceed " + allowedCreditHours,
                type: "error"
            });
            self.Kill(false);
        }
        else {
            //CheckIfExist(self.level());
            data = {
                RegCourses: self.courses(),
                RemovedCourses: self.removedCourses()
            };

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
                        window.location.href = '/Student/MySemesterRegistration?studentId=' + self.courses()[0].StudentId() + '&semesterId=' + self.courses()[0].SemesterId();//self.courses[0].Level();
                    });

                },

                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }

            });
        }
    };
};
ko.applyBindings(new viewModel());