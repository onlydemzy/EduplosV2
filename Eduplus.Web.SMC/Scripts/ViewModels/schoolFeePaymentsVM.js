var viewModel=function(){
var self=this;
self.sessionId=ko.observable().extend({required:true});
self.deptCode=ko.observable().extend({required:true});
self.report=ko.observable().extend({required:true});
self.kill=ko.observable(false);
self.reports = ko.observableArray(['To PDF', 'To Excel']);
self.sessions=ko.observableArray();
self.depts=ko.observableArray();
self.modelErrors = ko.validation.group(self);

 $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
        }
    });

    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateDepartment',
        dataType: 'json',
        success: function (data) {            
                self.depts(data);         
        }
    });

self.submit=function(){
var isValid=true;
if(self.modelErrors().length>0)
{
self.modelErrors.showAllMessages();
            isValid = false;
}
if(isValid)
{
switch (self.report())
            {
                case 'To PDF':
                    window.open('/Bursary/ViewSchoolFeePaymentsAsPDF?sessionId=' + self.sessionId() + '&deptCode=' + self.deptCode());
                    break;
                case 'To Excel':
                    window.location.href = '/Bursary/ViewSchoolFeePaymentsAsExcel?sessionId=' + self.sessionId() + '&deptCode=' + self.deptCode();
                    break;
            }
}

};

};
ko.applyBindings(new viewModel());