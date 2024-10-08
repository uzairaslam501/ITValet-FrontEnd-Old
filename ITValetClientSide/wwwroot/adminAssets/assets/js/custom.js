function AccountActivate(id) {
    $("#userActiveReadyForActive").val(id);
    $('#confirmActivateModal').modal('show');
}

function updateStatus() {
    $('#confirmActiveModalAction').attr('disabled', true);
    var userActiveReadyForActive = $('#userActiveReadyForActive').val();
    $.ajax({
        type: 'PUT',
        url: projectBaseUrl + "Admin/UpdateUserActiveness?Id=" + userActiveReadyForActive,
        dataType: "json",
        headers:
        {
            'Authorization': Token
        },
        success: function (response) {
            if (response == true) {
                triggerSweetAlert(nameOfUser + " Activated Successfully", "@ProjectVariables.SuccessColor");
            }
            else {
                triggerSweetAlert("Failed To Activate " + nameOfUser, "@ProjectVariables.DangerColor");
            }
        },
        error: function (response) {
            triggerSweetAlert("There is an error while Activating  " + nameOfUser, "@ProjectVariables.DangerColor");
        },
        complete: function () {
            loadTable();
            $('#userActiveReadyForActive').attr('disabled', false);
            $('#confirmActiveModalAction').modal('hide');
        }
    });
}