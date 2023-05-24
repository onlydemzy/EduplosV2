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
viewModel = function () {
    var self = this;
    self.sessionId = ko.observable().extend({ required: true });
    self.installment = ko.observable().extend({ required: true });
    self.lvl = ko.observable().extend({ required: true });
    self.matricnumber = ko.observable(regNo);
    self.data1 = ko.observableArray([]);
    self.sessions = ko.observableArray([]);
    self.installments = ko.observableArray([]);
    self.levels = ko.observableArray([100, 200, 300, 400, 500]);
    self.kill = ko.observable(false);
    self.amtToPay = ko.observable();
    self.modelErrors = ko.validation.group(self);

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
        data:{studentId:self.matricnumber()},
        url: '/HelperService/StudentFeeOptionsByProgType',
        dataType: 'json',
        success: function (data) {
            self.installments(data);
        }
    });

    self.fetch = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if (isValid) {
           
            self.kill(true);
            self.data1([]);
            self.data1.push(self.sessionId());
            self.data1.push(self.installment());
            self.data1.push(self.lvl());
            self.data1.push(self.matricnumber());
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Bursary/FetchConfirmedSchoolFeePaymentAmount',
                data: ko.toJSON(self.data1),
                 dataType:'json',
                success: function (data) {
                    if(data.value==0)
                    {
                        alert(data.message);
                        return 0;
                    }
                    else {
                        self.amtToPay(data.value);
                    } 
                },
                complete: function (data) {
                    self.kill(false);
                    alert(data);
                }


            });
           
        }
    };

    self.submit = function () {
        if (self.amtToPay() == 0 || self.amtToPay() == 'undefined')
        {
            alert("Amount to pay must be greater than 0");
            return 0;
        }
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if (isValid) {

            self.kill(true);
            self.data1([]);
            self.data1.push(self.sessionId());
            self.data1.push(self.installment());
            self.data1.push(self.amtToPay());
            self.data1.push(self.matricnumber());
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Bursary/SubmitConfirmedSchoolFeePayment',
                data: ko.toJSON(self.data1),
                dataType: 'json',
                success: function (data) {
                    if (data.value == 0) {
                        alert(data.message);
                        return 0;
                    }
                    else {
                        self.amtToPay(data.value);
                    }
                },
                complete: function () {
                    self.kill(false);
                }


            });
            /*
            $.ajax({
                type: 'Get',
                data:{sessionId:self.sessionId()},
                contentyType: 'application/json;charset=utf-8',
                url: '/Payments/CheckPreviousSessionDebt',
                
                dataType: 'json',
                success: function (data) {

                    switch(data)
                    {
                        case 0:
                            alert('ino');
                            //window.location.href = '/Payments/GenerateSchoolFeeInvoice?sessionId=' + self.sessionId() + '&installment=' + self.installment();

                            

                            break;
                        case 1:
                            alert("You are yet to clear previews session debts. Complete outstanding debts before paying for current session");
                            break;
                    }
                    
                }
        });
           */
        }
    };
}
ko.applyBindings(new viewModel());