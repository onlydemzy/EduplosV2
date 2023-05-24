var viewModel = function () {
    var self = this;
    self.progType = ko.observable();
    self.prog = ko.observable();
    self.progTypes = ko.observableArray();
    self.progs = ko.observableArray();
    self.spine = ko.observable(false);
    self.examsOfficers = ko.observableArray();

        $.ajax({
            type: 'get',
            url: '/HelperService/ProgrammeTypes',
            dataType: 'json',
            success: function (resp) {
                self.progTypes(resp);
            }
        });

    //fetch programmes
    self.progType.subscribe(function (typ) {
        self.progs([]);
        $.ajax({
            type: 'get',
            contentType: 'Application/json; charset=utf-8',
            data: { programType: typ },
            url: '/HelperService/ProgrammesByType',
            dataType: 'json',
            success: function (st) {
                self.progs(st);
            }
        });
    })

    self.prog.subscribe(function (code) {
        self.examsOfficers([]);
        $.ajax({
            type: 'get',
            contentType: 'Application/json; charset=utf-8',
            data: { program: code },
            url: '/AcademicAffairs/FetchExamsOfficers',
            dataType: 'json',
            success: function (st) {
                self.examsOfficers(st);
            }
        });
    })
    

}
ko.applyBindings(new viewModel());