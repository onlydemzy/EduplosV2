
grading = function (data) {
    var self = this;

    self.GradeId = ko.observable(data ? data.GradeId : 0);
    self.Low = ko.observable(data ? data.Low :0).extend({required:true });
    self.High = ko.observable(data ? data.High : 0).extend({ required: true });
    self.Grade = ko.observable(data ? data.Grade : '').extend({required: true});
    self.GradePoint=ko.observable(data?data.GradePoint:0).extend({ required: true });
    self.Remark=ko.observable(data?data.Remark:'');
    self.ProgrammeType = ko.observable(data ? data.ProgrammeType : '').extend({ required: true });
     
    self.modelErrors = ko.validation.group(self);
};
viewModel = function () {
    
    var self = this;
   
    self.list = ko.observableArray([]);
    self.progTypes = ko.observableArray();
    self.selectedItem = ko.observable();
    self.saveUrl = '/AcademicAffairs/SaveGrade';
    self.deleteUrl = '/AcademicAffairs/DeleteGrade';
    self.IsAllowed = ko.observable(false);
    self.spinar = ko.observable(false);
     
    //Populate course pased on programmeTypes
    $.ajax({
        
        type: 'Get',
        url: '/HelperService/ProgrammeTypes',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            self.progTypes(data);
            }
        
    });

    $.ajax({

        type: 'Get',
        url: '/AcademicAffairs/AllGradings',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.list.push(new grading(data));

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
        var newItem = new grading();
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
            if(result==null)
            {
                alert("Error saving grade");
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

   

