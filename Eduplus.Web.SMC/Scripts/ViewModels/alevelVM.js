//global variables
function getUrlParams(param) {
    var vars = {};
    window.location.href.replace(location.hash, '').replace(
        /[?&]+([^=&]+)=?([^&]*)?/gi, // regexp
        function (m, key, value) { // callback
            vars[key] = value !== undefined ? value : '';
        }
    );

    if (param) {
        return vars[param] ? vars[param] : null;
    }
    return vars;
}
var studentId = getUrlParams('studentId');
function qualy(data) {
    var self = this;
    self.Institution = ko.observable(data?data.Institution:"");
    self.Qualification = ko.observable(data ? data.Qualification : '');
    self.StartMonth = ko.observable(data ? data.StartMonth : '');
    self.EndMonth = ko.observable(data ? data.EndMonth : '');
    self.PersonId = ko.observable(data ? data.PersonId : '');
    self.QualificationId = ko.observable(data?data.QualificationId:0);

     
};
//Work on Image

function resultVM() {
    var self = this;
    
    var self = this;
    self.Institution = ko.observable().extend({ required: true });
    self.Qualification = ko.observable().extend({ required: true });
    self.StartMonth = ko.observable().extend({
        required: true,
        pattern: {
            message: 'Date not valid.',
            params: /[A-Z]{1}[a-z]{2}, \d{4}$/
        }
    });
    self.EndMonth = ko.observable().extend({
        required: true,
        pattern: {
            message: 'Date not valid.',
            params: /[A-Z]{1}[a-z]{2}, \d{4}$/
        }
    });
    self.PersonId = ko.observable();
    self.QualificationId = ko.observable();
    self.list = ko.observableArray();
    self.kill = ko.observable(false);
    
    self.modelError = ko.validation.group(self);
    
    $.ajax({
        type: 'get',
        contentType: 'Application /Json; charset=utf8',
        data:{studentId:studentId},
        url: '/Admission_Center/GetAlevelResults',
        dataType: 'json',
        success: function (data) {
                
                ko.utils.arrayForEach(data, function (sc) {
                    self.list.push(new qualy(sc));
                });
                 
                   

        }
    });

    self.addSubject = function () {
        var isvalid = true;
        if (self.modelError().length > 0) {
            self.modelError.showAllMessages();
            isvalid = false;
        }
        if (isvalid) {
            var item = new qualy();
            item.EndMonth = self.EndMonth();
            item.Institution = self.Institution();
            item.PersonId = studentId;
            item.Qualification = self.Qualification();
            item.StartMonth = self.StartMonth();
            
            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/Admission_Center/SubmitAlevelResult',
                data: ko.toJSON(item),
                success: function (message) {
                    alert(message);
                    self.list.push(item);
                },
                complete: function () {
                    //self.disableButton(false);

                }
            });

        }

    };

    self.removeSubject = function (item) {
        if (confirm('Are you sure you wish to delete this Subject?')) {
            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/Admission_Center/DeleteAlevelResult',
                data: ko.toJSON(item),
                success: function (message) {
                    alert(message);
                    self.list.remove(item);
                },
                complete: function () {
                    //self.disableButton(false);

                }
            })
        }
    }
    self.next = function () {
        window.location.href='/Admission_Center/Addmissions_Step5'
    }
}
$(document).ready(function () {
   
    ko.applyBindings(new resultVM());
    
});