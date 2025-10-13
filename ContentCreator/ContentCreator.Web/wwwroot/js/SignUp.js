$(document).ready(function () {

    $(document).on("click", "#SignUp", function (event) {
        event.preventDefault(); // prevent form submission

        // Get values and trim whitespace
        var UserName = $("#UserName").val().trim();
        var EmailAddress = $("#EmailAddress").val().trim();
        var FirstName = $("#FirstName").val().trim();
        var LastName = $("#LastName").val().trim();
        var PhoneNumber = $("#PhoneNumber").val().trim();
        var Password = $("#Password").val();
        var ConfirmPassword = $("#ConfirmPassword").val();

        // Validation
        if (!FirstName || FirstName.length < 3 || FirstName.length > 8) {
            Swal.fire('Warning', 'First name must be between 3 to 8 characters', 'warning');
            return false;
        }

        if (!LastName || LastName.length < 3 || LastName.length > 8) {
            Swal.fire('Warning', 'Last name must be between 3 to 8 characters', 'warning');
            return false;
        }

        if (!validateEmail(EmailAddress)) {
            Swal.fire('Warning', 'Please enter a valid email address', 'warning');
            return false;
        }

        if (!/^\d{10}$/.test(PhoneNumber)) {
            Swal.fire('Warning', 'Phone number must be exactly 10 digits', 'warning');
            return false;
        }

        if (!validatePassword(Password)) {
            Swal.fire('Warning', 'Password must be at least 8 characters long, contain one uppercase letter, one lowercase letter, one digit, and one special character', 'warning');
            return false;
        }

        if (ConfirmPassword !== Password) {
            Swal.fire('Warning', 'Confirm password and password do not match', 'warning');
            return false;
        }

        // Prepare form data
        var formdata = new FormData();
        formdata.append('UserName', UserName);
        formdata.append('Email', EmailAddress); // <- changed
        formdata.append('FirstName', FirstName);
        formdata.append('LastName', LastName);
        formdata.append('PhoneNumber', PhoneNumber);
        formdata.append('Password', Password);
        formdata.append('ConfirmPassword', ConfirmPassword);


        // AJAX request
        $.ajax({
            url: "https://localhost:7134/api/Authenticate/SignUp",
            type: "POST",
            data: formdata,
            contentType: false,
            processData: false,
            success: function (response) {
                console.log(response);
                Swal.fire('Success', 'User Created Successfully', 'success').then(() => {
                    $("#UserName, #EmailAddress, #FirstName, #LastName, #PhoneNumber, #Password, #ConfirmPassword").val(""); // reset form
                });
            },
            error: function (error) {
                console.warn(error);
                Swal.fire('Error', 'Something went wrong', 'error');
            }
        });
    });

    // Email validation
    function validateEmail(EmailAddress) {
        var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regex.test(EmailAddress);
    }

    // Password validation
    function validatePassword(Password) {
        var regex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
        return regex.test(Password);
    }

});
