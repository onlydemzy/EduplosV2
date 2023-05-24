var viewModel = function () {
    var self = this;
    self.session = ko.observable();
    self.fromDate = ko.observable();
    self.toDate = ko.observable();
    self.durations = ko.observableArray(['Entire Session', 'Period']);
    self.reports = ko.observableArray(['Collections Summary', 'By Accounts']);
    self.accounts = ko.observableArray();
    self.sessions = ko.observableArray();
    self.rpt = ko.observable();
    self.duration = ko.observable();
    self.hideS = ko.observable(true);
    self.hideP = ko.observable(true);
    self.hideAct = ko.observable(true);
    self.account = ko.observable();
    //populate controls
    $.ajax({
        type: 'Get',
        url: '/HelperService/AccountsList',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            self.accounts(data);
        }
    });
    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        contentType: 'Application/json;charset:utf-8',
        dataType: 'json',
        success: function (data) {
            self.sessions(data);
        }
    });
    self.durations.subscribe(function (d) {
        switch (d) {
            case 'Entire Session':
                self.hideS(false);
                self.hideP(true);
                self.fromDate(undefined);
                self.toDate(undefined);
                break;
            case 'Period':
                self.hideS(true);
                self.hideP(false);
                self.session(undefined);
                break;
        }

    });

    self.reports.subscribe(function (r) {
        switch (r) {
            case 'Collections Summary':
                self.hideAct(true);
                self.account(undefined);
                break;
            case 'By Accounts':
                self.hideAct(false);
                
                break;
        }

    });

    self.printSummary = function () {
        if(self.duration()=='Entire Session' &&self.session()==undefined)
        {
            alert("Select a session to view report");
            return;
        }
        else {
            window.open('/Bursary/SessionCollectionsSummary?sessionID=' + self.session());
        }
    }
    self.printBySessionByAccount = function () {
        if(self.duration()=='Entire Session'||self.account==undefined)
        {
            alert('Select session and account');
            return;
        }
        else {
            window.open('/Bursary/SessionCollectionsByAccount?self.accountCode='+self.account()+'&session='+self.session);
        }
    }
}
ko.applyBindings(new viewModel());