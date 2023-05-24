﻿

function resultDetails(data) {
    var self = this;
    self.Subject = ko.observable(data ? data.Subject : '');
    self.Grade = ko.observable(data ? data.Grade : '');
    self.ResultId = ko.observable(data ? data.ResultId : '');
    self.DetailId = ko.observable(data ? data.DetailId : 0);
    self.SitAttempt = ko.observable(sitattempt);
    self.StudentId = ko.observable(data ? data.StudentId : '');
    self.ExamNumber = ko.observable(data ? data.ExamNumber : '');
    self.Year = ko.observable(data ? data.Year : 0);
    self.ExamType = ko.observable(data ? data.ExamType : '');
    self.Venue = ko.observable(data ? data.Venue : '');

};
var sitattempt = 1;

function resultVM() {
    var self = this;
    self.SitAttempt = ko.observable(sitattempt);
    self.StudentId = ko.observable();
    self.ExamNumber = ko.observable().extend({ required: true });
    self.Year = ko.observable().extend({ required: true,maxLength:4 });
    self.ExamType = ko.observable().extend({ required: true });
    self.Venue = ko.observable().extend({ required: true });
    self.ResultId = ko.observable('');

    self.Details = ko.observableArray();
    self.examTypes = ko.observableArray(['WAEC', 'NECO', 'NABTEB', 'Teachers Grade Two']);
    self.Subjects = ko.observableArray();
    self.kill = ko.observable(false);
    self.Grades = ko.observableArray(['A1', 'B2', 'B3', 'C4', 'C5', 'C6', 'D7', 'E8', 'F9']);
    self.subject = ko.observable().extend({ required: true });
    self.grade = ko.observable().extend({ required: true });
    self.modelError = ko.validation.group(self);
    $.ajax({
        type: 'get',
        contentType: 'application/json; charset=utf-8',
        url: '/HelperService/OLevelSubjects',
        success: function (data) {
            self.Subjects(data);
        }

    });

    fetchSitting(sitattempt);

    self.addSubject = function () {
        var isvalid = true;
        if (self.modelError().length > 0) {
            self.modelError.showAllMessages();
            isvalid = false;
        }
        if (isvalid) {
            self.kill(true);
            var item = new resultDetails();
            item.ResultId(self.ResultId());
            item.StudentId(self.StudentId());
            item.ExamNumber(self.ExamNumber());
            item.ExamType(self.ExamType());
            item.Venue(self.Venue());
            item.SitAttempt(self.SitAttempt());
            item.Year(self.Year());
            item.Subject(self.subject());
            item.Grade(self.grade());

            $.ajax({
                type: 'Post',
                contentType: 'application/json; charset=utf-8',
                url: '/Admission_Center/AddOlevelResult',
                data: ko.toJSON(item),
                success: function (response) {
                    if (response == null)
                    {
                        alert("Inputted Examination Number is already in use by someone else");
                        return;
                    }
                    else {
                        self.Details.push(response);
                        if (self.ResultId() == '') {
                            self.ResultId(response.ResultId);
                            self.StudentId(response.StudentId);
                            
                        }
                        alert("Olevel subject successfully added");
                        self.grade(undefined);
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
                url: '/Admission_Center/DeleteOlevelSubject',
                data: ko.toJSON(item),
                success: function (message) {
                    alert(message);
                    self.Details.remove(item);
                },
                complete: function () {
                    //self.disableButton(false);

                }
            })
        }
    }


    //button events
    self.addsitting = function () {
        if (sitattempt >= 2) {
            swal("Error", "You cannot add more than 2 OLevel result sittings.", "warning");
            return 0;

        }
        else {
            sitattempt += 1;
            self.SitAttempt(sitattempt);
            fetchSitting(sitattempt);
            
            $('#addsit').prop('disabled', true);

        }


    };



    self.skip = function () {
        if (self.StudentId() == '') {
            alert("Please input your O/Level results to proceed");
            return;
        }
        else {
            window.location.href = '/Admission_Center/JambScore?studentId=' + self.StudentId();
        }
        
    }

    function fetchSitting(sitAt) {
        self.Details([]);
        self.ResultId('');
        self.Venue('');
        self.Year();
        self.ExamType(undefined);
        self.ExamNumber('');
        self.SitAttempt(sitAt);
        self.StudentId('');
         
        $.ajax({
            type: 'get',

            data: { sitting: sitAt },
            url: '/Admission_Center/FetchStudentOlevelResults',
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    self.ResultId(data.ResultId);
                    self.Venue(data.Venue);
                    self.ExamType(data.ExamType);
                    self.ExamNumber(data.ExamNumber);
                    self.SitAttempt(data.SitAttempt);
                    self.StudentId(data.StudentId);
                    self.Year(data.Year);

                    ko.utils.arrayForEach(data.Details, function (dt) {
                        self.Details.push(new resultDetails(dt))
                    });
                }
            }
        })
    }
}
$(document).ready(function () {
    ko.applyBindings(new resultVM());
});


