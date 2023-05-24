
function step1VM() {
    var self = this;

    self.Surname = ko.observable("").extend({ required: { message: 'Please supply your surname' }});
    self.Firstname = ko.observable("").extend({ required: { message: 'Please supply your firstname' } });
    self.Middlename = ko.observable("").extend({ required: { message: 'Please supply your middle name' } });
    self.Title = ko.observable().extend({ required: true });
    self.Sex = ko.observable().extend({ required: true });
    self.ResidentialAddress = ko.observable();
    self.Email = ko.observable("").extend({ required: { message: 'Please supply a valid email' } });
    self.Phone = ko.observable("");
    self.NextKin = ko.observable("");
    self.KinAddress = ko.observable("");
    self.KinPhone = ko.observable("");
    self.KinMail = ko.observable("");
    self.Country = ko.observable().extend({ required: { message: 'Please supply your country' } });;
    self.State = ko.observable().extend({ required: { message: 'Please supply your state' } });;
    self.Lg = ko.observable().extend({ required: { message: 'Please supply your lga' } });;
    self.MaritalStatus = ko.observable();
    self.DateOfBirth = ko.observable();
    self.RefereeAddress = ko.observable().extend({ required: true });;
    self.Referee = ko.observable().extend({ required: true });;
    self.RefereePhone = ko.observable().extend({ required: true });;
    self.RefereeMail = ko.observable().extend({ required: true });;
    self.Status = ko.observable().extend({ required: { message: 'Please choose your intended programme type' } });

    self.ProgrammeType = ko.observableArray(['Degree', 'Predegree'])
    self.Titles = ko.observableArray(['Mr', 'Mrs', 'Ms']);
    self.MStatus = ko.observableArray(['Single', 'Married', 'Divorced']);
    
    self.Countries = ko.observableArray([]);
    self.States = ko.observableArray([]);
    self.Lgs = ko.observableArray([]);

    self.modelErrors = ko.validation.group(self);
    self.submit = function () {
        var isValid = true;
        if (self.modelErrors().length != 0) {
            self.modelErrors.showAllMessages();
            isValid = false;
            
        }
       
        if (isValid)
        {
            $.ajax({
                type: 'post',
                url: '/Addmissions/Submit_Addmissions_Step1',
                dataType: 'json',
                contentType: 'application/json; charset:utf-8',
                data: ko.toJSON(self),
                success: function (data) {
                    alert("Profile successfully created, your password and registration number has been sent to your mailbox."
                        + " Follow the instructions on the EMail sent to you to login and complete your registration. Proceeding to login page...");
                    windows.location.href = 'https/www.smc.obonguniversity.edu.ng';
                },
                error: function (err) {
                    alert(err.status + " : " + err.statusText);
                }
            });
        }
        
    };
   
    self.Sexes = ko.observableArray(['Male', 'Female']);
    //=============================================Populate Controls======================================
    //Populate Countries
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        url: '/addmissions/PopulateCountry',
        success: function (data) {
            self.Countries(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }

    });
    //Populate State
    self.Country.subscribe(function (country) {
        self.State(undefined);
        $.ajax({
            type: 'get',
            data: { _country: country },
            contentType: 'application/json; charset:utf-8',
            url: '/Addmissions/PopulateState',
            success: function (data) {
                self.States(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });

    //Populate Lgs
    self.State.subscribe(function (state) {
        self.Lg(undefined);
        $.ajax({
            type: 'get',
            data: { _state: state },
            contentType: 'application/json; charset:utf-8',
            url: '/Addmissions/PopulateLga',
            success: function (data) {
                self.Lgs(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });
};
$(document).ready(function () {
    ko.applyBindings(new step1VM());
});