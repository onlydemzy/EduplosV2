var viewModel = function () {
    var self = this;
    self.trans = ko.observableArray();
    $.ajax({
        type: 'get',
        url: '/Alumni/LatestTranscriptApplicants',
        dataType: 'json',
        success: function (data) {
            self.trans(data);
        }
    });

    self.generate = function (tr) {
        $.ajax({
            type: 'get',
            contentType:'Application/json',
            data:{transcriptNo:tr.TranscriptNo},
            url: '/Result/Transcript',
            dataType: 'json',
            
        });
    }
}