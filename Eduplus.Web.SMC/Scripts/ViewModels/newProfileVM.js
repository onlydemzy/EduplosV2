function displayele(value,element){
    var ele;
    if (value == true)
    {
        ele = document.querySelector(element);
        ele.style.display = 'inline';
    }
    else {
        ele = document.querySelector(element);
        ele.style.display = 'none';
    }
    
}
displayele(false, ('#sp'));

function data() {
    var self = this;


    
    self.Surname = ko.observable("").extend({ required: { message: 'Please supply your surname' }});
    self.Firstname = ko.observable("").extend({ required: { message: 'Please supply your firstname' } });
    self.Middlename = ko.observable("");
     
    self.Sex = ko.observable().extend({ required: true });
    
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' },email:true });
     
    self.Phone = ko.observable(data ? data.CourseCode : '').extend({
        required: true,
        pattern: {
            message: 'Invalid phone number',
            params: /^\d{11}$/
        }
    });
    self.Password = ko.observable().extend({ required: { message: 'Please supply password' } });
    self.ConfirmPassword = ko.observable().extend({ required: { message: 'Confirm password' }, equal: { params: self.Password, message: 'Password do not match' } });
    self.Department = ko.observable();
    self.Faculty = ko.observable();
    self.SessionId = ko.observable();
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    self.disable = ko.observable(false);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);
    self.Faculties = ko.observableArray([]);
    self.Departments = ko.observableArray([]);
    
    self.Sexes = ko.observableArray(['Male', 'Female']);
    self.admitSessions = ko.observableArray();
    self.progs = ko.observableArray();
    self.ProgramType = ko.observable().extend({ required: true });
    self.Prog = ko.observable().extend({ required: true });
    self.ProgrammeCode = ko.observable();
    self.pTypes = ko.observableArray();
    self.modelErrors = ko.validation.group(self);
    self.spinar = ko.observable(false);
    

    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/ProgrammeTypes',
        success: function (data) {
            self.pTypes(data);
        }
    });

    self.ProgramType.subscribe(function (typ) {
        displayele(true, '#sp');
        self.Prog(undefined);
        $.ajax({
            type: 'get',
            data:{programType:typ},
            contentType: 'application/json; charset=utf-8',
            url: '/HelperService/ProgrammesByType',
            success: function (data) {
                self.progs(data);
            },
            complete: function () {
                displayele(false, '#sp');
            }
        });
    })
    self.Prog.subscribe(function (prog) {
        if (prog != undefined)
        {
            self.Department(prog.Department);
            self.Faculty(prog.Faculty);
            self.ProgrammeCode(prog.ProgrammeCode);
        }
       
    });
    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
            
        }
       
        if (isValid)
        {
            self.disable(true);
            
            $.ajax({
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                url: '/Admission_Center/SubmitNewStudentProfile',
                
                data: ko.toJSON(self),
                
                success: function (data) {
                    if (data == 1)
                    {
                        alert("Profile successfully created, Login using your email as username to continue with your registration. Check your e-mail for more details. Click Ok to Proceed...");
                        window.location.href='/Accounts/Login';
                                              
                    }
                    if(data==0)
                    {
                        alert("A user with same email address already exist.");
                        return;
                    }
                    if (data == 2) {
                        alert("A user with same phone already exist.");
                        return;
                    }
                        
                    
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                },
                complete: function () { self.disable(false);}
            });
        }
        
    };
   
    
    //=============================================Populate Controls======================================
   
  
};

    ko.applyBindings(new data());
