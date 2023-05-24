var student = function (data) {
    var self = this;
    self.StudentId = ko.observable(data.StudentId);
    self.Matricnumber = ko.observable(data.Matricnumber);
    self.Name = ko.observable(data.Name);
    self.Department = ko.observable(data.Department);
    self.Programme = ko.observable(data.Programme);
    self.Amount = ko.observable(data.Amount);
    self.Exempt = ko.observable(false);
};
var viewModel = function () {
    var self = this;
        
    self.programCode = ko.observable();
    self.deptCode = ko.observable();
        
    self.programs = ko.observableArray([]);
    self.students = ko.observableArray([]);
    self.depts = ko.observableArray([]);
    self.filterResult = ko.observableArray([]);
    var result;
    //Populate department
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset:utf-8',
        url: '/HelperService/PopulateDepartment',
        success: function (data) {
            self.depts(data)
        },
        error: function (err) {
            alert(err.status + " : " + err.statusText);
        }
    });

    //Populate Programmes
    self.deptCode.subscribe(function (deptCode) {
        self.programCode(undefined);
        $.ajax({
            type: 'get',
            data: { _departmentCode: deptCode },
            contentType: 'application/json; charset=utf-8',
            url: '/HelperService/PopulateProgramme',
            success: function (data) {
                self.programs(data)
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    });

    
    self.getList = function () {
        self.students([]);
        $.ajax({
            type: 'get',
            data: { programCode: self.programCode() },
            contentType: 'application/json; charset=utf-8',
            url: '/Bursary/FetchStudentsForExemption',
            success: function (data) {
                ko.utils.arrayForEach(data, function (data) {
                    self.students.push(new student(data));

                });

            },


            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
    };

    self.submit = function () {
        result = [];
        self.filterResult = ko.computed(function () {
            return ko.utils.arrayFilter(self.students(), function (st) {
                return st.Exempt==true;
            })
        });

            //self.filterResult(result);
                $.ajax({
                    type: 'post',
                    contentType: 'application/json; charset=utf-8',
                    url: '/Bursary/AddExemptions',
                    data: ko.toJSON(self.students()),
                    success: function () {
                        swal({
                            title: "Success",
                            text: "Students Successfully added to exemptions, they may proceed to register for semester courses",
                            type: "success",
                        });

                    }

                });
  
    }
};

ko.applyBindings(new viewModel());