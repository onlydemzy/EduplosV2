applicant=function(data)
{
    var self = this;
    self.count = ko.observable(0);
    self.RegNo=ko.observable(data?data.RegNo:'');
    self.EntryMode=ko.observable(data?data.EntryMode:'');
    self.ProgrammeType=ko.observable(data?data.ProgrammeType:'');
    self.Programme=ko.observable(data?data.Programme:'');
    self.Phone=ko.observable(data?data.Phone:'');
    self.Email=ko.observable(data?data.Email:'');
    self.Name=ko.observable(data?data.Name:'');
    self.Status = ko.observable(data ? data.Status : '');
    self.AddmissionCompleteStage=ko.observable(data?data.AddmissionCompleteStage:0);
};

candidatesVM = function () {
    
    var self = this;
    self.Title = ko.observable();
    self.ProgType = ko.observable();
    self.prog = ko.observableArray();
    self.sessionList = ko.observableArray([]);
    self.ProgTypes = ko.observableArray();
    self.candidatesList = ko.observableArray([]);
    self.need = ko.observable();
    self.dept = ko.observable();
    self.depts = ko.observableArray();
    self.needList = ko.observableArray(['Department', 'Programme']);
    self.programs = ko.observableArray();
    self.kill = ko.observable();
    self.rptType = ko.observable();
    self.genTypes = ko.observableArray(['All', 'Cleared for Screening']);
    self.pageSize = ko.observable(20);
    self.pageIndex = ko.observable(0);
    //Fetch Faculties

    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
           // ko.utils.arrayForEach(data, function (data) {
                //self.list.push(new article(data));
            self.sessionList(data);
           // });
        }
    });

    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateDepartment',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
             
            self.depts(data);
             
        }
    });
    /*
    self.dept.subscribe(function (de) {
        self.prog(undefined);
        $.ajax({
            type: 'Get',
            data:{_departmentCode:self.dept()},
            url: '/HelperService/PopulateProgramme',
            contentType: 'application/json; charset:utf-8',
            dataType: 'json',
            success: function (data) {
                // ko.utils.arrayForEach(data, function (data) {
                //self.list.push(new article(data));
                self.programs(data);
                // });
            }
        });
    });
    */
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/ProgrammeTypes',
        success: function (data) {
            self.ProgTypes(data);
        }
    });
    self.ProgType.subscribe(function (pt) {
        self.kill(true);
        $.ajax({
            type: 'get',
            data:{_departmentCode:self.dept(),programmeType:pt},
            url: '/HelperService/PopulateProgrammeByDeptType',
            success: function (data) {
                self.programs(data);
            },
            complete: function () {
                self.kill(false);
            }
        });
    })
    

    self.fetchList = function () {
        if (self.Title() == undefined || self.ProgType() == undefined || self.rptType() == undefined) {
            swal({
                title: "Missing Parameter",
                text: "Required field is Missing. Please check and try again",
                type: "error"
            });

        }
        else {
            self.candidatesList([]);
            self.kill(true);
            $.ajax({
                type: 'Get',
                data: { session: self.Title, prog: self.prog(),rpt:self.rptType() },
                contentType: 'application/json; charset:utf-8',
                url: '/Admission_Center/Applicants',
                success: function (data) {
                    if (data.length == 0)
                    {
                        alert('No records found');
                        return;
                    }
                    ko.utils.arrayForEach(data, function (data) {
                        self.candidatesList.push(new applicant(data));

                    });
                },
                complete: function () {
                    self.kill(false);
                }
            });
        }
    };

    self.details=function(item)
    {
      window.open('/Admission_Center/Student_Application_Summary?regNo=' + item.RegNo());
         
    }

    self.docs = function (item) {

        window.open('/Content/Documents/' + item.RegNo() + '.pdf');
    }
    self.admitStudent = function (item) {
        if (confirm('Sure to addmit ' + item.Name() + '?')) {
            $.ajax({
                type: 'get',
                data: { studentId: item.RegNo },
                url: '/Admission_Center/AddmitStudent',
                contentType: 'application/json; charset:utf-8',
                success: function (data) {
                    alert(data);
                }
            });
        }

    };

    //Pagination control=========================================================
    self.pagedList = ko.dependentObservable(function () {
        var size = self.pageSize();
        var start = self.pageIndex() * size;
        return self.candidatesList.slice(start, start + size);
    });
    self.maxPageIndex = ko.dependentObservable(function () {
        return Math.ceil(self.candidatesList().length / self.pageSize()) - 1;
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
    //End========================================================================


    self.printByProg = function () {
        if (self.Title === null || self.ProgType === null) {
            swal({
                title: "Missing Parameter",
                text: "Required field is Missing. Please check and try again",
                type: "error"
            });

        }
        else {
            window.open("/Admission_Center/PrintApplicants?session=" + self.Title() + "&prog=" + self.prog()+"&rpt="+self.rptType()+"&fil=p");
        }
    };
    self.printByDept = function () {
        if (self.Title === null || self.ProgType === null) {
            swal({
                title: "Missing Parameter",
                text: "Required field is Missing. Please check and try again",
                type: "error"
            });

        }
        else {
            window.open("/Admission_Center/PrintApplicantsByDept?session=" + self.Title() + "&dept=" + self.dept()+"&progType="+self.ProgType()+"&rpt=" + self.rptType());
        }
    };
};
ko.applyBindings(new candidatesVM());