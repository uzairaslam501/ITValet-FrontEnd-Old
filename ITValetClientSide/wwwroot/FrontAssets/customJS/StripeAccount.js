function deleteStripeAccount() {
    debugger;
    $("#staticBackdropDeleteFunctionType").val("Stripe");
    $("#staticBackdropLabel").text("Delete Stripe Account");
    $("#staticBackdropDeleteId").val(getUserIdFromViewBag);
    $("#deleteModal").modal("show");
}

function deleteUserStripeAccount() {
    debugger;
    var stripedeleteUserId = $("#staticBackdropDeleteId").val();
    if (stripedeleteUserId != null && stripedeleteUserId != "") {
        $.ajax({
            type: 'GET',
            headers: {
                'Authorization': Token
            },
            dataType: "json",
            url: projectBaseUrl + "User/postUpdateUserRecord?userId=" + stripedeleteUserId +"&IsDeleteStripeAccount="+ 1,
            data: {
            },
            success: function (response) {
                debugger;
                if (response.data == 0 || response.data == null) {
                    closeModal();
                    $("#stripeErr").removeClass("text-danger").addClass("text-success");
                    $("#stripeErr").text("Account Deleted successfully");

                    /*location.reload();*/
                    location.reload();
                    
                }
                else
                {

                    $("#stripeErr").text(response.message);
                    closeModal();

                }
                
            },
            error: function (response) {
                alert("Something went Wrong Please try again later, or check your internet connection");
            }
        });
    }
}

