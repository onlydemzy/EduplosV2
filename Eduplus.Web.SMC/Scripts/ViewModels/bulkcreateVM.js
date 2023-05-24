var viewModel = function () {
    var self = this;
    self.faculty = ko.observable();
    self.faculties = ko.observableArray();
    self.dis = ko.observable(false);
    $.ajax({
        type: 'Get',
        url: '/HelperService/LoadFaculties',
         
        dataType: 'json',
        success: function (data) {
            self.faculties(data);
        }
    });
    self.create = function () {
        self.dis(true);
        $.ajax({
            type: 'Get',
            data:{faculty:self.faculty()},
            url: '/Accounts/SubmitCreateStudentUsers',
            contentType: 'application/json; charset:utf-8',
            dataType: 'json',
            success: function (data) {
                alert(data);
            },
            complete: function () {
                self.dis(false);
            }
        });
    }
}
ko.applyBindings(new viewModel());