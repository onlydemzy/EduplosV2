
$(document).ready(function () {
    $('#dEmail').prop('visible', false);
});
function viewModel() {
    var self = this;
    //Initial Variables
    self.Recipient = ko.observable().extend({ required: true });
    self.DeliveryAddress = ko.observable().extend({ required: true });
    self.City = ko.observable().extend({ required: true });
    self.State = ko.observable().extend({ required: true });
    self.Country = ko.observable().extend({ required: true });
    self.transAmount = ko.observable();
    self.AmountToPay = ko.observable();
    self.DeliveryMode = ko.observable().extend({ required: true });
    self.TranscriptNo = ko.observable();
    self.DeliveryEmail = ko.observable();
    self.kill = ko.observable(false);
    self.modes = ko.observableArray(['Delivery Service', 'Email']);
    //self.states = ko.observableArray();
    self.countries = ko.observableArray();
    self.modelErrors = ko.validation.group(self);


    $.ajax({
        type: 'get',

        url: '/HelperService/PopulateCountry',
        dataType: 'json',
        success: function (data) {
            self.countries(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }

    });
    self.Country.subscribe(function (c) {
        $.ajax({
            type: 'get',
            data: { country: self.Country() },
            url: '/HelperService/FetchTranscriptAmount',
            dataType: 'json',
            success: function (data) {
                self.AmountToPay(data.Amount);
                self.TranscriptNo(data.ChargeId);
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
    })
    self.DeliveryEmail.subscribe(function (d) {
        if (d == "Email") {
            $('#dEmail').prop('visible', true);
        }
        else {
            $('#dEmail').prop('visible', false);
        }
    });

    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;

        }

        if (isValid) {
            if (self.DeliveryMode() == "Email" && self.DeliveryEmail() == null) {
                alert('You must specify delivery email');
                return;
            }
            self.kill(true);
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Alumni/SubmitTranscriptApplication',
                dataType:'json',
                data: ko.toJSON(self),
                success: function (data) {
                    if (data.flag != "Ok") {
                        alert(data.flag);
                    }
                    else {
                        window.location.href = '/Payments/PaymentInvoice?transId=' + data.value;
                    }
                }
            });

        }

    };
};
ko.applyBindings(new viewModel());
 