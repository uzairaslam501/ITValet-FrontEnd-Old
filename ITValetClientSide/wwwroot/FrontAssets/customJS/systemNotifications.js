function getNotificationCount() {
    $.ajax({
        type: "GET",
        url: projectBaseUrl + 'Notification/GetNotificationsCount?UserId=' + loggedInUserId,
        cache: false,
        success: function (data) {
            $("#notifiCount").empty();

            if (data != 0) {
                $("#notifiCount").append(data);
                $("#notifiCount").show();
            }
        },
        error: function () {
            triggerSweetAlert("Slow Internet Connection Detected, Change your Internet, or Try Again Later", "@ProjectVariables.DangerColor");
        },
        complete: function () {
            getMessageNotificationCount();
        }
    });
}

function getMessageNotificationCount() {
    $.ajax({
        type: "GET",
        url: projectBaseUrl + 'Notification/GetNotificationsCount?UserId=' + loggedInUserId + '&Title=Message',
        cache: false,
        success: function (data) {
            $("#messageNotification").empty();

            if (data != 0) {
                $("#messageNotification").append(data);
                $("#messageNotification").show();
            }
        },
        error: function () {
            triggerSweetAlert("Slow Internet Connection Detected, Change your Internet, or Try Again Later", "@ProjectVariables.DangerColor");
        },
        complete: function () {

        }
    });
}

function getNotifications() {
    $.ajax({
        type: "GET",
        url: projectBaseUrl + 'Notification/GetNotifications?UserId=' + loggedInUserId,
        cache: false,
        success: function (data) {
            if ($('#notificationList').hasClass('show')) {
                var notificationsHeader = '<div b-n5xx7nq1xe="" style="border-bottom: 1px solid #e3e6ec;  padding-top: 0.5rem!important; padding-bottom: 0.5rem!important;" class="dropdown-header d-flex align-items-center py-3">' +
                    '<h5 b-n5xx7nq1xe="" class="text-body mb-0 me-auto">Notifications</h5>' +
                    ' <button b - n5xx7nq1xe="" id = "closeNotificationsButton" class="ml-auto dropdown-notifications-all text-body btn btn-default" data - bs - toggle="tooltip" data - bs - placement="top" title = "Close Notifications" fdprocessedid = "lm797p" >' +
                    '<i b-n5xx7nq1xe="" class="text-danger fa fa-times fa-lg"></i>' +
                    '</button>' +
                    '</div>';

                var notificationsFooter = '<div style="position: sticky; bottom: 0; " id="notificationsFooter" class="row">' +
                    '<a   style=" background-color: #ffffff; border-top: 1px solid #e3e6ec; " title="Delete All Notifications"  class="col-md-6  dropdown-item dropdown-notifications-footer pull-left" onclick="deleteAllNotifications()"> <i style="margin-left:10px; margin-right: 10px;" b-n5xx7nq1xe="" class="fa fa-trash fa-lg"></i> Delete All Notifications</a>' +
                    '<a style=" background-color: #ffffff; border-top: 1px solid #e3e6ec;"  title="Mark all as read"  class="text-primary col-md-6 dropdown-item dropdown-notifications-footer" onclick="markAllNotifications()"><i b-n5xx7nq1xe="" style="margin-right: 10px; color: #fcd609" class="fa fa-check fa-lg"></i> <span style="color:#fcd609">Mark All Notification</span></a></div>';
                var notifications = '<div id="notifications">';

                for (var i = 0; i < data.length; i++) {

                    if (data[i].description.length >= 40) {
                        data[i].description = data[i].description.slice(0, 40) + "...";
                    }

                    var styling = "";
                    var markAsRead = "";
                    if (data[i].isRead == 0) {
                        styling = 'style = "background-color:#d7e2f7"';
                        markAsRead = '<a onclick="markNotification(\'' + data[i].notificationId + '\')" href="#" title="Mark as Read"> <i class="fa fa-circle fa-solid fa-sm text-primary"></i></a>';
                    }

                    notifications += '<div class="dropdown-item dropdown-notifications-item" ' + styling + '>' +
                        '<div class="dropdown-notifications-item-icon bg-systemclr">' +
                        '<i class="fa fa-bell fa-solid fa-lg text-white"></i>' +
                        '</div>' +
                        '<a onclick="markNotification(\'' + data[i].notificationId + '\')" style="width:-webkit-fill-available" href="' + data[i].url + '" target="_blank"  title="Go to notification Page" class="dropdown-notifications-item-content">' +
                        '<div class="dropdown-notifications-item-content-details">' + data[i].createdAt + '</div>' +
                        '<span style="font-weight:600; color: #000">' + data[i].title + '</span>' +
                        '<p style="color: #999">' + data[i].description + '</p>' +
                        '</a>' +
                        '<div>' +
                        '<a href="#" onclick="deleteNotification(\'' + data[i].notificationId + '\')"  title="Delete this notification">' +
                        '<i class="fa fa-trash fa-solid fa-sm text-danger"></i>' +
                        '</a><br/>' +
                        markAsRead +
                        '</div>' +
                        '</div>';
                }

                notifications += "</div>";
                $("#notificationsBody").empty();
                $("#notificationsBody").append(notificationsHeader);

                if (data.length == 0) {
                    $('#delAllNotifiBtn').prop('disabled', true);
                    $('#markAllNotifiBtn').prop('disabled', true);
                    $("#notificationsBody").append('<span class="dropdown-item dropdown-notifications-item text-center d-flex justify-content-center align-items-center" >' +
                        'No Notification to display' +
                        '</span>');
                }
                else {
                    $("#notificationsBody").append(notifications);
                    $('#delAllNotifiBtn').prop('disabled', false);
                    $('#markAllNotifiBtn').prop('disabled', false);
                }
                $("#notificationsBody").append(notificationsFooter);
                $("#notificationsBody").show();
            }

        },
        error: function () {
            console.log("Error fetching notification list.");
            $('#notifiLi').empty();
        },
        complete: function () {

        }
    });

}

