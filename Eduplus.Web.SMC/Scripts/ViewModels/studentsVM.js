student = function (data) {
    var self = this;

    self.StudentId = ko.observable(data.StudentId);
    self.FullName = ko.observable(data.FullName);
    self.Status = ko.observable(data.Status);
    self.Phone = ko.observable(data.Phone);
    self.Programme = ko.observable(data.Programme);
    self.MatricNumber = ko.observable(data.MatricNumber);

};

viewModel = function () {
    var self = this;
    
    self.ProgrammeCode = ko.observable();
    self.admittedSession = ko.observable();
    self.searchText = ko.observable();
    self.Kill = ko.observable(false);
    self.DepartmentCode = ko.observable();
    self.departments = ko.observableArray([]);
    self.programmes = ko.observableArray([]);
    self.sessions = ko.observableArray([]);
    self.list = ko.observableArray([]);
    self.dis = ko.observable(false);
    self.hid = ko.observable(true);
    self.pageSize = ko.observable(25);
    self.pageIndex = ko.observable(0);
    self.query = ko.observable('');
    self.reportTypes = ko.observableArray(['PDF', 'Excel']);
    self.reportType = ko.observable();

    //Check if user is admin
    $.ajax({
        type: 'Get',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            PopulateProgsCourse(data);
        }
    });

    function PopulateProgsCourse(chk) {
        if (chk == 1 || chk == 2)//User is admin Populate Programe
        {
            //Populate Programme
            $.ajax({
                type: 'Get',
                contentyType: 'application/json;charset=utf-8',
                url: '/HelperService/LoadProgrammes',
                dataType: 'json',
                success: function (data) {
                    self.programmes(data);
                }
            });

        }
        else {
            //Populate Course

            $.ajax({
                type: 'Get',
                contentyType: 'application/json;charset=utf-8',
                url: '/HelperService/PopulateDeptProgrammes',
                dataType: 'json',
                success: function (data) {
                    self.programmes(data);
                }
            });
        }
    }

    self.fetch = function () {
        self.dis(true);
        self.list([]);
        $.ajax({
            type: 'Get',
            data: { _programmeCode: self.ProgrammeCode(), _sessionAdmitted: self.admittedSession() },
            url: '/Student/FetchStudents',
            contentType: 'application/json; charset:utf-8',
            dataType: 'json',
            success: function (data) {

                if (data == []) {
                    alert('No records found');
                }
                else {
                    ko.utils.arrayForEach(data, function (data) {
                        self.list.push(new student(data));

                    });
                }

            },
            complete: function () {
                self.dis(false);
                if (self.pagedList().length == 0) {
                    alert('No records found');
                }
            }
        });
    };

    self.search = function () {
        if (self.searchText() == '' || self.searchText() == undefined) {
            alert('Search string cannot be empty');
            return;
        }
        else {
            self.dis(true);
            self.list([]);
            $.ajax({
                type: 'Get',
                data: { search: self.searchText() },
                url: '/Student/FindStudents',
                contentType: 'application/json; charset:utf-8',
                dataType: 'json',
                success: function (data) {

                    if (data == []) {
                        swal({
                            title: "Students",
                            text: "No records found for choosen period",
                            type: "error"
                        });
                    }
                    ko.utils.arrayForEach(data, function (data) {
                        self.list.push(new student(data));

                    });
                },
                complete: function () {
                    self.dis(false);
                }
            });
        }

    };
    self.fetch = function () {
        if (self.ProgrammeCode() == undefined || self.admittedSession() == undefined) {
            alert('A required field is missing');
            return;
        }
        else {
            self.dis(true);
            self.list([]);
            $.ajax({
                type: 'Get',
                data: { _departmentCode: self.DepartmentCode(), _programmeCode: self.ProgrammeCode(), _sessionAdmitted: self.admittedSession() },
                url: '/Student/FetchStudents',
                contentType: 'application/json; charset:utf-8',
                dataType: 'json',
                success: function (data) {

                    if (data == []) {
                        swal({
                            title: "Students",
                            text: "No records found for choosen period",
                            type: "error"
                        });
                    }
                    ko.utils.arrayForEach(data, function (data) {
                        self.list.push(new student(data));

                    });
                },
                complete: function () {
                    self.dis(false);
                }
            });
        }

    };

    self.print = function () {
        if (self.reportType() == 'PDF') {
            window.open('/Student/PrintStudent?deptCode=' + self.DepartmentCode() + '&progCode=' + self.ProgrammeCode() + '&ses=' + self.admittedSession());
        }

        else if (self.reportType() == 'Excel') {
            window.location.href = '/Student/StudentsAsExcel?deptCode=' + self.DepartmentCode() + '&progCode=' + self.ProgrammeCode() + '&ses=' + self.admittedSession();
        }
        else {
            alert('Select a report type to generate');
            return;
        }
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
    //Populate Students
    self.pagedList = ko.dependentObservable(function () {
        var size = self.pageSize();
        var start = self.pageIndex() * size;
        return self.list.slice(start, start + size);
    });
    self.maxPageIndex = ko.dependentObservable(function () {
        return Math.ceil(self.list().length / self.pageSize()) - 1;
    });
    self.previousPage = function () {
        if (self.pageIndex() > 0) {
            self.pageIndex(self.pageIndex() - 1);
        }
    };
    self.nextPage = function () {
        if (self.pageIndex() < self.maxPageIndex()) {
            self.pageIndex(self.pageIndex() + 1);
        }
    };
    self.allPages = ko.dependentObservable(function () {
        var pages = [];
        for (i = 0; i <= self.maxPageIndex() ; i++) {
            pages.push({ pageNumber: (i + 1) });
        }
        return pages;
    });
    self.moveToPage = function (index) {
        self.pageIndex(index);
    };


    self.edit = function (student) {
        window.open('/Student/EditProfile?studentId=' + student.StudentId());
    }
    self.detail = function (student) {
        window.open('/Student/StudentBioProfile?studentId=' + student.StudentId());
    }

};

ko.applyBindings(new viewModel());