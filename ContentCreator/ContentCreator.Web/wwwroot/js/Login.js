9 + $(document).ready(function () {
    localStorage.clear();
    $("#loginForm").submit(function (event) {
        event.preventDefault();
        var UserNameOrEmail = $("#UserNameOrEmail").val();
        var password = $("#password").val();

        var formData = new FormData();
        formData.append('UserNameOrEmail', UserNameOrEmail);
        formData.append('password', password);
        $.ajax({
            url: "https://localhost:7134/" + "api/Authenticate/AuthenticateContentCreator",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                console.log(response)
                localStorage.setItem("UserId", response.Result.UserId)
                localStorage.setItem("UserToken", response.Result.UserToken)
                Swal.fire('success', 'success!', 'success');
            },
            error: function (error) {
                Swal.fire('Error', 'Something went wrong!', 'error');
            }
        });
    });
});