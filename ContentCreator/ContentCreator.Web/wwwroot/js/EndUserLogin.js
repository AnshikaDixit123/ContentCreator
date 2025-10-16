$(document).ready(function () {
    localStorage.clear();
    $("#loginForm").submit(function (event) {
        event.preventDefault();
        var UserNameOrEmail = $("#UserNameOrEmail").val();
        var password = $("#password").val();

        var formData = new FormData();
        formData.append('UserNameOrEmail', UserNameOrEmail);
        formData.append('password', password);
        $.ajax({
            url: "https://localhost:7134/" + "api/Authenticate/AuthenticateEndUser",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode === 200) {
                    localStorage.setItem("UserId", response.Result.UserId);
                    localStorage.setItem("UserToken", response.Result.UserToken);
                    localStorage.setItem("RoleType", response.Result.RoleType);
                    location.href = "/content /socialcontent";
                } else {
                    Swal.fire('Error', response.Message || 'Invalid credentials!', 'error');
                }
            },
            error: function (error) {
                Swal.fire('Error', 'Something went wrong!', 'error');
            }
        });
    });
});