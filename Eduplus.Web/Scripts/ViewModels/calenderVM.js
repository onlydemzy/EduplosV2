var sessionsList;
//disable controls


calenderDetails = function () {
    var self = this;
    self.Activity = ko.observable('');
    self.Duration = ko.observable('');
    self.Semester = ko.observable('');
    
};

viewModel = function () {
    var self = this;
   
    self.Title = ko.observable();
    self.Details = ko.observableArray([]);
    
        $.ajax({
            type: 'get',
            contentType: 'application/json; charset:utf-8',
            url: '/Admissions/CurrentAcademicCalender',
            dataType:'json',
            success: function (data) {
                if (data == null) {
                    
                }
                else {
                    
                    self.Title(data.Title);
                    
                    self.Details(data.Details);
                    
                }
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });

};

ko.applyBindings(new viewModel());