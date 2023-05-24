
faculty = function (data) {
    var self = this;

    self.FacultyCode = ko.observable(data?data.FacultyCode:'');
    self.Title = ko.observable(data?data.Title:'');
    self.Location = ko.observable(data?data.Location:'');

    self.modelErrors = ko.validation.group(self);
};
facultiesVM = function () {
    
    var self = this;
   
    self.list = ko.observableArray([]);

    self.selectedItem = ko.observable();
    self.saveUrl = '/Administration/SaveFaculty';
    
    //Fetch Faculties

    $.ajax({
        type: 'Get',
        url: '/HelperService/LoadFaculties',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.list.push(new faculty(data));

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
        var newItem = new faculty();
        self.list.push(newItem);
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
ko.applyBindings(new facultiesVM());

   

