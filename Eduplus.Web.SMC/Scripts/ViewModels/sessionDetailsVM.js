
semester = function (data) {
    var self = this;
    self.SessionId = ko.observable(data ? data.SessionId : 0);
    self.SemesterId = ko.observable(data?data.SemesterId:0);
    self.Title = ko.observable(data?data.Title:'').extend({ required: true, maxLength: 100 });
    self.StartDate = ko.observable(data ? data.StartDate : '');
    self.EndDate = ko.observable(data ? data.EndDate : '')
    self.IsCurrent = ko.observable(data ? data.IsCurrent : false);
    self.ApplyLate1 = ko.observable(data ? data.ApplyLate1 : false);
    self.ApplyLate2 = ko.observable(data ? data.ApplyLate2 : false);
    self.IsCurrent = ko.observable(data ? data.IsCurrent : false);
    self.LateRegistration2StartDate=ko.observable(data?data.LateRegistration2StartDate:'');
    self.LateRegistration2EndDate = ko.observable(data ? data.LateRegistration2StartDate : '');
    self.semesters = ko.observableArray(['1st Semester', '2nd Semester', '3rd Semester']);
    self.modelErrors = ko.validation.group(self);



}
session = function () {

    var self = this;
    
    self.SessionId = ko.observable();
    self.Title = ko.observable();
    self.StartDate = ko.observable(); 
    
    self.EndDate = ko.observable();
    self.IsCurrent = ko.observable();
    
    self.selectedItem = ko.observable();
    self.saveUrl = '/Administration/SaveSemester';
    
    $.ajax({
        type: 'Get',
        url: '/Administration/SemesterList',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.semesterList.push(new session(data));

            });
        }
    });

};

viewModel = function () {
    var self = this;
    self.semesterList = ko.observableArray([]);
    self.SessionId = ko.observable();
    self.Title = ko.observable();
    self.StartDate = ko.observable();
    self.EndDate = ko.observable();
    self.IsCurrent = ko.observable();
    self.selectedItem = ko.observable();
    self.modelErrors=ko.validation.group(semester);
    self.saveUrl = '/Administration/SaveSemester';
    $.ajax({
        type: 'Get',
        url: '/Administration/SessionSemesters',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            self.SessionId(data.SessionId);
            self.Title(data.Title);
            self.StartDate(data.StartDate);
            self.EndDate(data.EndDate);
            self.IsCurrent(data.IsCurrent);

            ko.utils.arrayForEach(data.Semesters, function (data) {
                self.semesterList.push(new semester(data));

            });
        }
    });

    self.edit = function (item) {

        self.selectedItem(item);

    };

    self.cancel = function () {
        self.selectedItem(null);
    };

    self.add = function () {
        var newItem = new semester();
        self.semesterList.push(newItem);
        self.selectedItem(newItem);

    };



    self.save = function () {
        var isValid=true;

        var item = self.selectedItem();
        item.SessionId = self.SessionId();

        $.post(self.saveUrl, item, function (result) {
            alert(result);
            self.selectedItem(null);
        });

    };

    self.templateToUse = function (item) {
        return self.selectedItem() === item ? 'editTmpl' : 'itemsTmpl';
    };
}
ko.applyBindings(new viewModel());



