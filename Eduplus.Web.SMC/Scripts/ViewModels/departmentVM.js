var facultyList;

$.ajax({
    type: 'Get',
    url: '/HelperService/LoadFaculties',
    contentType: 'application/json; charset:utf-8',
    
    dataType: 'json',
    success: function (data) {
        facultyList = data;
    }
});
faculty = function () {
    var self = this;
    self.FacultyCode = ko.observable();
    self.Faculty = ko.observable();
    
};

department = function (data) {
    var self = this;
    self.DepartmentCode = ko.observable(data?data.DepartmentCode:'').extend({ required: true, minLength: 4 });
    self.Title = ko.observable(data?data.Title:'').extend({ required: true, maxLength: 100 });
    self.Location = ko.observable(data?data.Location:'').extend({required:false});
    self.Faculty = ko.observable(data ? data.Faculty : '');
    self.FacultyCode = ko.observable(data?data.FacultyCode:'');
    self.IsAcademic = ko.observable(data?data.IsAcademic:false);
    self.faculties = ko.observableArray(facultyList);
    self.modelErrors = ko.validation.group(self);

}
departmentsVM = function () {

    var self = this;
    self.departmentList = ko.observableArray([]);
    
    self.selectedItem = ko.observable();
    self.saveUrl = '/Administration/SaveDepartment';
    //Fetch all departments
    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateDepartment',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.departmentList.push(new department(data));

            });
        }
    });
    

    self.cancel = function () {
        self.selectedItem(null);
    };

    self.add = function () {
        var newItem = new department();
        self.departmentList.push(newItem);
        self.selectedItem(newItem);

    };

    self.remove = function (item) {
        if (item.id()) {
            if (confirm('Are you sure you wish to delete this item?')) {
                $.post(self.deleteUrl, item).complete(function (result) {
                    self.departmentList.remove(item);

                });
            }
        }
        else {
            self.departmentList.remove(item);

        }
    };

    self.edit = function (item) {
        self.selectedItem(item);
    };

    self.save = function () {
        var item = self.selectedItem();
        if (item.IsAcademic === true && item.FacultyCode === null)
        {
            alert("Choose faculty for this academic department");
        }
        else
        {
            $.post(self.saveUrl, item, function (result) {
                self.selectedItem(result);
                self.selectedItem(null);
            });

        }
      
    };

    self.templateToUse = function (item) {
        return self.selectedItem() === item ? 'editTmpl' : 'itemsTmpl';
    };

};
ko.applyBindings(new departmentsVM());



