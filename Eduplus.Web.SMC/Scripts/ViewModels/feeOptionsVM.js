feeOption = function (data) {
    var self = this;
    self.OptionsId = ko.observable(data ? data.OptionsId : 0);
    self.PercentageTution = ko.observable(data ? data.PercentageTution : 0);
    self.ProgrammeType = ko.observable(data ? data.ProgrammeType : '');
    self.PercentageSundry = ko.observable(data ? data.PercentageSundry : 0);
    self.Enabled = ko.observable(data ? data.Enabled : false);
    self.Cycle = ko.observable(data ? data.Cycle : 1);
    self.Installment = ko.observable(data ? data.Installment : '');
    self.WriteExam1 = ko.observable(data ? data.WriteExam1 : false);
    self.WriteExam2 = ko.observable(data ? data.WriteExam2 : false);
    self.Register1 = ko.observable(data ? data.Register1 : false);
    self.Register2 = ko.observable(data ? data.Register2 : false);
    self.edit = function () {
        window.location.href = '/Bursary/EditFeeOptions?optionId='+self.OptionId();
    }

};
var fees = [];

var viewModel = function () {
    var self = this;
    self.list = ko.observableArray();
    self.option = ko.observable();
    //Fetch options
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        url: '/HelperService/GetActiveFeeOptions',
        dataType:'json',
        success: function (result) {
            self.list(result);
        }
    });
    self.add = function () {
        window.location.href = '/Bursary/EditFeeOptions';
    }
}
ko.applyBindings(new viewModel());