$(document).ready(function () {

    GetRolesList()
    function GetRolesList() {
        $('#tblRolesListBody').html(``)
        $.ajax({
            url: "https://localhost:7134/" + "api/General/GetRoleList",
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log(response);
                    var recordRoles = response.Result.length;
                    for (var i = 0; i < recordRoles; i++) {
                        var data = response.Result[i];
                        var tableData = `<tr>
                            <td value="${data.Id}">${i + 1}</td>
                            <td>${data.RoleName}</td>
                            <td>${data.UserCount}</td>
                            <td>${data.RoleDescription}</td>
                            <td>${data.RoleType}</td>
                            <td>${data.IsProtected}</td>
                            <td><span class="badge bg-success">Assign</span></td>
                        </tr>`
                        $('#tblRolesListBody').append(tableData);
                    }
                }
                else {
                    console.log("error");
                }
            },
            error: function (error) {
                console.warn(error);
            }
        });
    }
})