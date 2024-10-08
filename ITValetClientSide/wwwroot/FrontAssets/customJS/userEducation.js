function addNewEducationBtn(id){
    if(id == 1){
        resetEducationSectionValues();
    }
    $("#addEducationSection").show();
}

function hideEducationSection(){
    $("#addEducationSection").hide();
    resetEducationSectionValues();
}

function resetEducationSectionValues(){
    $("#addUpdateEducationBtnLabel").text("Add");
    $("#educationId").val("");
    $("#educationInstituteName").val("");
    $("#educationDegreeName").val("");
    $("#educationStartDate").val(null);
    $("#educationEndDate").val(null);
}

function addUpdateEducationBtn() {
    $("#addUpdateEducationBtnLabel").prop("disabled", true);
    var educationId = $("#educationId").val();
    var educationInstituteName = $("#educationInstituteName").val();
    var educationDegreeName = $("#educationDegreeName").val();
    var educationStartDate = $("#educationStartDate").val();
    var educationEndDate = $("#educationEndDate").val();

    if(educationInstituteName == null || educationInstituteName == ""){
        $("#educationErrorAlertMessagesSection").text("All Fields Are Required");
        $("#addUpdateEducationBtnLabel").prop("disabled", false);
        return;
    }
    if(educationDegreeName == null || educationDegreeName == ""){
        $("#educationErrorAlertMessagesSection").text("All Fields Are Required");
        $("#addUpdateEducationBtnLabel").prop("disabled", false);
        return;
    }
    if(educationStartDate == null || educationStartDate == ""){
        $("#educationErrorAlertMessagesSection").text("All Fields Are Required");
        $("#addUpdateEducationBtnLabel").prop("disabled", false);
        return;
    }
    if (educationEndDate == null || educationEndDate == "") {
        $("#educationErrorAlertMessagesSection").text("All Fields Are Required");
        $("#addUpdateEducationBtnLabel").prop("disabled", false);
        return;
    }

    if(educationEndDate <= educationStartDate){
        $("#educationErrorAlertMessagesSection").text("End Date Must be Greater Than Start Date");
        $("#educationEndDate").val(null);
        $("#addUpdateEducationBtnLabel").prop("disabled", false);
        return ;
    }
    $("#educationErrorAlertMessagesSection").text("");
    var addOrUpdateUrl = "";
    var ajaxType = "";
    if(educationId == "" || educationId == null || educationId == undefined){
        ajaxType = 'POST';
        addOrUpdateUrl = "User/PostAddUserEducation?DegreeName="+educationDegreeName+"&InstituteName="+educationInstituteName+
        "&StartDate="+educationStartDate+"&EndDate="+educationEndDate+ "&UserId="+getUserIdFromViewBag;
    }
    else{
        ajaxType = 'PUT';
        addOrUpdateUrl = "User/PostUpdateUserEducation?userEducationId="+ educationId + "&DegreeName="+educationDegreeName+"&InstituteName="+educationInstituteName+
        "&StartDate="+educationStartDate+"&EndDate="+educationEndDate;
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
            hideEducationSection();
            getUserEducation();
            location.reload();
        },
        error: function (response) {
            alert("Something went Wrong Please try again later, or check your internet connection");
        },
        complete: function (response) {
            $("#addUpdateEducationBtnLabel").prop("disabled", false);
        }
    });
}

function getUserEducation() {
    document.getElementById("myEducationDisplaySection").innerHTML = "";
    $.ajax({
        type: 'GET',
        headers: {
            'Authorization': Token
        },
        url: projectBaseUrl + "User/GetUserEducationByUserId?UserId=" + getUserIdFromViewBag,
        dataType: "json",
        success: function (response) {
            var createTagsBadge = "";
            if(response.data.length == 0){
                createTagsBadge = "Add Your Education By Clicking Add New Button";
            }
            else{
                for (var i = 0; i < response.data.length; i++) {
                    createTagsBadge += '<div class="row">'+
                                            '<div class="col-md-12">'+
                                                '<div class="p-3 d-flex align-items-center  border-bottom osahan-post-header">'+
                                                    '<div class="font-weight-bold mr-1 overflow-hidden">'+
                                                        '<div class="mr-2">' +
                                                            '<h6 class="font-weight-bold text-dark mb-0">' + response.data[i].instituteName + '</h6>' +
                                                            '<div class="text-truncate text-primary">' + response.data[i].degreeName + '</div>' +
                                                            '<div class="small text-gray-500">' + response.data[i].startDate + ' - ' + response.data[i].endDate + ' </div>' +
                                                        '</div>'+
                                                    '</div>'+
                                                    '<span class="ml-auto">'+
                                                            '<div class="btn-group">'+
                                                            '<button type="button" class="btn btn-light btn-sm rounded" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">'+
                                                            '<i class="mdi mdi-dots-vertical"></i>'+
                                                            '</button>'+
                                                            '<div class="dropdown-menu dropdown-menu-right" style="">'+
                                                                '<button class="dropdown-item" type="button" onclick="deleteEducation('+response.data[i].id+')"><i class="mdi mdi-trash"></i> Delete</button>'+
                                                                '<button class="dropdown-item" type="button" onclick="getEducationById('+ response.data[i].id +')"><i class="mdi mdi-x-circle"></i> Update </button>'+
                                                            '</div>'+
                                                            '</div>'+
                                                        '</span>'+
                                                '</div>'+
                                            '</div>'+
                                        '</div>';
                }
            }
            document.getElementById("myEducationDisplaySection").innerHTML = createTagsBadge;
        },
        error: function (response) {
            alert("Something went Wrong Please try again later, or check your internet connection");
        },
        complete: function (response) {
            getUserExperience();
        }
    });
}

function deleteEducation(id){
    $("#staticBackdropDeleteFunctionType").val("Education");
    $("#staticBackdropLabel").text("Delete Education");
    $("#staticBackdropDeleteId").val(id);
    $("#deleteModal").modal("show");
}

function deleteFunctionEducation(){
    var deleteId = $("#staticBackdropDeleteId").val();
    if(deleteId != null && deleteId != ""){
    $.ajax({
        type: 'DELETE',
        headers: {
            'Authorization': Token
        },
        dataType: "json",
        url: projectBaseUrl + "User/DeleteUserEducation?userEducationId=" + deleteId,
        data: {
        },
        success: function (response) {
            closeModal();
            getUserEducation();
            location.reload();
        },
        error: function (response) {
            alert("Something went Wrong Please try again later, or check your internet connection");
        }
    });
    }
}

function getEducationById(id){
    $.ajax({
        type: 'GET',
        headers: {
            'Authorization': Token
        },
        dataType: "json",
        url: projectBaseUrl + "User/GetUserEducationById?userEducationId=" + id,
        data: {
        },
        success: function (response) {
            if(response.data != null){
                var educationId = $("#educationId").val(response.data.id);
                var educationInstituteName = $("#educationInstituteName").val(response.data.instituteName);
                var educationDegreeName = $("#educationDegreeName").val(response.data.degreeName);
                var educationStartDate = $("#educationStartDate").val(response.data.startDate);
                var educationEndDate = $("#educationEndDate").val(response.data.endDate);
                var addUpdateEducationBtnLabel = $("#addUpdateEducationBtnLabel").text("Update");
                addNewEducationBtn(2);
            }
        },
        error: function (response) {
            alert("Something went Wrong Please try again later, or check your internet connection");
        }
    });
}