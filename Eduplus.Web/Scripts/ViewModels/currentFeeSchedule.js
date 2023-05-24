var viewModel = function () {
    var self = this;
    self.facultyCode = ko.observable().extend({required:true});
    self.programmeType = ko.observable().extend({ required: true });
    self.faculties = ko.observableArray();
    self.types = ko.observableArray(['Degree', 'Post HND']);
    self.modelErrors = ko.validation.group(self);

    $.ajax({
        type: 'Get',
        url: '/Admissions/PopulateFaculty',
        contentType: 'application/json; charset:utf-8',

        dataType: 'json',
        success: function (data) {
            self.faculties(data);
        }
    });

    self.view = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;

        }
        if (isValid)
        {
           window.location.href = '/Admissions/CurrentFeeSchedules?faculty=' + self.facultyCode() + '&type=' + self.programmeType();
        }
        
    }
};

ko.applyBindings(new viewModel());