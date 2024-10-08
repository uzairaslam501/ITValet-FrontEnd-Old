function deletePayPalAccount() {
    $("#staticBackdropDeleteFunctionType").val("PayPal");
    $("#staticBackdropLabel").text("Delete Paypal Account");
    $("#staticBackdropDeleteId").val(getUserIdFromViewBag);
    $("#deleteModal").modal("show");
}

function deleteFunctionPayPal() {
    var deleteId = $("#staticBackdropDeleteId").val();
    if (deleteId != null && deleteId != "") {
        $.ajax({
            type: 'DELETE',
            headers: {
                'Authorization': Token
            },
            dataType: "json",
            url: projectBaseUrl + "PayPalGateWay/DeletePayPalAccount?id=" + deleteId,
            data: {
            },
            success: function (response) {
                closeModal();
                location.reload();
            },
            error: function (response) {
                alert("Something went Wrong Please try again later, or check your internet connection");
            }
        });
    }
}