$(document).ready(function () {

    GetRoleList()
    function GetRoleList() {
        $('#RoleName').html(``)
        $.ajax({
            url: "https://localhost:7134/" + "api/General/GetRoleList",
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var recordRoleType = response.Result.length;
                    $('#RoleName').append(`<option value="">Select Role</option>`);
                    for (var i = 0; i < recordRoleType; i++) {
                        var data = response.Result[i];
                        var optionHtml = `<option value="${data.Id}">${data.RoleName}</option>`;
                        $('#RoleName').append(optionHtml);
                    }
                }
                else {
                    Swal.Fire('warning', 'something went wrong', 'warning');
                }
            },
            error: function (error) {
                console.warn(error)
            }
        });
    }
    $(document).on("click", "#createUser", function () {
        var UserName = $("#UserName").val();
        var EmailAddress = $("#EmailAddress").val();
        var FirstName = $("#FirstName").val();
        var LastName = $("#LastName").val();
        var PhoneNumber = $("#PhoneNumber").val();
        var RoleID = $("#RoleName").val();
        console.log("Selected RoleId: ", RoleID);
        var Password = $("#Password").val();
        var ConfirmPassword = $("#ConfirmPassword").val();

        if (FirstName.length <= 2 || FirstName.length > 8) {
            Swal.fire('Warning', 'First name must be between 3 to 15 char', 'warning');
            return false;
        }
        else if (LastName.length < 3 || LastName.length > 8) {
            Swal.fire('Warning', 'Last name must be between 3 to 8 char', 'warning')
            return false;
        }
        else if (!validateEmail(EmailAddress)) {
            Swal.fire('Warning', 'Please enter a valid email address', 'warning');
            return false;
        }
        else if (PhoneNumber.length > 10) {
            Swal.fire('Warning', 'Phone number must be of 10 digit', 'warning');
            return false;
        }
        else if (!validatePassword(Password)) {
            Swal.fire('Warning', 'Password must be at least 8 characters long, contain one uppercase letter, one lowercase letter, one digit, and one special character', 'warning');
            return false;
        }
        var formdata = new FormData();
        formdata.append('UserName', UserName);
        formdata.append('EmailAddress', EmailAddress);
        formdata.append('FirstName', FirstName);
        formdata.append('LastName', LastName);
        formdata.append('PhoneNumber', PhoneNumber);
        formdata.append('RoleID', RoleID);
        formdata.append('Password', Password);
        formdata.append('ConfirmPassword', ConfirmPassword);

        $.ajax({
            url: "https://localhost:7134/" + "api/Home/CreateUser",
            type: "POST",
            data: formdata,
            contentType: false,
            processData: false,
            success: function (response) {
                console.log(response);
                Swal.fire('Successful', 'User Created Successfully', 'success');
            },
            error: function (error) {
                console.warn(error)
                Swal.fire('Error', 'Something went wrong', 'error');
            }
        });
    });
    function validateEmail(EmailAddress) {
        var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(EmailAddress);
    }
    function validatePassword(Password) {
        var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
        return regex.test(Password);
    }
})