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
function score(data) {
    var self = this;
    self.Subject = ko.observable(data?data.Subject:"");
    self.Score = ko.observable(data?data.Score:0);
    self.JambRegNumber = ko.observable(data?data.JambRegNumber:'');
    self.JambYear = ko.observable(data?data.JambYear:0);
    self.ScoreId = ko.observable(data ? data.ScoreId : 0);
    self.StudentId = ko.observable(data?data.StudentId:'');

     
};
//Work on Image

function resultVM() {
    var self = this;
    
    var self = this;
    self.JambRegNumber = ko.observable().extend({required:true});
    self.JambYear = ko.observable().extend({ required: true,minLength:4 });
    self.subjects = ko.observableArray();
   
    self.StudentId = ko.observable(studentId);
    self.jambScores = ko.observableArray();
    self.kill = ko.observable(false);
    self.subject = ko.observable().extend({ required: true });
    self.score = ko.observable().extend({ required: true });
    self.modelError = ko.validation.group(self);
    self.totalScore = ko.pureComputed(function () {
        var total = 0;
        if (self.jambScores().length > 0)
        {
            ko.utils.arrayForEach(self.jambScores(), function (item) {
                var value = parseInt(item.Score());

                total += value;
            })
        }
        
        return total;
    }, this);

    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/OLevelSubjects',
        success: function (data) {
            self.subjects(data);
        }

    });

    $.ajax({
        type: 'get',
        contentType: 'Application /Json; charset=utf8',
        data:{studentId:studentId},
        url: '/Admission_Center/GetStudentJambParts',
        dataType: 'json',
        success: function (data) {
            if (data.StudentId != null)
            {
                self.JambRegNumber(data.JambRegNumber);
                self.JambYear(data.JambYear);
                self.StudentId(data.StudentId);
                ko.utils.arrayForEach(data.Scores, function (sc) {
                    self.jambScores.push(new score(sc));
                });
                 
                
            }
            

        }
    });

    self.addSubject = function () {
        var isvalid = true;
        if (self.modelError().length > 0) {
            self.modelError.showAllMessages();
            isvalid = false;
        }
        if (isvalid) {
            self.kill(true);
            var item = new score();
            item.JambRegNumber(self.JambRegNumber());
            item.StudentId(self.StudentId());
            item.JambYear(self.JambYear());
             
            item.Subject(self.subject());
            item.Score(self.score());

            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/Admission_Center/SubmitJambResult',
                data: ko.toJSON(item),
                success: function (response) {
                    if (response == null) {
                        alert("Inputted Examination Number is already in use by someone else");
                        return;
                    }
                    else {
                        item.ScoreId(response.ScoreId);
                        self.jambScores.push(item);
                        alert("Jamb score successfully added");
                        self.score(0);
                        self.subject(undefined);
                    }

                    
                },
                complete: function () {
                    self.kill(false);

                }
            });

        }

    };

    self.removeSubject = function (item) {
        if (confirm('Are you sure you wish to delete this Subject?')) {
            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/Admission_Center/DeleteJambSubject',
                data: ko.toJSON(item),
                success: function (message) {
                    alert(message);
                    self.jambScores.remove(item);
                },
                complete: function () {
                    //self.disableButton(false);

                }
            })
        }
    }



    self.skip = function () {
        window.location.href = '/Admission_Center/Addmissions_Step5'
    }
}
$(document).ready(function () {
   
    ko.applyBindings(new resultVM());
    
});