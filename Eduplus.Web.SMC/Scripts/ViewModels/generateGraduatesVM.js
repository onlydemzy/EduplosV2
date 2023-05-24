
viewModel = function () {
    var self = this;
    self.programmeCode = ko.observable();
    self.session = ko.observable();
    self.Kill = ko.observable(false);
    self.programmes = ko.observableArray([]);
    self.sessions = ko.observableArray([]);


    //Check if user is admin
    $.ajax({
        type: 'Get',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            if (data == 1 || data == 2) {
                self.Kill(false);
                LoadProgramme();

            }
            else {
                self.Kill(true);
            }
        }
    });

    self.submit = function () {
        window.location.href = '/Result/Graduants?session=' + self.session() + '&programmeCode=' + self.programmeCode();
    };

    //Populate Session
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/PopulateSession',
        success: function (data) {
            self.sessions(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }
    });

    //Populate Programmes LoadProgrammes
    LoadProgramme = function () {
        $.ajax({
            type: 'get',
            contentType: 'application/json; charset=utf-8',
            url: '/HelperService/LoadProgrammes',
            success: function (data) {
                self.programmes(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    };


};

ko.applyBindings(new viewModel());