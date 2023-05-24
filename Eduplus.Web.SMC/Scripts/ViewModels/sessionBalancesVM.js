var viewModel=function(){
var self=this;
self.sessionId=ko.observable().extend({required:true});
self.progType=ko.observable().extend({required:true});
self.report=ko.observable().extend({required:true});
self.kill=ko.observable(false);
self.reports = ko.observableArray(['To PDF', 'To Excel']);
self.sessions=ko.observableArray();
self.progTypes=ko.observableArray();
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
     url: '/HelperService/ProgrammeTypes',
     contentType: 'Application/json; charset:utf-8',
     dataType: 'json',
     success: function (result) {
         self.progTypes(result);
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
                    window.open('/Bursary/StudentBalancesByProgTypeAsPDF?sessionId=' + self.sessionId() + '&progType=' + self.progType());
                    break;
                case 'To Excel':
                    window.location.href = '/Bursary/ViewSchoolFeePaymentsAsExcel?sessionId=' + self.sessionId() + '&deptCode=' + self.deptCode();
                    break;
            }
}

};

};
ko.applyBindings(new viewModel());