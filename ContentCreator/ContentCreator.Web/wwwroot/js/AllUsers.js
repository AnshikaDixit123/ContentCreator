$(document).ready(function () {
    const jwtToken = localStorage.getItem('UserToken');

    GetUserList()
    function GetUserList() {
        $('#tblUserListBody').html(``)
        var url = "https://localhost:7134/" + "api/General/GetUserList";
        $.ajax({
            url: url,
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var recordUser = response.Result.length;
                    for (var i = 0; i < recordUser; i++) {
                        var data = response.Result[i];
                        var statusType = data.IsActive ? "Active" : "In-Active";
                        var tableData = `<tr data-user-id="${data.UserId}" data-status="${statusType}" data-role="${data.Role}">
                            <td>${i + 1}</td>
                            <td><img src="${data.ProfileImage}" class="rounded-circle" width="40" height="40"></td>
                            <td>${data.FullName}</td>
                            <td>${data.Email}</td>
                            <td>${data.PhoneNumber}</td>
                            <td>${data.Role}</td>
                            <td><span class="badge bg-success">View Details</span></td>
                        </tr>`
                        $('#tblUserListBody').append(tableData)
                    }
                    $('#tblUserList').DataTable;
                }
                else {
                    Swal.fire('warning', 'something went wrong', 'warning');
                }
            },
            error: function (error) {
                console.warn(error)
            }
        });
        $(document)

    }
});