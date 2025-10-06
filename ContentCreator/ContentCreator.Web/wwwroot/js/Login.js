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
                localStorage.setItem("UserId", response.Result.UserId)
                localStorage.setItem("RoleType", response.Result.RoleType)
                localStorage.setItem("UserToken", response.Result.UserToken)
                location.href = "/home/dashboard";
            },
            error: function (error) {
                Swal.fire('Error', 'Something went wrong!', 'error');
            }
        });
    });
});