

//Initial selections
$(document).ready(function () {
    $('#btnEx').prop('disabled', true);
    $('#btncs').prop('disabled', true);
    $('#btnds').prop('disabled', true);
})

function viewModel() {
    var self = this;
    self.userId = ko.observable(0);
    self.username = ko.observable("");
    self.fullname= ko.observable("");
    self.email = ko.observable("");
    self.newPassword = ko.observable("");
    self.passwordConfirm = ko.observable("");
    
    
    //Button events
    self.searchUser = function () {
        
        $.ajax({
            type: "GET",
            data: { username: self.username },
            url: "/Accounts/FetchUser",
            contentType: "Application/json; charset=utf-8",
            traditional: true,
            cache:false,
            success: function (response) {
                
                if (self.username() == "")
                    alert("Enter username to search");
                if (response != null) {
                    self.userId(response.UserId);
                    self.username(response.Username);
                    self.fullname(response.Fullname);
                    self.email(response.Email);
                    //self.newPassword(response.NewPassword);
                    //self.passwordConfirm(response.ConfirmPassword)
                    
                }
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
    }

    self.reset = function () {
        $.ajax({
            type: "POST",
            url: "/Accounts/ResetPassword",
            contentType: "Application/json; charset=utf-8",
            datatype: 'json',
            traditional: true,
            data: ko.toJSON(self),
            cache: false,
            success: function (response) {
                
                    alert("Password reset was successful");
                    window.location.href = '/Home/Index';
                
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }

        });
    };
    
}

//Apply binding
$(document).ready(function () {
    ko.applyBindings(new viewModel());
});