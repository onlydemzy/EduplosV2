var viewModel = function () {
    var self = this;
    self.session = ko.observable();
    self.dept = ko.observable();
    self.progType = ko.observable();
    self.modelErrors = ko.validation.group(self);
    self.depts = ko.observableArray();
    self.yrs = ko.observableArray();
    self.progTypes = ko.observableArray();
    self.rpt = ko.observable();
    self.rpts = ko.observableArray(['Excel', 'Pdf']);

    //fetching data
    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        contentType: 'Application/json;charset:utf-8',
        dataType: 'json',
        success: function (data) {
            self.yrs(data);
        }
    });

    $.ajax({
        type: 'Get',
        url: '/HelperService/ProgrammeTypes',
        contentType: 'Application/json; charset:utf-8',
        dataType: 'json',
        success: function (result) {
            self.progTypes(result);
        }
    });

    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateDepartment',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
                 self.depts(data);
 
        }
    });

    self.generate = function () {
        if (self.rpt() == 'Pdf')
        {
            window.open('/Student/GenerateMatricRegister?yrAdmitted=' + self.session() + '&progType=' + self.progType() + '&deptCode=' + self.dept());
        }
        else {
            window.open('/Student/GenerateMatricRegisterExcel?yrAdmitted=' + self.session() + '&progType=' + self.progType() + '&deptCode=' + self.dept());
        }
        
    }

};
ko.applyBindings(new viewModel);
