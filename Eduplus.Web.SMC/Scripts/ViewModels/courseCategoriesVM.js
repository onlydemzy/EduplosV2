
cats= function (data) {
    var self = this;

    self.CategoryId = ko.observable(data ? data.CategoryId : '');
    self.Category = ko.observable(data ? data.Category : '').extend({required: true});
    
    self.modelErrors = ko.validation.group(self);
};
viewModel = function () {

    var self = this;

    self.list = ko.observableArray([]);
    
    self.selectedItem = ko.observable();
    self.saveUrl = '/AcademicAffairs/SaveCourseCategory';
    
    self.spinar = ko.observable(false);

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
        }
    };

    self.save = function () {
        self.spinar(true);
        var item = self.selectedItem();
        
        $.post(self.saveUrl, item, function (result) {
            if (result == null) {
                alert("Course category already exist");
                return false;
            }
            self.selectedItem(result);
            self.selectedItem(null);
        });

    };

    self.templateToUse = function (item) {
        return self.selectedItem() === item ? 'editTmpl' : 'itemsTmpl';
    };

};
ko.applyBindings(new viewModel());



