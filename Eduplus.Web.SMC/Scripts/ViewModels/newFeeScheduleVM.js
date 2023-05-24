function getUrlParameter(name1) {
    name1 = name1.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name1 + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};
 
var scheduleId = getUrlParameter('scheduleId');
var aclist = [];

$.ajax({
    type: 'Get',
    url: '/Bursary/AccountsList',
    contentType: 'application/json; charset:utf-8',
    dataType: 'json',
    success: function (data) {
        aclist = data;
    }
});
 
 

//Models
Detail = function (data) {
    var self = this;
    self.ScheduleId = ko.observable(data ? data.ScheduleId : 0);
    self.ScheduleDetailId = ko.observable(data ? data.ScheduleDetailId : 0);
    self.AccountCode = ko.observable(data ? data.AccountCode : '').extend({ required: true });
    self.Amount = ko.observable(data?data.Amount:0.00).extend({required:true,minValue:100});
    self.AppliesTo = ko.observable(data?data.AppliesTo:'').extend({ required: true });
    self.Type = ko.observable(data?data.Type:'').extend({ required: true });
    self.accounts = ko.observableArray(aclist);
    self.types = ko.observableArray(['Bill','LateRegistration1','LateRegistration2','Sundry','Tuition']);
    self.applies = ko.observableArray(['All', 'Freshmen',"Returning Students", 'Optional','Level100','Level200','Level300','Level400','Level500','Level600','Non-Indigenes']);
    self.modelErrors = ko.validation.group(self);
};

// view mpdel
viewModel = function () {
    var self = this;
     
    self.ScheduleId = ko.observable();
    self.SessionId = ko.observable().extend({ required: true });
    self.ProgrammeType = ko.observable('').extend({required:true});
    self.FacultyCode = ko.observable('').extend({ required: true });
    self.progTypes = ko.observableArray([]);

    self.Details = ko.observableArray([]);
    self.Sessions = ko.observableArray([]);
    self.Faculties = ko.observableArray([]);
    self.SelectedItem = ko.observable();
    self.disableButton = ko.observable(false);
    self.selectedItem = ko.observable();
    self.count = ko.pureComputed(function () {
        return self.Details().length;
    });
     
    //Populate Schedule
    
    if (scheduleId > 0) {
        $.ajax({
            type: 'Get',
            data: { scheduleId: scheduleId },
            url: '/Bursary/ScheduleDetail',
            dataType: 'json',
            success: function (data) {
                if (data.ScheduleId > 0) {
                    
                    ko.utils.arrayForEach(data.Details, function (dt) {
                        self.Details.push(new Detail(dt));

                        self.ScheduleId(data.ScheduleId);
                        self.SessionId(data.SessionId);
                        self.ProgrammeType(data.ProgrammeType);
                        self.FacultyCode(data.FacultyCode);
                    });
                }

            }
        });
    }
    //============Populate Controls=================================

    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        contentType: 'Application/json;charset:utf-8',
        dataType: 'json',
        success: function (data) {
            self.Sessions(data);
        }
    });
    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateFaculty',
        contentType: 'Application/json;charset:utf-8',
        dataType: 'json',
        success: function (data) {
            self.Faculties(data);
        }
    });

    $.ajax({
        type: 'Get',
        url: '/HelperService/ProgrammeTypes',
        contentType: 'Application/json; charset:utf-8',
        dataType: 'json',
        success: function (result) {
            self.progTypes(result);
        }
    });
    
    
    

    self.totalAmount = ko.pureComputed(function () {
        var total = 0;
        ko.utils.arrayForEach(self.Details(), function (item) {
            var value = parseInt(item.Amount());

            total += value;
        })
        return total;
    }, this);
    
    
    self.modelErrors = ko.validation.group(self);
    self.detailErrors = ko.validation.group(self.Details(), { deep: true });

    self.addItem = function () {

        
        var lastIndex = self.count() - 1;
        if (lastIndex < 0 || self.Details().length == 0 || self.Details()[lastIndex].ScheduleDetailId() > 0) {
            self.Details.push(new Detail());
             
        }
       
        else {

            var isValid = true;
            if (self.modelErrors().length != 0) {
                self.modelErrors.showAllMessages();
                isValid = false;
            }
            if (self.detailErrors().length != 0) {
                self.detailErrors.showAllMessages();
                isValid = false;
            }

            if (isValid) {
                self.disableButton(true);
                var scheduleLine = self.Details()[lastIndex];
                if (scheduleLine.AccountCode() == undefined || scheduleLine.Amount() == 0 || scheduleLine.AppliesTo() == null || scheduleLine.Type() == null)
                {
                    alert('A required line field is missing');
                    self.disableButton(false);
                    return;
                }
                else if (scheduleLine.ScheduleDetailId() == 0)
                {
                    scheduleLine.ScheduleId = self.ScheduleId();
                    scheduleLine.FacultyCode = self.FacultyCode();
                    scheduleLine.SessionId = self.SessionId();
                    scheduleLine.ProgrammeType = self.ProgrammeType();
                    $.ajax({
                        type: 'Post',
                        contentType: 'application/json; charset=utf-8',
                        url: '/Bursary/AddFeeScheduleLine',
                        data: ko.toJSON(scheduleLine),
                        dataType:'json',
                        success: function (data) {
                            if (data == null) {
                                alert("Oops. Something went wrong, please try again");
                                self.Details.remove(scheduleLine);
                            }
                                else
                                {
                                     
                                    self.ScheduleId(data.ScheduleId);
                                    self.Details.remove(scheduleLine);
                                    self.Details.push(new Detail(data));
                                     
                                }
                                
                            },
                        
                        complete: function () {
                            self.disableButton(false);
                            self.Details.push(new Detail());
                             
                        }
                    });
                }
                else {
                    self.disableButton(false);
                    self.Details.push(new Detail());
                }
                
                    

                    
 
            }
        }
        
    }

    self.removeItem = function (detail) {
        if (confirm("Are you sure, you want to delete this line item?"))
        {
            self.disableButton(true);
            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/Bursary/DeleteFeeScheduleLineItem',
                data: ko.toJSON(detail),
                success: function (message) {
                    switch(message)
                    {
                        case "00":
                            alert("Schedule Item successfully deleted");
                            self.Details.remove(detail);
                            break;
                        case "01":
                            alert("Error: scheduleID not found");
                            break;
                        case "02":
                            alert("Error: cannot remove schedule item because this fee schedule has been applied");
                         
                    }
                     
                },
                complete: function () {
                    self.disableButton(false);
                     
                }

            });
            self.Details.remove(detail)
        }
        
    };


    self.save = function () {

        var isValid = true;
        if (self.modelErrors().length != 0)
        {
            self.modelErrors.showAllMessages();
            isValid = false;
        }
        if (self.detailErrors().length != 0)
        {
            self.detailErrors.showAllMessages();
            isValid = false;
        }

        if (isValid)
        {
            if (confirm("Sure you want to submit?"))
            {
                self.disableButton(true);
                $.ajax({
                    type: 'Post',
                    contentType: 'application/json; charset=utf-8',
                    url: '/Bursary/SaveFeeSchedule',
                    data: ko.toJSON(self),
                    success: function (message) {
                        alert(message);
                        window.location.reload();
                    },

                });
            }
            
        }
       
    }
};
$(document).ready(function () {
    ko.applyBindings(new viewModel());
    
});