function deleteNotification(notificationId) {
    $('#notifiLoader').show();

    $.ajax({
        type: "DELETE",
        url: projectBaseUrl + "Notification/DeleteNotification?NotificationId=" + notificationId,
        cache: false,
        success: function (data) {
            getNotifications();
            getNotificationCount();
        },
        error: function () {
            triggerSweetAlert("Error deleting notification.", "@ProjectVariables.DangerColor");
        },
        complete: function () {
            $('#notifiLoader').hide();
        }
    });
}

function deleteAllNotifications() {
    $('#delAllNotifiBtn').prop('disabled', true);
    $('#notifiLoader').show();

    $.ajax({
        type: "DELETE",
        url: projectBaseUrl + "Notification/DeleteAllNotifications?UserId=" + loggedInUserId,
        cache: false,
        success: function (data) {
            triggerSweetAlert("All Notifications deleted successfully.", "@ProjectVariables.SuccessColor");

            getNotifications();
            getNotificationCount();
        },
        error: function () {
            triggerSweetAlert("Error deleting all notification.", "@ProjectVariables.DangerColor");
        },
        complete: function () {
            $('#notifiLoader').hide();
            $('#delAllNotifiBtn').prop('disabled', false);
        }
    });
}

function markNotification(notificationId) {
    $('#notifiLoader').show();

    $.ajax({
        type: "Get",
        url: projectBaseUrl + "Notification/MarkNotification?NotificationId=" + notificationId,
        cache: false,
        success: function (data) {
            getNotifications();
            getNotificationCount();
        },
        error: function () {
            triggerSweetAlert("Error marking notification.", "@ProjectVariables.DangerColor");
        },
        complete: function () {
            $('#notifiLoader').hide();
        }
    });
}

function markAllNotifications() {
    $('#notifiLoader').show();
    $('#markAllNotifiBtn').prop('disabled', true);

    $.ajax({
        type: "Get",
        url: projectBaseUrl + "Notification/MarkAllNotifications?UserId=" + loggedInUserId,
        cache: false,
        success: function (data) {
            getNotifications();
            getNotificationCount();
            triggerSweetAlert("All Notifications marked as Complete.", "@ProjectVariables.SuccessColor");
        },
        error: function () {
            triggerSweetAlert("Error marking all notification.", "@ProjectVariables.DangerColor");
        },
        complete: function () {
            $('#notifiLoader').hide();
            $('#markAllNotifiBtn').prop('disabled', false);
        }
    });
}

function CloseNotifications() {
    $("#notificationList").removeClass("show");
}