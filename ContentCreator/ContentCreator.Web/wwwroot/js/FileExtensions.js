$(document).ready(function () {
    let clickedFileType = null;
    GetFileTypeList()
    function GetFileTypeList() {
        $("#tblFileTypeListBody").html(``);
        var selectedFileType = $("#fileTypeSelect").val();
        $.ajax({
            url: "https://localhost:7134/" + "api/General/GetFileTypeList",
            type: "GET",
            data: { filterFileType: clickedFileType }, 
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
                        var isSelected = data.FileType === selectedFileType ? "selected" : "";
                        optionHtml += `<option value="${data.FileType}" ${isSelected}>${data.FileType}</option>`;
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
        var isActive = $("input[name='isActive']:checked").val(); 

        var formData = new FormData();
        formData.append('FileType', fileType);
        formData.append('FileExtension', fileExtension);
        formData.append('MinimumSize', minimumSize);
        formData.append('MaximumSize', maximumSize);
        formData.append('IsActive', isActive);
        
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
        var fileTypeId = $(this).val();
        $("#fileExtensionInput").val('');
        $("#minimumSizeInput").val('');
        $("#maximumSizeInput").val('');
        if (fileTypeId) {
            clickedFileType = fileTypeId;
            $("#btnAddFileExtension, #isActiveYes, #isActiveNo").attr("disabled", false)
            $("#fileExtensionInput, #minimumSizeInput, #maximumSizeInput, #btnAddFileExtension").attr("readonly", false);
            GetFileTypeList()
        }
        else {
            clickedFileType = null;
            GetFileTypeList()
            $("#btnAddFileExtension, #isActiveYes, #isActiveNo").attr("disabled", true)
            $("#fileExtensionInput, #minimumSizeInput, #maximumSizeInput").attr("readonly", true);
        }
    })



    //Validations...
    $(document).on('keypress', '.block-spacebar', function (e) {
        if (e.which === 32) {
            e.preventDefault(); // Block spacebar
        }
    });

    // Remove spaces on paste
    $(document).on('paste', '.block-spacebar', function (e) {
        e.preventDefault();
        let pastedData = e.originalEvent.clipboardData.getData('text');
        let cleaned = pastedData.replace(/\s+/g, ''); // Remove all spaces
        document.execCommand('insertText', false, cleaned);
    });

    $(document).on("keypress paste", ".numOnly", function (e) {
        if (e.type === "keypress" && (e.which < 48 || e.which > 57)) e.preventDefault();
        if (e.type === "paste" && !/^\d+$/.test((e.originalEvent || e).clipboardData.getData('text'))) e.preventDefault();
    });

    $("#fileExtensionInput").on("keypress", function (e) {
        let val = this.value;
        let char = e.key;

        if (val.length >= 5) e.preventDefault();
        if (char === "." && val.includes(".")) e.preventDefault();
        if (val.length === 0 && char !== ".") e.preventDefault();
        if (!/^[a-zA-Z0-9.]$/.test(char)) e.preventDefault();
    });


});