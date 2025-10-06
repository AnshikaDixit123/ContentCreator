$(document).ready(function () {
    let clickedRoleId = null;
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
                        var btn = ``;
                        if (data.IsExtensionNeeded) {
                            btn = `<button type="button" class="btn btn-success addExtension" 
                            data-bs-toggle="modal" data-bs-target="#createSubscriptionModal">Assign</button>`;
                        }
                        var tableData = `<tr data-roleId = ${data.Id}>
                             <td>
                                <div class="d-flex align-items-center gap-2">
                                    <span>${i + 1}</span>
                                    <button class="btn btn-sm btn-light toggle-detailstoggle-details"
                                            data-bs-toggle="collapse"
                                            data-bs-target="#details-${data.Id}"
                                            aria-expanded="false">▼</button>
                                </div>
                            </td>
                            <td>${data.RoleName || ''}</td>
                            <td>${data.UserCount || ''}</td>
                            <td>${data.RoleDescription || ''}</td>
                            <td>${data.RoleType || ''}</td>
                            <td>${data.IsProtected}</td>
                            <td>${btn}</td>
                        </tr>
                        <tr class="collapse bg-light" id="details-${data.Id}">
                            <td colspan="7">
                                <div class="p-3 text-secondary"><table class="table table-sm table-bordered mb-0">
                                    <tbody>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>&nbsp;</td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>&nbsp;</td>
                                        </tr>
                                    </tbody>
                                </table></div>
                            </td>
                        </tr>
                    `; 
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
    function GetExtensionList(clickedRoleId) {
        $.ajax({
            url: "https://localhost:7134/" + "api/General/GetExtensionList?RoleId=" + clickedRoleId,
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    console.log(response);
                    var optionHtml = ` `;
                    var recordExtensions = response.Result.length;
                    for (var i = 0; i < recordExtensions; i++) {
                        var data = response.Result[i];
                        optionHtml += `<option value="${data.Id}">${data.FileExtension}</option>`;
                    }
                    $("#multiple-select").html(optionHtml);
                    $('#multiple-select').select2({
                        placeholder: "Please select...",
                        allowClear: true,   // adds a clear (x) button
                        tags: true          // enable creating new tags

                    });
                }
                else {
                    console.log("Error fetching extensions");
                }
            },
            error: function (error) {
                console.warn(error);
            }
        });
    }
    
    $(document).on("click", ".addExtension", function () {
        var roleId = $(this).closest("tr").data("roleid");
        if (roleId) {
            clickedRoleId = roleId;
            GetExtensionList(clickedRoleId)
        }
    });
    $(document).on("click", "#addBtn", function () {
        var roleId = clickedRoleId;
        var fileTypeIds = $("#multiple-select").val();

        var formData = new FormData();
        formData.append('RoleId', roleId);
        if (fileTypeIds && fileTypeIds.length > 0) {
            for (var i = 0; i < fileTypeIds.length; i++) {
                formData.append('FileTypeId', fileTypeIds[i]);
            }
        }
        $.ajax({
            url: "https://localhost:7134/" + "api/General/AssignExtensions",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    Swal.fire('Successful', response.Message, 'success');                    
                }
                else {
                    Swal.fire('Warning', response.Message, 'error');
                }

            },
            error: function (error) {
                console.warn(error)
            }
        })
    });
    $(document).on("click", ".toggle-details", function () {      
        var roleId = $(this).closest("tr").data("roleid");
        $.ajax({
            url: "https://localhost:7134/" + "api/General/GetAssignedExtensionData",
            type: "GET",
            data: { RoleId: roleId }, 
            success: function (response) {
                if (response.StatusCode == 200) {
                    Swal.fire('Successful', response.Message, 'success');
                }
                else {
                    Swal.fire('Warning', response.Message, 'error');
                }

            },
            error: function (error) {
                console.warn(error)
            }
        })
    })
});