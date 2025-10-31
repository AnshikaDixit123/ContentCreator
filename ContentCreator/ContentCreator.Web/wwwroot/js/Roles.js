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
                        var tableData = `
                            <tr data-roleId="${data.Id}">
                              <td>
                                <div class="d-flex align-items-center gap-2">
                                  <span>${i + 1}</span>
                                  <button class="btn btn-sm btn-light get-details"
                                      type="button"
                                      data-bs-toggle="collapse"
                                      data-bs-target="#details-${data.Id}"
                                      aria-expanded="false"
                                      aria-controls="details-${data.Id}">
                                    <span class="arrow">▶</span>
                                  </button>
                                </div>
                              </td>
                              <td>${data.RoleName || ''}</td>
                              <td>${data.UserCount || ''}</td>
                              <td>${data.RoleDescription || ''}</td>
                              <td>${data.RoleType || ''}</td>
                              <td>${data.IsProtected}</td>
                              <td>${btn}</td>
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

    $(document).on('click', '.get-details', function () {
        const $clickedBtn = $(this);
        const $clickedArrow = $clickedBtn.find('.arrow');
        const $tr = $clickedBtn.closest('tr');
        const roleId = $tr.data('roleid');

        // Remove any existing detail rows first
        $('.detail-row').remove();

        if ($clickedArrow.text() === '▶') {
            // Reset all arrows to right
            $('.arrow').text('▶');

            // Set clicked arrow down
            $clickedArrow.text('▼');

            // 🔹 Call the API to get assigned extensions
            $.ajax({
                url: "https://localhost:7134/api/General/GetAssignedExtensionData",
                type: "GET",
                data: { RoleId: roleId },
                success: function (response) {
                    console.log("API Response:", response); // 🔹 debug

                    let extensions = [];

                    // 1️⃣ Check where the array is in response
                    if (Array.isArray(response.Result)) {
                        extensions = response.Result;
                    } else if (response.Result && Array.isArray(response.Result.Extensions)) {
                        extensions = response.Result.Extensions;
                    }

                    if (extensions.length === 0) {
                        // No extensions found — show message
                        const noDataHtml = `
                        <tr class="detail-row">
                            <td colspan="7">
                                <div style="padding: 10px; background: #f9f9f9; color: #777;">
                                    No extensions found for this role.
                                </div>
                            </td>
                        </tr>
                    `;
                        $tr.after(noDataHtml);
                        return;
                    }

                    // 2️⃣ Build HTML safely
                    let detailsHtml = '';
                    extensions.forEach(ext => {
                        // Handle both camelCase or PascalCase
                        const name = ext.Extension || ext.extension || "Unknown";
                        const isActive = (ext.IsActive !== undefined) ? ext.IsActive : (ext.isActive || false);


                        detailsHtml += `
                        <tr class="detail-row">
                            <td colspan="7">
                                <div style="padding: 10px; background: #f9f9f9; display: flex; justify-content: space-between;">
                                    <span>${name}</span>
                                    <span style="color: ${isActive ? 'green' : 'red'};">
                                        ${isActive ? 'Active' : 'Inactive'}
                                    </span>

                                </div>
                            </td>
                        </tr>
                    `;
                    });

                    // Add all details below the clicked row
                    $tr.after(detailsHtml);
                },
                error: function (error) {
                    console.error("Error fetching assigned extensions:", error);
                    const errorHtml = `
                    <tr class="detail-row">
                        <td colspan="7">
                            <div style="padding: 10px; background: #ffecec; color: #d00;">
                                Failed to load extensions. Please try again later.
                            </div>
                        </td>
                    </tr>
                `;
                    $tr.after(errorHtml);
                }
            });
        } else {
            // Collapse arrow back to right
            $clickedArrow.text('▶');
        }
    });

});