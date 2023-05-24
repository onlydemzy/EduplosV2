
schedule = function (data) {
    var self = this;
    self.SessionId = ko.observable(data ? data.SessionId : 0);
    self.ScheduleId = ko.observable(data ? data.ScheduleId : 0);
    self.Session = ko.observable(data ? data.Session : '');
    self.ProgrammeType = ko.observable(data ? data.ProgrammeType : '');
    self.Faculty = ko.observable(data ? data.Faculty : '');
    self.FacultyCode = ko.observable(data ? data.FacultyCode : '');
    self.Total = ko.observable(data ? data.Total : 0);
    
    self.Status = ko.observable(data ? data.Status : "");
};

viewModel = function () {
    var self = this;
    self.Schedules = ko.observableArray([]);
    self.dC = ko.observable(false);
    $.ajax({
        type: 'Get',
        url: '/Bursary/AllFeeSchedules',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.Schedules.push(new schedule(data))
            });
        }
    });

    self.add = function () {
        window.location.href = '/Bursary/AddFeeSchedule';
    }

    self.detail = function (item) {
        window.location.href = '/Bursary/AddFeeSchedule?scheduleId=' + item.ScheduleId();
    }
    self.apply = function (item) {
        if (confirm("Are you sure, you want to apply this feeschedule?"))
        {
            self.dC(true);
            $.ajax({
                type: 'Get',
                data: { scheduleId: item.ScheduleId() },
                url: '/Bursary/ApplySessionFeeSchedule',
                contentType: "Application/json; charset=utf-8",
                traditional: true,
                success: function (msg) {
                    alert(msg);
                    window.location.reload();
                },
                complete: function () {
                    self.dC(false);
                }
            });
        }
       
    
    }
};

$(document).ready(function () {
    ko.applyBindings(new viewModel());
});

