
function data() {
    var self = this;

    self.Surname = ko.observable("").extend({ required: { message: 'Please supply your surname' }});
    self.Firstname = ko.observable("").extend({ required: { message: 'Please supply your firstname' } });
    self.Middlename = ko.observable("");
    self.Title = ko.observable().extend({ required: true });
    self.Sex = ko.observable().extend({ required: true });
    
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' },email:true });
    self.Phone = ko.observable("").extend({ required: { message: 'Please supply a phone number' }, phone: true,maxLength:11});
    
    self.Password = ko.observable().extend({ required: { message: 'Please supply password' } });
    self.ConfirmPassword = ko.observable().extend({ required: { message: 'Confirm password' }, equal: { params: self.Password, message: 'Password do not match' } });
    
    self.titles = ko.observableArray(['Mr', 'Mrs', 'Ms']);
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    self.disable = ko.observable(false);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);
    self.Faculties = ko.observableArray([]);
    self.Departments = ko.observableArray([]);
    
    self.Sexes = ko.observableArray(['Male', 'Female']);
    self.progs = ko.observableArray();
    self.ProgramType = ko.observable().extend({ required: true });
    self.ProgrammeCode = ko.observable().extend({ required: true });
    self.pTypes = ko.observableArray(["Degree","NCE"]).extend({required:true});
    self.modelErrors = ko.validation.group(self);

    $.ajax({
        type: 'get',
        contentType:'application/json; charset=utf-8',
        url: '/Admissions/AllProgrammes',
        success: function (data) {
            self.progs(data);
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
                url: '/Admissions/SubmitNewStudent1',
                
                data: ko.toJSON(self),
                
                success: function (data) {
                    if (data == 1)
                    {
                        alert("Profile successfully created, Login using your email as username to continue with your registration. Check your e-mail for more details. Click Ok to Proceed...");
                        window.location.href='https://smc.obonguniversity.edu.ng';
                                              
                    }
                    if(data==0)
                    {
                        alert("A user with same email address already exist.Choose a different Email");
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
