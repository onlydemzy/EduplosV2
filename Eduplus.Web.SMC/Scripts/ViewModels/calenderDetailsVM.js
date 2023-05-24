//Get query string url
var urlparam;
function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
};

var urlparam = getParameterByName('id');

calenderDetail = function (data) {
    var self = this;
    self.DetailsId = ko.observable(data ? data.DetailsId : 0);
    self.StartDate = ko.observable(data ? data.StartDate : '');
    self.EndDate = ko.observable(data ? data.EndDate : '');
    self.Semester = ko.observable(data ? data.Semester : '');
    self.Activity = ko.observable(data ? data.Activity : '');
    self.CalenderId = ko.observable(data ? data.CalenderId : 0);
    self.semesters=ko.observableArray(['1st Semester','2nd Semester']);
    //moment(data?data.StartDate:'').format('DD-MM-YYYY')
}
viewModel = function () {

    var self = this;
    self.Title = ko.observable();
    self.SessionId = ko.observable();
    self.Session=ko.observable();
    self.list = ko.observableArray([]);
    self.selectedItem = ko.observable();
    self.CalenderId=ko.observable();

    var saveUrl = '/AcademicAffairs/SaveCalenderDetail';
    var deleteUrl = '/AcademicAffairs/DeleteCalenderDetail';
    var title;
    //Fetch Details
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        data: { id: urlparam },
        url: '/AcademicAffairs/AcademicCalenderDetails',
        dataType: 'json',
        success: function (data) {
            self.Title(data.Title);
            self.SessionId(data.SessionId);
            self.Session(data.Session);
            self.CalenderId(data.CalenderId);


            ko.utils.arrayForEach(data.Details, function (data) {
                self.list.push(new calenderDetail(data));
                
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
        var newItem=new calenderDetail();
        self.list.push(newItem);
        self.selectedItem(newItem);

    };

    self.remove = function (item) {
        
        $.post(deleteUrl, item, function (result) {
            self.list.remove(item);
            self.selectedItem(result);
            self.selectedItem(null);
        });
        
    }

    self.back = function () {
        window.location.href = '/AcademicAffairs/AcademicCalender';
    };
    self.save = function () {
        var item = self.selectedItem();
        item.CalenderId = self.CalenderId();
        
        if (item.StartDate == null || item.StartDate == '' || item.Activity == '' || item.Semester == '')
        {
            alert("A required field is missing");
            return false;
        }
        else {
            $.post(saveUrl, item, function (result) {
                self.selectedItem(result);
                self.selectedItem(null);
            });
        }
        

    };

    self.templateToUse = function (item) {
        return self.selectedItem() === item ? 'editTmpl' : 'itemsTmpl';
    };
}
$(document).ready(function () {
    ko.applyBindings(new viewModel());
});
