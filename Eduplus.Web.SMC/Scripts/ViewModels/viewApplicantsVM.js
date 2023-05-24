var viewModel = function () {
    var self = this;
    self.session = ko.observable();
    self.progType = ko.observable();

    self.sessions = ko.observableArray();
    self.progTypes = ko.observableArray();
    self.data1 = ko.observableArray();
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
        url: '/HelperService/ProgrammeTypes',
        contentType: 'application/json; charset:utf-8',
        
        success: function (data) {
            self.progTypes(data);
        }

    });

    self.fetch = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if (isValid) {
            self.data1.push(self.session());
            self.data1.push(self.progType());

            window.location.href = '/Admission_Center/ViewApplicantsRpt?session=' + self.session() + '&progType=' + self.progType();

        }
    }
}
ko.applyBindings(new viewModel());