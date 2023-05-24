var viewModel = function () {
    var self = this;
    self.fromDate = ko.observable().extend({ required: true });
    self.toDate = ko.observable().extend({ required: true });
    self.report = ko.observable().extend({ required: true });
    self.account = ko.observable();
    self.deptCode = ko.observable();
    self.progType = ko.observable();
    self.depts = ko.observableArray();
    self.feeType = ko.observable().extend({ required: true });
    self.feeTypes = ko.observableArray(['School Fee', 'Other Fees']);
    self.accounts = ko.observableArray();
    self.kill = ko.observable(false);
    self.reports = ko.observableArray(['To PDF', 'To Excel']);
    self.progTypes = ko.observableArray();
    self.modelErrors = ko.validation.group(self);
    $.ajax({
        type: 'Get',
        url: '/Bursary/AccountsList',
        contentType: 'Application/json; charset:utf-8',
        dataType: 'json',
        success: function (result) {
            self.accounts(result);
        }
    });
    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateDepartment',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            self.depts(data);

        }
    });
    $.ajax({
        type: 'Get',
        url: '/HelperService/ProgrammeTypes',
        dataType: 'json',
        success: function (data) {
            self.progTypes(data);
        }
    });
    self.feeType.subscribe(function (d) {
        if (d == 'School Fee') {
            self.kill(false);
        }
        if (d == 'Other Fees') {
            self.kill(true);
        }
    })
    self.submit = function () {
        var isValid = true;

        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;

        }
        if (isValid) {

            switch (self.report()) {
                case 'To PDF':
                    window.open('/Bursary/ViewStudentPaymentsAsPDF?fromDate=' + self.fromDate() + '&toDate=' + self.toDate() + '&accountCode=' + self.account() + '&deptCode=' + self.deptCode() + '&progType=' + self.progType() + '&rpt=' + self.feeType());
                    break;
                case 'To Excel':
                    window.location.href = '/Bursary/ViewStudentPaymentsAsExcel?fromDate=' + self.fromDate() + '&toDate=' + self.toDate() + '&accountCode=' + self.account() + '&deptCode=' + self.deptCode() + '&progType=' + self.progType() + '&rpt=' + self.feeType();
                    break;
            }
        }
    }
}
ko.applyBindings(new viewModel());