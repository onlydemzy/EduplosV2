var viewModel = function () {
    var self = this;
    self.session = ko.observable();
    self.dept = ko.observable();
    self.gender = ko.observable();
    self.sessions = ko.observableArray();
    self.depts = ko.observableArray();
    self.viewBy = ko.observable();
    self.genders = ko.observableArray(['Male','Female']);
    self.data1 = ko.observableArray();
    self.viewBys = ko.observableArray(['Department', 'Gender']);
    self.enrolments = ko.observableArray();
    self.modelErrors = ko.validation.group(self);
    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
        }
    });

    $.ajax({

        type: 'Get',
        url: '/HelperService/PopulateAcademicDepartment',
        contentType: 'application/json; charset:utf-8',
        
        success: function (data) {
            self.depts(data);
        }

    });

    self.fetch = function () {

        window.location.href = '/Admission_Center/StudentsEnrolmentRpt?session=' + self.session() + '&dept=' + self.dept() + '&sex=' + self.gender();

    }
    self.fetchSummary = function () {
        self.enrolments();
        $.ajax({

            type: 'Get',
            data: { session: self.session(), dept: self.dept(), sex: self.gender() },
            url: '/Admission_Center/StudentsEnrolmentSummary',
            contentType: 'application/json; charset:utf-8',
            dataType:'json',
            success: function (data) {
                self.enrolments(data);
            }

        });
    }
}
ko.applyBindings(new viewModel());