var subject = function (data) {
    var self = this;
    self.SubjectId = ko.observable(data ? data.SubjectId : 0);
    self.Title = ko.observable(data ? data.Title : '');
}

viewModel = function () {

    var self = this;

    self.list = ko.observableArray([]);
     

    self.IsVisible = ko.observable(false);
   
    self.selectedItem = ko.observable();
    self.saveUrl = '/HelperService/SaveSubject';
    self.IsAllowed = ko.observable(false);
    self.spinar = ko.observable(false);

    
            //Populate subjects
            $.ajax({
                type: 'Get',
                contentyType: 'application/json;charset=utf-8',
                url: '/HelperService/OlevelSubjects',
                dataType: 'json',
                success: function (data) {
                    ko.utils.arrayForEach(data, function (d) {
                        self.list.push(new subject(d));
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
        var newItem = new subject();
        self.list.push(newItem);
        self.selectedItem(newItem);
         
    };

    

    self.save = function () {
        self.spinar(true);
        var item = self.selectedItem();
        
        $.post(self.saveUrl, item, function (result) {
            if (result == null) {
                alert("subject already exist");
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



