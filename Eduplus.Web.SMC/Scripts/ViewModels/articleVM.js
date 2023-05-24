var viewModel = function () {
    var self = this;
    
    self.Title = ko.observable().extend({ required: true });
    self.Type = ko.observable().extend({ required: true });
    self.Photo = ko.observable();
    self.PersonPhoto = ko.observable();

    someReader = new FileReader()

    self.save = function () {
        $.ajax({
            type: 'post',
            url: '/Articles/SaveArticle',
            data: ko.toJSON(self),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            
            mimeType: "multipart/form-data",
                
            success: function (data) {
                alert("Waow");
            }
            
        })
    };
};

$(document).ready(function () {

    ko.applyBindings(new viewModel());

});