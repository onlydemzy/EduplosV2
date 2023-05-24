var sessionsList;
//disable controls
disableControls=function(){
    $('#current').prop('disabled', true);
    $('#title').prop('disabled', true);
    $('#control-table').prop('disabled', true);
    $('#sdate').prop('disabled', true);
};
enableControls = function () {
    $('#current').prop('disabled', false);
    $('#title').prop('disabled', false);
    $('#sdate').prop('disabled', false);
}
$(document).ready(function () {
    //$('#btnEx').prop('disabled', true);
    //$('#btncs').prop('disabled', true);
    //$('#btnds').prop('disabled', true);
    disableControls();
})

calenderDetails = function () {
    var self = this;
    self.Activity = ko.observable('');
    self.StartDate = ko.observable('');
    self.EndDate = ko.observable('');
    self.Semester = ko.observable('');
    self.SemesterList = ko.observableArray(['1st Semester', '2nd Semester']);
};

viewModel = function () {
    var self = this;
    self.CalenderId = ko.observable();
    self.SessionId = ko.observable();
    self.Title = ko.observable();
    self.IsCurrent = ko.observable();
    self.Details = ko.observableArray([]);
    self.sessionList = ko.observableArray([]);
    self.selectedItem = ko.observable();
    self.template = ko.observable();

    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            // ko.utils.arrayForEach(data, function (data) {
            //self.list.push(new article(data));
            self.sessionList(data);
            // });
        }
    });

    

    //
    
    //====================================button controls===================================
    self.editCalender = function () {
        enableControls();
    };

    self.addActivity = function () {
        
        var newItem = new calenderDetails();

        self.Details.push(newItem);
        self.selectedItem(newItem);
    };
    self.remove = function () {
        self.Details.remove(calenderDetails);
    };

    self.addCalender = function () {
        enableControls();
        self.Details ([]);
    };

    self.save = function () {
        $.ajax({
            type: 'Post',
            url: '/Administration/SaveCalender',
            contentType: 'application/json; charset:utf-8',
            data:ko.toJSON(self),
            success: function (data) {
                alert("Hallelujah");
            }
        });
    }
    //=======================================================End of button controls============================
    //Fetch Calender
    self.SessionId.subscribe(function (id) {
        self.CalenderId(undefined);
        $.ajax({
            type: 'get',
            data: { sessionId: id },
            contentType: 'application/json; charset:utf-8',
            url: '/Administration/FetchSessionCalender',
            success: function (data) {
                if (data == null) {
                    
                }
                else {
                    self.SessionId(data.SessionId);
                    self.Title(data.Title);
                    self.IsCurrent(data.IsCurrent);
                    self.Details(data.Details);
                    
                }
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });
};

ko.applyBindings(new viewModel());