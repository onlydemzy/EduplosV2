var viewModel = function () {
    var self = this;
    self.matricNo = ko.observable('');
    self.list = ko.observableArray();
    self.busy = ko.observable(false);
    //fetch student
    self.fetchCourses = function () {
        if(self.matricNo=='')
        {
            alert("Input student matricnumber")
            return;
        }
        self.busy(true);
        self.list([]);
        $.ajax({
            type: 'get',
            contentType: 'application/json; charset=utf8',
            data: { matricNo: self.matricNo },
            
            url: '/AcademicAffairs/FetchStudentOutstandings',
            dataType:'json',
            success: function (response) {
                self.list(response);
            },
            complete: function () {
                self.busy(false);
            }
        });
    }

    self.delete = function (item) {
        $.ajax({
            type: 'get',
            data: { outstandingId: item.OutStandingCourseId },
            url: '/AcademicAffairs/DeleteOutstanding',
            contentType: 'application/json; charset=utf8',
            success: function (res) {
                alert(res);
                self.list.remove(item);
            }
        })
    };
}
ko.applyBindings(new viewModel());