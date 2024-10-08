function updateSkills() {
    var skills = $("#skillcategories").val();
    if (skills != null && skills != "") {
        $("#updateSkillsBtn").prop("disabled", true);
        $("#updatedSkillsAlertMessage").text("");
        $.ajax({
            type: 'POST',
            headers: {
                'Authorization': Token
            },
            url: projectBaseUrl + "User/PostAddUserSkill?SkillName=" + skills + "&UserId=" + parseInt(getUserIdFromViewBag),
            dataType: "json",
            success: function (response) {
                location.reload();
            },
            error: function (response) {
                alert("Something went Wrong Please try again later, or check your internet connection");
            },
            complete: function (response) {
                $("#updateSkillsBtn").prop("disabled", false);
            }
        });
    }
    else {
        $("#updatedSkillsAlertMessage").text("Select atleast 1 skills to update");
    }
}

function getUserSkills() {
    document.getElementById("mySelectedTagSkill").innerHTML = "";
    var dropdown = document.getElementById("skillcategories");
    var allOptions = [];
    for (var i = 0; i < dropdown.options.length; i++) {
        allOptions.push(dropdown.options[i].value);
    }
    $.ajax({
        type: 'GET',
        headers: {
            'Authorization': Token
        },
        url: projectBaseUrl + "User/GetUserSkillByUserId?UserId=" + getUserIdFromViewBag,
        dataType: "json",
        success: function (response) {
            var createTagsBadge = "";
            for (var i = 0; i < response.data.length; i++) {
                createTagsBadge += '<span class="badge badge-success ml-1" style="font-size:12px">' + response.data[i].skillName + '</span>';
            }

            document.getElementById("mySelectedTagSkill").innerHTML = createTagsBadge;
        },
        error: function (response) {
            alert("Something went Wrong Please try again later, or check your internet connection");
        },
        complete: function (response) {
            getUserEducation();
        }
    });
}