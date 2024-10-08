
if (getMessage.trim() != "") {
    triggerSweetAlert(getMessage, getColor);
}

function matchPassword(confirmPw) {
    var newPw = $('#password').val();
    if (newPw != confirmPw) {
        $('#pwErrMsg').text("Password & Confirm Password did not match");
        $('#RegisterForm').attr("disabled", true);
    }
    else {
        $('#pwErrMsg').text("");
        $('#RegisterForm').attr("disabled", false);
    }
}

function validateEmail(email) {
    email = email.trim();
    if (email.includes('"')) {
        email = email.replace('"', '');
    }

    var Emailpattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (Emailpattern.test(email)) {
        $('#emailErrMsg').text("");
    }
    else {
        $('#emailErrMsg').text("Please enter a valid email address!");
        $('#RegisterForm').attr("disabled", true);
        return;
    }
    if (email != "") {
        $.ajax({
            type: 'GET',
            url: projectBaseUrl + "GeneralPurpose/validateEmail",
            dataType: "json",
            data: {
                Email: email,
                UserId: -1
            },
            success: function (response) {
                if (response) {
                    $('#emailErrMsg').text("");
                    $('#RegisterForm').attr('disabled', false);
                }
                else {
                    $('#emailErrMsg').text("");

                    $('#emailErrMsg').text("Email already exist");
                    $('#emailAddress').val("");
                    $('#RegisterForm').attr('disabled', true);
                }
            },
            error: function () {
                $('#emailErrMsg').text("");
                $('#RegisterForm').attr('disabled', false);
                alert("Ajax failed");
            }
        });
    }

}

function validateUsername(username) {
    if (username != "") {
        $.ajax({
            type: 'GET',
            url: projectBaseUrl + "GeneralPurpose/validateUsername",
            dataType: "json",
            data: {
                username: username,
                UserId: -1
            },
            success: function (response) {
                if (response) {
                    $('#nameErrMsg').text("");
                    $('#RegisterForm').attr('disabled', false);
                }
                else {
                    $('#nameErrMsg').text("");

                    $('#nameErrMsg').text("Username already exist");
                    $('#username').val("");
                    $('#RegisterForm').attr('disabled', true);
                }
            },
            error: function () {
                $('#nameErrMsg').text("");
                $('#RegisterForm').attr('disabled', false);
                alert("Ajax failed");
            }
        });
    }

}

function limitText(limitField, limitNum) {
    if (limitField.value.length > limitNum) {
        limitField.value = limitField.value.substring(0, limitNum);
    }
}

function triggerSweetAlert(message, color) {
    $.wnoty({
        type: color.includes("E54826") ? 'error' : 'success',
        message: message,
        autohideDelay: 5000
    });
}

function NotificationSound() {
    notificationSound.play();
}