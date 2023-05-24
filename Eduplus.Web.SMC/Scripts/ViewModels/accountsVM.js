var groups;

$.ajax({
    type: 'Get',
    url: '/Bursary/AccountsList',
    contentType: 'Application/json; charset:utf-8',
    dataType: 'json',
    success: function (result) {
        groups = result;
    }
});
account = function (data) {
    var self = this;

    self.AccountCode = ko.observable(data?data.AccountCode:'');
    self.Title = ko.observable(data?data.Title:'');
    self.Description = ko.observable(data?data.Description:'');
    self.AccountType = ko.observable(data?data.AccountType:'');
    self.OpeningBalance = ko.observable(data?data.OpeningBalance:0.00);
    self.CurrentBalance = ko.observable(data ? data.CurrentBalance : 0.00);
     
    self.Active = ko.observable(data?data.Active:false);

    self.AccountTypes = ko.observableArray(['Assets','Cash Account', 'Expense','Liabilities', 'Revenue', ]);
     
    self.modelErrors = ko.validation.group(self);
};
accountVM = function () {

    var self = this;

    self.list = ko.observableArray([]);

    self.selectedItem = ko.observable();
    self.saveUrl = '/Bursary/SaveAccount';

    //Fetch Accounts

    $.ajax({
        type: 'Get',
        url: '/Bursary/AccountsList',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            
                ko.utils.arrayForEach(data, function (data) {
                    self.list.push(new account(data));
  
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
        var newItem = new account();
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




