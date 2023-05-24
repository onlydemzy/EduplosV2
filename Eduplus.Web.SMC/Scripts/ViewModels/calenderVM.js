var sessions;
$.ajax({
    type: 'Get',
    url: '/HelperService/PopulateSession',
    contentType: 'application/json; charset:utf-8',
    dataType: 'json',
    success: function (data) {
        sessions = data;
    }
});

calender = function (data) {
    var self = this;
    self.CalenderId = ko.observable(data ? data.CalenderId : 0);
    self.Title = ko.observable(data ? data.Title : '');
    self.Semester = ko.observable(data ? data.Semester : '');
    self.Semesters = ko.observableArray(['1st Semester', '2nd Semester']);
    self.SessionId = ko.observable(data ? data.SessionId : 0);
    self.Session = ko.observable(data ? data.Session : '');
    self.Sessions=ko.observable(sessions);
    self.IsCurrent=ko.observable(data?data.IsCurrent:false);
}
calenderVM = function () {

    var self = this;
    
    self.list = ko.observableArray([]);
    self.selectedItem = ko.observable();
    self.saveUrl = '/AcademicAffairs/SaveCalender';
    var title;
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        url: '/AcademicAffairs/AcademicCalenders',
        dataType: 'json',
        success: function (data) {

            ko.utils.arrayForEach(data, function (data) {
                self.list.push(new calender(data));
                
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
        var newItem = new calender();
        self.list.push(newItem);
        self.selectedItem(newItem);

    };

    self.details = function (item) {
        window.location.href = '/AcademicAffairs/CalenderDetails?id=' + item.CalenderId();
    };

    self.save = function () {
        var item = self.selectedItem();
       
        
        
            $.post(self.saveUrl, item, function (result) {
                self.selectedItem(result);
                self.selectedItem(null);
            });
     

    };

    self.templateToUse = function (item) {
        return self.selectedItem() === item ? 'editTmpl' : 'itemsTmpl';
    };
}
$(document).ready(function () {
    ko.applyBindings(new calenderVM());
});
