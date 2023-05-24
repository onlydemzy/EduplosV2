
session = function (data) {
    var self = this;
    self.SessionId = ko.observable(data?data.SessionId:0);
    self.Title = ko.observable(data?data.Title:'').extend({ required: true, maxLength: 100 });
    self.StartDate = ko.observable(moment(data ? data.StartDate : '').format('DD-MMMM-YYYY'));
    self.EndDate = ko.observable(moment(data ? data.EndDate : '').format('DD-MMMM-YYYY'));

    self.SDate=ko.observable(data?data.StartDate:Date());
    self.EDate=ko.observable(data?data.EDate:Date());
    self.IsCurrent = ko.observable(data?data.IsCurrent:false);
    
    self.modelErrors = ko.validation.group(self);

}
sessionsVM = function () {

    var self = this;
    self.sessionList = ko.observableArray([]);
    
    self.selectedItem = ko.observable();
    self.saveUrl = '/Administration/SaveSession';
    //Fetch all departments
    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.sessionList.push(new session(data));

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
        var newItem = new session();
        self.sessionList.push(newItem);
        self.selectedItem(newItem);

    };

    

    self.save = function () {
        var item = self.selectedItem();
        
        $.post(self.saveUrl, item, function (result) {
            self.selectedItem(result);
            self.selectedItem(null);
        });

    };
    self.details = function (item) {
        
        window.location.href = '/Administration/SessionDetails?sessionId=' + item.SessionId();
    }

    self.templateToUse = function (item) {
        return self.selectedItem() === item ? 'editTmpl' : 'itemsTmpl';
    };

};
ko.applyBindings(new sessionsVM());



