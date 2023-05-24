

studentVM = function () {
    
    var self = this;
    self.Title = ko.observable();
    self.ProgType = ko.observable();
    self.sessionList = ko.observableArray([]);
    self.ProgTypes = ko.observableArray(['Predegree', 'Degree', 'Masters', 'Ph.D']);
    self.candidatesList = ko.observableArray([]);
    
    //Fetch Faculties

    $.ajax({
        type: 'Get',
        url: '/HelperService/PopulateSession',
        contentType: 'application/json; charset:utf-8',
        dataType: 'json',
        success: function (data) {
           // ko.utils.arrayForEach(data, function (data) {
                //self.list.push(new article(data));
            self.sessionList(data);
           // });
        }
    });

    

    self.fetchList = function () {
        if (self.Title===null || self.ProgType===null) {
            swal({
                title: "Missing Parameter",
                text: "Required field is Missing. Please check and try again",
                type: "error"
            });   
              
        }
        else {
            $.ajax({
                type: 'Get',
                data:{session:self.Title,progType:self.ProgType},
                contentType: 'application/json; charset:utf-8',
                url: '/Admission_Center/Applicants',
                success: function (data) {
                    // ko.utils.arrayForEach(data, function (data) {
                    //self.list.push(new article(data));
                    self.candidatesList(data);
                },
                });
        }
    };

    
};
ko.applyBindings(new candidatesVM());

   

