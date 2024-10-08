function addNewExperience(id) {
    if (id == 1) {
        resetExperienceSectionValues();
    }
    $("#addExperienceSection").show();
}

function hideExperienceSection() {
    $("#addExperienceSection").hide();
    resetExperienceSectionValues();
}

function resetExperienceSectionValues() {
    $("#addUpdateExperienceBtnLabel").text("Add");
    $("#experienceId").val("");
    $("#userExperience").val("");
}

function addUpdateExperienceBtn() {
    $("#addUpdateExperienceBtnLabel").prop("disabled", true);
    var experienceId = $("#experienceId").val();
    var userExperience = $("#userExperience").val();

    if (userExperience == null || userExperience == "") {
        $("#experienceErrorAlertMessagesSection").text("All Fields Are Required");
        $("#addUpdateExperienceBtnLabel").prop("disabled", false);
        return;
    }

    $("#experienceErrorAlertMessagesSection").text("");
    var addOrUpdateUrl = "";
    var ajaxType = "";
    if (experienceId == "" || experienceId == null || experienceId == undefined) {
        ajaxType = 'POST';
        addOrUpdateUrl = "User/PostAddUserExperience?Description=" + userExperience + "&UserId=" + getUserIdFromViewBag;
    }
    else {
        ajaxType = 'PUT';
        addOrUpdateUrl = "User/PostUpdateUserExperience?userExperienceId=" + experienceId + "&Description=" + userExperience;
    }

    $.ajax({
        type: ajaxType,
        headers: {
            'Authorization': Token
        },
        url: projectBaseUrl + addOrUpdateUrl,
        data: {
        },
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            hideExperienceSection();
            getUserExperience();
            location.reload();
        },
        error: function (response) {
            alert("Something went Wrong Please try again later, or check your internet connection");
        },
        complete: function (response) {
            $("#addUpdateExperienceBtnLabel").prop("disabled", false);
        }
    });
}

function getUserExperience() {
    document.getElementById("myExperienceDisplaySection").innerHTML = "";
    $.ajax({
        type: 'GET',
        headers: {
            'Authorization': Token
        },
        url: projectBaseUrl + "User/GetUserExperienceByUserId?userId=" + getUserIdFromViewBag,
        dataType: "json",
        success: function (response) {
            var createTagsBadge = "";
            if (response.data.length == 0) {
                createTagsBadge = "Add Your Experience By Clicking Add New Button";
            }
            else {
                for (var i = 0; i < response.data.length; i++) {
                    createTagsBadge += '<div class="row">' +
                        '<div class="col-md-12">' +
                        '<div class="p-3 d-flex align-items-center  border-bottom osahan-post-header">' +
                        '<div class="mr-2">' +
                        '<p style="max-height: 100px; overflow-y: scroll; word-break: break-all;">' + response.data[i].description + '</p>' +
                        '</div>' +
                        '<span class="ml-auto">' +
                        '<div class="btn-group">' +
                        '<button type="button" class="btn btn-light btn-sm rounded" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                        '<i class="mdi mdi-dots-vertical"></i>' +
                        '</button>' +
                        '<div class="dropdown-menu dropdown-menu-right" style="">' +
                        '<button class="dropdown-item" type="button" onclick="deleteExperience(' + response.data[i].id + ')"><i class="mdi mdi-trash"></i> Delete</button>' +
                        '<button class="dropdown-item" type="button" onclick="getExperienceById(' + response.data[i].id + ')"><i class="mdi mdi-x-circle"></i> Update </button>' +
                        '</div>' +
                        '</div>' +
                        '</span>' +
                        '</div>' +
                        '</div>' +
                        '</div>';
                }
            }
            document.getElementById("myExperienceDisplaySection").innerHTML = createTagsBadge;
        },
        error: function (response) {
            alert("Something went Wrong Please try again later, or check your internet connection");
        }
    });
}

function deleteExperience(id) {
    $("#staticBackdropDeleteFunctionType").val("Experience");
    $("#staticBackdropLabel").text("Delete Experience");
    $("#staticBackdropDeleteId").val(id);
    $("#deleteModal").modal("show");
}

function deleteFunctionExperience() {
    var deleteId = $("#staticBackdropDeleteId").val();
    if (deleteId != null && deleteId != "") {
        $.ajax({
            type: 'DELETE',
            headers: {
                'Authorization': Token
            },
            dataType: "json",
            url: projectBaseUrl + "User/DeleteUserExperience?userExperienceId=" + deleteId,
            data: {
            },
            success: function (response) {
                closeModal();
                getUserExperience();
                location.reload();
            },
            error: function (response) {
                alert("Something went Wrong Please try again later, or check your internet connection");
            }
        });
    }
}

function getExperienceById(id) {
    $.ajax({
        type: 'GET',
        headers: {
            'Authorization': Token
        },
        dataType: "json",
        url: projectBaseUrl + "User/GetUserExperienceById?userExperienceId=" + id,
        data: {
        },
        success: function (response) {
            if (response.data != null) {
                var ExperienceId = $("#experienceId").val(response.data.id);
                var userExperience = $("#userExperience").val(response.data.description);
                var addUpdateExperienceBtnLabel = $("#addUpdateExperienceBtnLabel").text("Update");
                addNewExperience(2);
            }
        },
        error: function (response) {
            alert("Something went Wrong Please try again later, or check your internet connection");
        }
    });
}