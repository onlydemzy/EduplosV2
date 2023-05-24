var viewModel = function () {
    var self = this;
    self.transId = ko.observable();
    self.studentId = ko.observable();
    self.amount = ko.observable();
    self.department = ko.observable();
    self.prog = ko.observable();
    self.payType = ko.observable();
    self.status = ko.observable();
    self.particulars = ko.observable();
    self.name = ko.observable();
    self.pDate = ko.observable().extend({required:true});
    self.dC = ko.observable(false);
    self.modelErrors = ko.validation.group(self);
    
    self.fetchTrans = function () {
        if (self.transId() == '') {
            alert("Input a valid transactionID");
            return 0;
        }

        self.dC(true);

        $.ajax({
            type: 'get',
            data: { transId: self.transId() },
            url: '/Payments/FetchManualInvoice',
            success: function (data) {
                if (data == null)
                { alert("Invoice not found"); return 0; }
                self.studentId(data.StudentId);
                self.transId(data.TransactionId);
                self.amount(data.Amount);
                self.department(data.Department);
                self.prog(data.Programme);
                self.payType(data.PaymentType);
                self.status(data.Status);
                self.particulars(data.Particulars);
                self.name(data.Name);
                
            },
            complete:function()
            {
                self.dC(false);
            }
        });
        
    } 
 
    self.confirmPay = function () {
        var isValid = true;
        if (self.modelErrors().length > 0)
        {
            
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if (isValid) {
            if (confirm("Sure to confirm payment for this transaction?")) {

                $.ajax({
                    type: 'get',
                    data: { transId: self.transId(),pDate:self.pDate() },
                    url: '/Payments/SubmitManualPayConfirmation',
                    //data: ko.toJSON(transId),

                    success: function (data) {
                        alert(data);
                        window.location.reload();
                    },
                })
            }
        }
        
    };
 
    self.cancel=function()
    {
        window.location.reload();
    }
};
ko.applyBindings(new viewModel());