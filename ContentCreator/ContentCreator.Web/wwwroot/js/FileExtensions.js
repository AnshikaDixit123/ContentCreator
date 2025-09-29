$(document).ready(function () {
    let emptyGuid = "00000000-0000-0000-0000-000000000000";
    let clickedFileType = emptyGuid;
    GetFileTypeList()
    function GetFileTypeList() {
        $("#tblFileTypeListBody").html(``);
        $.ajax({
            url: "https://localhost:7134/" + "api/General/GetFileTypeList",
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200) {
                    var recordFileType = response.Result.FileTypeDetailList.length;
                    for (var i = 0; i < recordFileType; i++) {
                        var data = response.Result.FileTypeDetailList[i];
                        var tableData = `<tr>
                            <td>${i + 1}</td>
                            <td>${data.FileType}</td>                                             
                            <td>${data.FileExtension}</td>                                             
                            <td>${data.MinimumSize}</td>                                             
                            <td>${data.MaximumSize}</td>                                             
                            <td>${data.IsActive}</td>                                             
                        </tr>`
                        $('#tblFileTypeListBody').append(tableData);
                    }
                    // for dropdown
                    var optionHtml = `<option value="">Select File Type</option>`;
                    var recordOnlyFileType = response.Result.FileTypeList.length;
                    for (var i = 0; i < recordOnlyFileType; i++) {
                        var data = response.Result.FileTypeList[i];
                        optionHtml += `<option value="${data.FileType}">${data.FileType}</option>`;
                    }
                    $('#fileTypeSelect').html(optionHtml);
                }
                else {
                    console.log("Error in getting FileTypeList")
                }
            },
            error: function (error) {
                console.warn("Fail in getting FileTypeList");
            }
        });
    }
    $(document).on("click", "#btnAddFileExtension", function () {
        var fileType = $("#fileTypeSelect").val();
        var fileExtension = $("#fileExtensionInput").val();
        var minimumSize = $("#minimumSizeInput").val();
        var maximumSize = $("#maximumSizeInput").val();

        var formData = new FormData();
        formData.append('FileType', fileType);
        formData.append('FileExtension', fileExtension);
        formData.append('MinimumSize', minimumSize);
        formData.append('MaximumSize', maximumSize);
        
        $.ajax({
            url: "https://localhost:7134/" + "api/General/AddExtension",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    Swal.fire('Successful', response.Message, 'success');
                    GetFileTypeList();
                }
                else {
                    Swal.fire('Warning', response.Message, 'error');
                }

            },
            error: function (error) {
                console.warn("Failed the process of adding extension");
            }
        });       
    });
    $(document).on("change", "#fileTypeSelect", function () {
        
    })
});