function getUrlParams(param) {
    var vars = {};
    window.location.href.replace(location.hash, '').replace(
        /[?&]+([^=&]+)=?([^&]*)?/gi, // regexp
        function (m, key, value) { // callback
            vars[key] = value !== undefined ? value : '';
        }
    );

    if (param) {
        return vars[param] ? vars[param] : null;
    }
    return vars;
}
var regNo = getUrlParams('studentId');

function viewModel() {
    var self = this;
    self.SessionId = ko.observable().extend({ required: true });

    self.Kill = ko.observable(false);
    self.Amount = ko.observable().extend({ required: true });
    self.PayDate = ko.observable().extend({ required: true });;
    self.AccountCode = ko.observable().extend({ required: true });;
    self.StudentId = ko.observable(regNo);
    self.VoucherNumber = ko.observable().extend({ required: true });
    self.PaidBy = ko.observable().extend({ required: true });
    self.Particulars = ko.observable().extend({ required: true });
    self.AccountCode = ko.observable().extend({ required: true });;
    self.modelErrors = ko.validation.group(self);

    self.accounts = ko.observableArray([]);

    self.sessions = ko.observableArray([]);

    //Populate controlls
    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
        }
    });
    $.ajax({
        type: 'Get',
        contentyType: 'application/json;charset=utf-8',
        url: '/HelperService/FetchFeeAccounts',
        dataType: 'json',
        success: function (data) {
            self.accounts(data);
        }
    });

    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        (isValid)
        {
            self.StudentId(regNo);
            self.Kill(true);
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Bursary/SaveStudentDebit',

                data: ko.toJSON(self),
                success: function (data) {

                    swal({
                        title: "Bursary",
                        text: data,
                        type: "success"
                    },
                        function () {
                            window.location.href = '/Bursary/StudentAccount';
                        }

                    );


                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
        }
    };
};
ko.applyBindings(new viewModel());