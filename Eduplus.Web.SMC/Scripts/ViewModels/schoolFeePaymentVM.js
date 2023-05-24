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
    self.data1 = ko.observableArray([]);
    self.sessions = ko.observableArray([]);
    self.installments = ko.observableArray([]);
    self.levels = ko.observableArray([100, 200, 300, 400, 500]);
    self.kill = ko.observable(false);
    self.studentId = ko.observable(regNo);
    
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
        data:{studentId:self.studentId()},
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
            self.data1.push(self.sessionId());
            self.data1.push(self.installment());
            self.data1.push(self.lvl());
            self.data1.push(self.studentId());
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Payments/SubmitSchoolFeePayment',
                data: ko.toJSON(self.data1),
                 dataType:'json',
                success: function (data) {
                    if(data.message=="Ok")
                    {
                        window.location.href = '/Payments/PaymentInvoice?transId=' + data.value;
                    }
                    else {
                        alert(data.message);
                        window.location.reload();
                    } 
                },
                complete: function () {
                    self.kill(false);
                }


            });
            
        }
    };
}
ko.applyBindings(new viewModel());