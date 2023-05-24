var token=function(data)
{
    var self = this;
    self.TokenId=ko.observable(data?data.TokenId:0);
    self.Company = ko.observable(data?data.Company:'').extend({ required: true });
    self.ExpiryDate = ko.observable();
    
    self.modelErrors = ko.validation.group(self);
}
viewModel = function () {
    var self = this;
    
    self.tokens = ko.observableArray([]);
    self.selectedItem = ko.observable();
    self.companyname=ko.observable();
    self.url=ko.observable();
    self.saveUrl = '/Accounts/CreateToken';
    self.show=ko.observable(false);
    //fetch tokens
    $.ajax({
        type: 'get',
        url: '/Accounts/GetTokens',
        contentType: 'Application / Json; charset=utf-8',
        success: function (data) {
            ko.utils.arrayForEach(data, function (data) {
                self.tokens.push(new token(data));
            });
            
        }
    });

    self.cancel = function () {
        self.selectedItem(null);
    };

    self.add = function () {
        var newItem = new token();
        self.show(true);
        

    };

    self.save = function () {
        
        $.ajax({
            type:'Post',
            
            data:{companyname:self.companyname(),url:self.url()},
            url:self.saveUrl,
            success: function (data) {
                alert(data);
            },
            error: function (err) {
                alert(err.status + " : " + err.statusText);
            }
        });
        

    };

    self.view = function (item) {
        window.location.href = '/Accounts/TokenDetails?tokenId=' + item.TokenId();
    };
    
}

ko.applyBindings(new viewModel());