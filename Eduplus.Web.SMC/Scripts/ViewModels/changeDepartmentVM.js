function displayele(value, element) {
    var ele;
    if (value == true) {
        ele = document.querySelector(element);
        ele.style.display = 'inline';
    }
    else {
        ele = document.querySelector(element);
        ele.style.display = 'none';
    }

}
displayele(false, ('#sp'));

var viewModel = function () {
    var self = this;
    self.searchText = ko.observable();
    self.studentId = ko.observable();
    self.currentDept = ko.observable();
    self.name = ko.observable();
    self.matricNumber = ko.observable();
    self.status = ko.observable();
    self.currentProg = ko.observable();
    self.currentProgType = ko.observable();
    self.reason = ko.observable().extend({required:true});
    self.newProg = ko.observable().extend({ required: true });
    self.prog = ko.observable().extend({ required: true });
    self.modelErrors = ko.validation.group(self);
    self.newDept = ko.observable();
    self.faculty = ko.observable();
    self.kill = ko.observable(false);
    self.pTypes = ko.observableArray();
    self.progType = ko.observable();
    self.progs = ko.observableArray();
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/ProgrammeTypes',
        success: function (data) {
            self.pTypes(data);
        }
    });

    self.progType.subscribe(function (typ) {
        displayele(true, '#sp');
        self.newProg(undefined);
        $.ajax({
            type: 'get',
            data: { programType: typ },
            contentType: 'application/json; charset=utf-8',
            url: '/HelperService/ProgrammesByType',
            success: function (data) {
                self.progs(data);
            },
            complete: function () {
                displayele(false, '#sp');
            }
        });
    })
    self.prog.subscribe(function (prog) {
        if (prog != undefined) {
            self.newDept(prog.Department);
            self.faculty(prog.Faculty);
            self.newProg(prog.ProgrammeCode);
        }

    });
    self.fetchStudent = function () {
        self.kill(true);
        $.ajax({
            type: 'get',
            contentType: 'Application /Json; charset=utf8',
            data: { studentId: self.searchText() },
            url: '/Admission_Center/GetStudentStep1',
            dataType: 'json',
            success: function (data) {
                self.name(data.FullName);
                self.matricNumber(data.MatricNumber);

                self.studentId(data.StudentId);
                self.currentDept(data.Department);
                self.currentProgType(data.ProgrammeType);

                self.currentProg(data.Programme);

                self.status(data.Status);

            },
            complete: function () {
                self.kill(false);
            }
        })
    };
    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length > 0)
        {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if (isValid)
        {
            self.kill(true);
            var data1 = [];
            data1.push(self.studentId());
            data1.push(self.newProg());
            data1.push(self.reason());
            $.ajax({
                type: 'post',
                contentType: 'application/json; charset=utf-8',
                url: '/Student/SubmitChangeOfDepartment',
                data:ko.toJSON(data1),
                success: function (resp) {
                    alert(resp);
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                },
                complete: function () {
                    self.kill(false);
                }
            })
        }
        
    }
}

ko.applyBindings(new viewModel());