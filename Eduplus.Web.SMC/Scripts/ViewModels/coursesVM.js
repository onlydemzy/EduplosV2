
course = function (data) {
    var self = this;

    self.CourseId = ko.observable(data ? data.CourseId : '');
    self.CourseCode = ko.observable(data ? data.CourseCode : '').extend({
        required: true,
        pattern: {
            message: 'Course code format not valid.',
            params: /[A-Z]{3}\s\d{3}$/
        }
    });
    self.Title = ko.observable(data ? data.Title : '').extend({ required: true });
    self.Level = ko.observable(data ? data.Level : 0).extend({ required: true });
    self.Semester = ko.observable(data ? data.Semester : '').extend({ required: true });
    self.Active = ko.observable(data ? data.Active : true);
    self.Type = ko.observable(data ? data.Type : '').extend({ required: true });
    self.ProgrammeCode = ko.observable(data ? data.ProgrammeCode : '');
    self.CreditHours = ko.observable(data ? data.CreditHours : 0).extend({ required: true });
    self.Category = ko.observable(data ? data.Category : '');


    self.modelErrors = ko.validation.group(self);
};
viewModel = function () {

    var self = this;

    self.list = ko.observableArray([]);
    self.pageSize = ko.observable(20);
    self.pageIndex = ko.observable(0);
    self.ProgrammeCode = ko.observable();
    self.programmes = ko.observableArray([]);

    self.categories = ko.observableArray([]);
    self.Types = ko.observableArray(['Compulsory', 'Elective']);
    self.Semesters = ko.observableArray(['1st Semester', '2nd Semester']);

    self.IsVisible = ko.observable(false);
    self.Levels = ko.observableArray([100, 200, 300, 400, 500, 600, 700, 800, 900]);
    self.query = ko.observable('');
    self.selectedItem = ko.observable();
    self.saveUrl = '/AcademicAffairs/SaveCourse';
    self.IsAllowed = ko.observable(false);
    self.spinar = ko.observable(false);

    //Fetch User stat
    $.ajax({
        type: 'Get',
        url: '/HelperService/UserStat',
        dataType: 'json',
        success: function (data) {
            PopulateProgsCourse(data);
        }
    });

    //Check if user is admin
    function PopulateProgsCourse(chk) {
        if (chk == 1)//User is admin Populate Programe
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
            //Populate Programmes based on department
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
    };
    //Populate course pased on programmes
    $.ajax({

        type: 'Get',
        data: { programmeCode: self.ProgrammeCode() },
        url: '/HelperService/CoursesByProgramme',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.list.push(new course(data));

            });
        }
    });
    self.ProgrammeCode.subscribe(function (programmeCode) {
        self.list([]);
        self.spinar(true);

        $.ajax({
            type: 'Get',
            contentyType: 'application/json;charset=utf-8',
            data: { progType:'',progCode:programmeCode},
            url: '/HelperService/CourseCategories',
            dataType: 'json',
            success: function (data) {
                self.categories(data);
            },
            complete: function () {
                self.spinar(false);
            }
        });


        $.ajax({
            type: 'Get',
            contentyType: 'application/json;charset=utf-8',
            data: { programmeCode: programmeCode },
            url: '/HelperService/CoursesByProgramme',
            dataType: 'json',
            success: function (data) {
                self.list(data);
            },
            complete: function () {
                self.spinar(false);
            }
        });
    });



    self.edit = function (item) {
        self.selectedItem(item);
    };

    self.cancel = function () {
        self.selectedItem(null);
    };

    self.add = function () {
        var newItem = new course();
        self.list.push(newItem);
        self.selectedItem(newItem);
        self.moveToPage(self.maxPageIndex());
    };

    self.remove = function (item) {
        if (item.id()) {
            if (confirm('Are you sure you wish to delete this item?')) {
                self.spinar(true);
                $.post(self.deleteUrl, item).complete(function (result) {
                    self.list.remove(item);
                    self.spinar(false);
                    if (self.pageIndex() > self.maxPageIndex()) {
                        self.moveToPage(self.maxPageIndex());
                    }
                });
            }
        }
        else {
            self.list.remove(item);
            if (self.pageIndex() > self.maxPageIndex()) {
                self.moveToPage(self.maxPageIndex());
            }
        }
    };

    self.save = function () {
        self.spinar(true);
        var item = self.selectedItem();
        item.ProgrammeCode = self.ProgrammeCode();


        $.post(self.saveUrl, item, function (result) {
            if (result == null) {
                alert("Course code already exist");
                return false;
            }
            self.selectedItem(result);
            self.selectedItem(null);
        });

    };

    //Search List
    self.searchResult = ko.computed(function () {
        var search = self.query().toLowerCase();
        return ko.utils.arrayFilter(self.list(), function (course) {
            return (
                (self.query().length == 0 || course.Title().toLowerCase().indexOf(self.query().toLowerCase()) > -1)
                )
        });
    });

    self.templateToUse = function (item) {
        return self.selectedItem() === item ? 'editTmpl' : 'itemsTmpl';
    };

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
    self.query.subscribe(self.searchResult);
};
ko.applyBindings(new viewModel());



