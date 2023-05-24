//Initial selections
$(document).ready(function () {
    $('#sp').prop('visible', false);

});

var viewModel = function () {
    var self = this;
    self.fromDate = ko.observable().extend({required:true});
    self.toDate = ko.observable().extend({ required: true });
    self.payType = ko.observable().extend({ required: true });
    self.progType = ko.observableArray();
    self.progTypes = ko.observableArray();
    self.payments = ko.observableArray();
    self.modelErrors = ko.validation.group(self);

    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/AccountsList',
        success: function (data) {
            self.progTypes(data);
        }
    });

    self.fetch = function () {
        var isValid = true;
        if (self.modelErrors.length > 0)
        {
            self.modelErrors.showMesssage();
            isValid = false;
        }
        if (isValid)
        {
            $('#btnFetch').prop('disabled', true);
            $('#sp').prop('display', 'inline');
             
        }
        $.ajax({
            type: 'get',
            data:{}
        })
    }
}
ko.applyBindings(new viewModel());