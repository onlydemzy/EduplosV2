
article = function (data) {
    var self = this;

    self.ArticleId = ko.observable(data.ArticleId);
    self.Title = ko.observable(data.Title);
    self.Type = ko.observable(data.Type);

    self.modelErrors = ko.validation.group(self);
};
articlesVM = function () {
    
    var self = this;
   
    self.list = ko.observableArray([]);

    self.selectedItem = ko.observable();
    self.rootUrl = '/ContentManagement/';
    self.deleteUrl = '/ContentManagement/DeleteArticle';
    
    //Fetch Faculties

    $.ajax({
        type: 'Get',
        url: self.rootUrl+'/AllContents',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.list.push(new article(data));

            });
        }
    });

    self.edit = function (item) {
        window.location.href = self.rootUrl + '/EditArticle'+'?articleId='+item.ArticleId();
    };

    
    self.add = function () {
        window.location.href = self.rootUrl + '/AddArticle';
    };

    self.remove = function (item) {
        if (item.ArticleId()) {
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

    
};
ko.applyBindings(new articlesVM());

   

