ko.validation.rules['areSame'] = {
    getValue: function (o) {
        return (typeof o === 'function' ? o() : o);
    },
    validator: function (val, otherField) {
        return val === this.getValue(otherField);
    },
    message: 'Your passwords do not match'
};

var viewModel = function () {
    var self = this;
    self.OldPassword = ko.observable().extend({required:{message:'Input your current password'}});
    self.NewPassword = ko.observable().extend({ required: { message: 'Input your new password' } });
    self.ConfirmPassword = ko.observable().extend({ areSame: self.NewPassword() });
    self.User=ko.observable(0);
    self.modelErrors = ko.validation.group(self);

    $.ajax({
        type: 'Get',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            self.User(data);
        }
    });

    self.submit = function () {
        var isValid = true;
        if(self.modelErrors().length!=0)
        {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if(isValid)
        {
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Accounts/ChangePasswordSubmit',

                data: ko.toJSON(self),
                success: function (data) {

                    swal({
                        title: "Accounts",
                        text: data,
                        type: "success"
                    },
                        function () {
                            if (self.User() == 2) 
                            { window.location.href = '/Student/Index'; }
                            else { window.location.href = '/Home/Index'; }
                            
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

$(document).ready(function () {
    ko.validation.registerExtenders();
    ko.applyBindings(new viewModel());
    
});