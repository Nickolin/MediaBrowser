﻿(function ($, document, apiClient) {

    function deleteTimer(page, id) {

        Dashboard.confirm("Are you sure you wish to cancel this recording?", "Confirm Recording Cancellation", function (result) {

            if (result) {

                Dashboard.showLoadingMsg();

                ApiClient.cancelLiveTvTimer(id).done(function () {

                    Dashboard.alert('Recording cancelled.');

                    reload(page);
                });
            }

        });
    }

    function renderTimers(page, timers) {

        var html = '';

        html += '<ul data-role="listview" data-inset="true" data-split-icon="delete">';

        var index = '';

        for (var i = 0, length = timers.length; i < length; i++) {

            var timer = timers[i];

            var startDateText = LibraryBrowser.getFutureDateText(parseISO8601Date(timer.StartDate, { toLocal: true }));

            if (startDateText != index) {
                html += '<li data-role="list-divider">' + startDateText + '</li>';
                index = startDateText;
            }

            html += '<li><a href="livetvtimer.html?id=' + timer.Id + '">';

            var program = timer.ProgramInfo || {};
            var imgUrl;
            
            if (program.ImageTags && program.ImageTags.Primary) {

                imgUrl = ApiClient.getImageUrl(program.Id, {
                    height: 160,
                    tag: program.ImageTags.Primary,
                    type: "Primary"
                });
            } else {
                imgUrl = "css/images/items/searchhintsv2/tv.png";
            }

            html += '<img src="css/images/items/searchhintsv2/tv.png" style="display:none;">';
            html += '<div class="ui-li-thumb" style="background-image:url(\'' + imgUrl + '\');width:5em;height:5em;background-repeat:no-repeat;background-position:center center;background-size: cover;"></div>';

            html += '<h3>';
            html += timer.Name;
            html += '</h3>';

            html += '<p>';
            html += LiveTvHelpers.getDisplayTime(timer.StartDate);
            html += ' - ' + LiveTvHelpers.getDisplayTime(timer.EndDate);
            html += '</p>';


            if (timer.SeriesTimerId) {
                html += '<div class="ui-li-aside" style="right:0;">';
                html += '<div class="timerCircle seriesTimerCircle"></div>';
                html += '<div class="timerCircle seriesTimerCircle"></div>';
                html += '<div class="timerCircle seriesTimerCircle"></div>';
                html += '</div>';
            }

            html += '</a>';

            html += '<a data-timerid="' + timer.Id + '" href="#" title="Cancel Recording" class="btnDeleteTimer">Cancel Recording</a>';

            html += '</li>';
        }

        html += '</ul>';

        var elem = $('#items', page).html(html).trigger('create');

        $('.btnDeleteTimer', elem).on('click', function () {

            var id = this.getAttribute('data-timerid');

            deleteTimer(page, id);
        });

        Dashboard.hideLoadingMsg();
    }

    function reload(page) {

        Dashboard.showLoadingMsg();

        apiClient.getLiveTvTimers().done(function (result) {

            renderTimers(page, result.Items);

        });
    }

    $(document).on('pagebeforeshow', "#liveTvTimersPage", function () {

        var page = this;

        reload(page);
    });

})(jQuery, document, ApiClient);