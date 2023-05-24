var departmentList;

$.ajax({
    type: 'Get',
    url: '/HelperService/PopulateDepartment',
    contentType: 'application/json; charset:utf-8',
    dataType: 'json',
    success: function (data) {
        departmentList = data;
    }
});
department = function () {
    var self = this;
    self.Code = ko.observable(data.Code);
    self.Title = ko.observable(data.Title);
    
};

programme = function (data) {
    var self = this;
    self.ProgrammeCode = ko.observable(data?data.ProgrammeCode:'').extend({ required: true, minLength: 4 });
    self.Title = ko.observable(data ? data.Title : '').extend({ required: true, maxLength: 100 });
    self.ProgrammeType = ko.observable(data?data.ProgrammeType:'').extend({required:true});
    self.DepartmentCode = ko.observable(data?data.DepartmentCode:'');
    self.IsActive = ko.observable(data ? data.IsActive : false);
    self.MatricNoFormat = ko.observable(data ? data.MatricNoFormat : '').extend({required:true});
    self.departments = ko.observableArray(departmentList);
    self.Department = ko.observable(data ? data.Department : '');
    
    
}
programmesVM = function () {

    var self = this;
    self.programmeList = ko.observableArray([]);
    self.ProgTypes = ko.observableArray();
    self.selectedItem = ko.observable();
    self.saveUrl = '/Administration/SaveProgramme';

    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/ProgrammeTypes',
        success: function (data) {
            self.ProgTypes(data);
        }
    });
    //Fetch all departments
    $.ajax({
        type: 'Get',
        url: '/HelperService/LoadProgrammes',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.programmeList.push(new programme(data));

            });
        }
    });

    
    self.edit = function (item) {
        
        self.selectedItem(item);
    };

    self.cancel = function () {
        self.selectedItem(null);
    };

    self.add = function () {
        var newItem = new programme();
        self.programmeList.push(newItem);
        self.selectedItem(newItem);

    };

    self.remove = function (item) {
        if (item.id()) {
            if (confirm('Are you sure you wish to delete this item?')) {
                $.post(self.deleteUrl, item).complete(function (result) {
                    self.list.remove(item);

                });
            }
        }
        else {
            self.list.remove(item);

        }
    };

    self.save = function () {
        var item = self.selectedItem();
        $.post(self.saveUrl, item, function (result) {
            self.selectedItem(result);
            self.selectedItem(null);
        });

    };

    self.templateToUse = function (item) {
        return self.selectedItem() === item ? 'editTmpl' : 'itemsTmpl';
    };

};
ko.applyBindings(new programmesVM());



