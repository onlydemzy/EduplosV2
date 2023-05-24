var groups;
var accts;
$.ajax({
    type: 'Get',
    url: '/HelperService/ProgrammeTypes',
    contentType: 'Application/json; charset:utf-8',
    dataType: 'json',
    success: function (result) {
        groups = result;
    }
});
$.ajax({
    type: 'Get',
    url: '/HelperService/AccountsList',
    contentType: 'Application/json; charset:utf-8',
    dataType: 'json',
    success: function (result) {
        accts = result;
    }
});
ocharge = function (data) {
    var self = this;

    self.ChargeId = ko.observable(data ? data.ChargeId :0);
    self.AccountCode = ko.observable(data ? data.AccountCode : '');
    self.Description = ko.observable(data?data.Description:'');
    self.ProgrammeType = ko.observable(data?data.ProgrammeType:'');
    self.Amount = ko.observable(data?data.Amount:0.00);
    
    self.mainAcct = ko.observable();
    self.progTypes = ko.observableArray(groups);
    self.accounts = ko.observableArray(accts);
     
    self.modelErrors = ko.validation.group(self);

    self.mainAcct.subscribe(function (a) {
        self.AccountCode(a.AccountCode);
        self.Description(a.Title);

    });
};
accountVM = function () {

    var self = this;

    self.list = ko.observableArray([]);

    self.selectedItem = ko.observable();
    self.saveUrl = '/Bursary/SaveOtherCharge';

    //Fetch Charges

    $.ajax({
        type: 'Get',
        url: '/HelperService/FetchOtherCharges',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            
                ko.utils.arrayForEach(data, function (data) {
                    self.list.push(new ocharge(data));
  
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
        var newItem = new ocharge();
        self.list.push(newItem);
        self.selectedItem(newItem);

    };

    self.remove = function (item) {
        if (item.id()) {
            if (confirm('Are you sure you wish to delete this item?')) {
                $.post(self.deleteUrl, item).complete(function (result) {
                    self.list.remove(item);

                });
            }
        }
        else {
            self.list.remove(item);

        }
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

};
$(document).ready(function () {
    ko.applyBindings(new accountVM());
});




