var viewModel = function () {
    var self = this;
    self.session = ko.observable();
    self.programme = ko.observable();
    self.sessions = ko.observableArray();
    self.progs = ko.observableArray();

    $.ajax({
        type: 'Get',
        url: '/HelperService/LoadProgrammes',
        dataType: 'json',
        success: function (data) {
            
                self.progs(data);
 
        }
    });

    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            
                self.sessions(data);
 
        }
    });

    self.gen = function () {
        var data1=[];
        data1.push(self.session());
        data1.push(self.programme())
        $.ajax({
            type: 'Post',
            contentType: 'application/json; charset=utf-8',
            url: '/Accounts/SubmitBulkCreateStudentUsers',
            data: ko.toJSON(data1),
            success: function (data) {
                alert(data);
            }
        });
    }
}
ko.applyBindings(new viewModel());