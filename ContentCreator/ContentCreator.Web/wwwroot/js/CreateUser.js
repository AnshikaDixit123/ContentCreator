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
})