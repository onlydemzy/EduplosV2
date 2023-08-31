

//Initial selections
$(document).ready(function () {
    $('#otp').prop('disabled', false);
    $('#btncs').prop('disabled', true);
    $('#btnds').prop('disabled', true);
})

function viewModel() {
    var self = this;
    self.userId = ko.observable(0);
    self.username = ko.observable("");
    self.otp= ko.observable("");
    self.email = ko.observable("");
    
    self.dpMail = ko.observable("");
    self.newPassword = ko.observable("");
    self.confirmPassword = ko.observable("");
    self.hide = ko.observable(false);
    self.hide2 = ko.observable(false);
    self.busy = ko.observable(false);
    self.data = ko.observableArray();
    self.showOtp = ko.observable(false);   
        $.ajax({
            type: "GET",
            
            url: "/Accounts/GetPasswordResetData",
            contentType: "Application/json; charset=utf-8",
            dataType:'json',
            success: function (response) {
                self.userId(response.UserId);
                self.username(response.UserName);
                self.email(response.Email);
                self.dpMail(response.MaskedMail);
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
    self.generateOtp = function () {
        self.busy(true);
        $.ajax({
            type: "get",
            url: "/Accounts/GeneratePasswordOTP",
            contentType: "Application/json; charset=utf-8",
            datatype: 'json',
            success: function (response) {
                if (response == "Ok") {
                    alert("Verification code sent to your email");
                    self.busy(false);
                    self.showOtp(true);
                }

            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
    };

    self.changePassword = function () {
        self.data.push(self.username());
        self.data.push(self.newPassword());
        self.data.push(self.userId());
        self.data.push(self.otp());
        self.busy(true);
        $.ajax({
            type: "post",
            url: "/Accounts/SavePasswordReset",
            contentType: "Application/json; charset=utf-8",
            data:ko.toJSON(self.data()),
             
            success: function (response) {
                if (response == "Ok") {
                    self.busy(false);
                    alert("Password succesfully updated. Please login again");
                    window.location.href='/Accounts/Login';
                }
                else { alert(response); }
                
                
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
        }
    }

//Apply binding
$(document).ready(function () {
    ko.applyBindings(new viewModel());
});