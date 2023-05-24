 

function viewModel() {
    var self = this;
    self.SessionId = ko.observable().extend({required:true});
    self.ChargeId = ko.observable();
    self.charge = ko.observable().extend({ required: true });;
    self.Amount = ko.observable();
     
    self.dC = ko.observable(false);
    self.modelErrors=ko.validation.group(self);
    self.sessions = ko.observableArray([]);
    self.charges = ko.observableArray([]);
    self.data1 = ko.observableArray([]);
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
        url: '/HelperService/StudentOtherChargesByProgType',
        dataType: 'json',
        success: function (data) {
            self.charges(data);
        }
    });
    self.charge.subscribe(function (ch) {
        self.ChargeId(ch.ChargeId);
        self.Amount(ch.Amount);
    })
    self.submit = function () {
        
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if (isValid)
        {
           self.data1.push(self.SessionId());
           self.data1.push(self.ChargeId());

            self.dC(true);
            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/Payments/SubmitOtherChargesPayment',
                dataType:'json',
                data: ko.toJSON(self.data1()),
                
                success: function (result) {
                    if(result.status===1)
                    {
                        window.location.href = '/Payments/PaymentInvoice?transId=' + result.value;
                    }
                    else {
                        if (result.message == "Invalid Request")
                        {
                            alert('Error Generating invoice. Update your profile with valid email and phone number');
                            return;
                        }
                        alert(result.message);
                        self.dC(false);
                        return;
                    }
                },
                error: function (err) {
                    swal({
                        title: "Fee payment",
                        text: err.status + " : " + err.statusText,
                        type: "error"
                    },
                                function () {
                                    window.location.href = '/Bursary/StudentAccount';
                                }

                            );
                }
            })
        }
        
    }
    //====================================================Debit Student ACcount
    
};

ko.applyBindings(new viewModel());