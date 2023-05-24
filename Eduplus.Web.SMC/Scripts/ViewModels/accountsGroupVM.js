
group = function (data) {
    var self = this;

    self.AccountsGroupId = ko.observable(data ? data.AccountsGroupId : 0);
    self.Title = ko.observable(data ? data.Title : '').extend({ required: true });
    
    self.modelErrors = ko.validation.group(self);
};
accountVM = function () {

    var self = this;

    self.list = ko.observableArray([]);

    self.selectedItem = ko.observable();
    self.saveUrl = '/Bursary/SaveAccountGroup';

    //Fetch Accounts

    $.ajax({
        type: 'Get',
        url: '/Bursary/AllAccountsGroup',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            
                ko.utils.arrayForEach(data, function (data) {
                    self.list.push(new group(data));
  
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
        var newItem = new group();
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




