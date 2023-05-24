

//Initial selections
$(document).ready(function () {
    $('#btnPay').prop('disabled', true);
    $('#btncs').prop('disabled', true);
    $('#btnds').prop('disabled', true);
    $('#btof').prop('disabled', true);
    $('#btsf').prop('disabled', true);
    $('#btnDetails').prop('disabled', true);
})

function viewModel() {
    var self = this;
    self.studentId = ko.observable();
    self.regNo = ko.observable();
    self.name = ko.observable();
    self.yearAddmitted = ko.observable();
    self.currentLevel = ko.observable();
    self.department = ko.observable();
    self.email = ko.observable();
    self.phone = ko.observable();
    self.balance = ko.observable();
    self.dC=ko.observable(false);
    //Button events
    self.searchStudent = function () {

        if (self.regNo() === "")
        { alert("Enter regnumber to search"); }
        else
        {
            self.dC(true);
            $.ajax({
                type: "GET",
                data: { matricNumber: self.regNo },
                url: "/Bursary/FetchStudentAccount",
                contentType: "Application/json; charset=utf-8",
                
                success: function (response) {


                    if (response != null) {
                        self.studentId(response.StudentId);
                        self.regNo(response.RegNo);
                        self.name(response.Name);
                        self.yearAddmitted(response.YearAddmitted);
                        self.currentLevel(response.CurrentLevel);
                        self.department(response.Department);
                        self.email(response.Email);
                        self.phone(response.Phone);
                        self.balance(response.Balance);
                        $('#btncs').prop('disabled', false);
                        $('#btnds').prop('disabled', false);
                        $('#btnPay').prop('disabled', false);
                        $('#btof').prop('disabled', false);
                        $('#btsf').prop('disabled', false);
                        $('#btnDetails').prop('disabled', false);
                        
                    }
                    else {
                        alert("No account records found for the searched MatricNumber");
                    }


                    //new accountStatement(data);
                },
                complete:function(){
                    self.dC(false);
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }

            });
        }

        
    }
    
    

    self.creditStudent = function () {
        if (self.studentId != null)
            window.location.href = "/Bursary/CreditStudent?studentId="+self.studentId();
        else
            alert("you must provide a matricnumber before you proceed");
    };
    self.debitStudent = function () {
        if (self.studentId != null)
            window.location.href = "/Bursary/DebitStudentAccount?studentId="+self.studentId();
        else
            alert("you must provide a matricnumber before you proceed");
    };
    
    self.addToFeeException = function () {
        if (self.studentId != null)
        {
            window.location.href = "/Bursary/AddStudentToFeeExemption";
        }
            
        else
            alert("you must provide a matricnumber before you proceed");
    };

    self.accountDetails = function () {
        if (self.studentId != null) {
            window.open("/Bursary/StudentAccountsDetail?studentId="+self.studentId());
        }

        else
            alert("you must provide a matricnumber before you proceed");
    }
    self.postOldFee = function () {
        if (self.studentId != null) {
            window.location.href="/Bursary/PostConfirmedOldSchoolFeePayment?studentId=" + self.studentId();
        }

        else
            alert("you must provide a matricnumber before you proceed");
    }
    self.paySchoolFee = function () {
        if (self.studentId != null) {
            window.location.href = "/Payments/SchoolFeePayment?studentId=" + self.studentId();
        }

        else
            alert("you must provide a matricnumber before you proceed");
    }
}

//Apply binding
$(document).ready(function () {
    ko.applyBindings(new viewModel());
});