var viewModel = function () {
    var self = this;
    self.from = ko.observable().extend({ required: true });
    self.to = ko.observable().extend({ required: true });
    self.pType = ko.observable().extend({ required: true });
    self.progs = ko.observableArray(['Degree', 'NCE']);
    self.pays = ko.observableArray();
    self.modelErrors = ko.validation.group(self);
    self.biz = ko.observable(false);
    self.total = ko.observable();
    self.fetch = function () {
        var isValid = true;
        if (self.modelErrors.length > 0) {
            self.modelErrors.show();
            isValid = false;
        }
        if(isValid)
        {
            self.biz(true);
            $.ajax({
                type: 'get',
                data: { from: self.from(), to: self.to() ,progType:self.pType()},
                url: '/Bursary/FetchCollectionSummary',
                success: function (data) {
                    if (data.length == 0) {
                        alert('No records found within the selected period');
                        return;
                    }
                    self.pays(data.Details);
                    self.total(data.Total);
                },
                complete: function () {
                    self.biz(false);
                }
            })
        }
    }

}

ko.applyBindings(new viewModel());