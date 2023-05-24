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
    self.AccountCode = ko.observable().extend({ required: true });;
    self.StudentId = ko.observable(regNo);
    
    self.Particulars = ko.observable().extend({ required: true });
    
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
        url: '/HelperService/AccountsList',
        dataType: 'json',
        success: function (data) {
            self.accounts(data);
        }
    });

    self.submit = function () {
        self.Kill(true);
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
                url: '/Bursary/SaveStudentCredit',

                data: ko.toJSON(self),
                dataType:'json',
                success: function (data) {
                    if(data.status!=1)
                    {
                        alert(data.message);
                        self.Kill(false);
                        return;
                    }
                    else {
                        window.open('/Payments/PaymentInvoice?transId=' + data.value);
                    }
                    
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
        }
    };
};
ko.applyBindings(new viewModel());