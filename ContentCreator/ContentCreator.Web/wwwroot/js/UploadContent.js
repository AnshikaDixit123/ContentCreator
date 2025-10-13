$(document).ready(function () {
    const roleId = localStorage.getItem("RoleId");
    let allowedExtensions = [];
    var isStaticFileNeeded = true;
    if (roleId && roleId.trim().toUpperCase() === "4FF01FAB-90B3-4D90-EF35-08DE0A1A8CAA") {
        $("#mediaSection").addClass("d-none");
        isStaticFileNeeded = false;
    } else {
        $("#mediaSection").removeClass("d-none");
    }
    $(document).on("click", "#btnforPost", function () {
        var userId = localStorage.getItem("UserId"); 
        var postDescription = $("#postContent").val();
        var mediaFile = $("#mediaFile")[0].files[0];
        var visibility = $("#visibility").val();

        //var IsPublic = false;
        //var IsPrivate = false;
        //var IsSubscribed = false;
        //if (visibility == "public") {
        //    IsPublic = true
        //} else if (visibility == "private") {
        //    IsPrivate = true
        //} else {
        //    IsSubscribed = true
        //}
        var formData = new FormData();
        formData.append('UserId', userId);
        formData.append('PostDescription', postDescription);
        if (mediaFile && isStaticFileNeeded) formData.append("File ", mediaFile);
        formData.append("IsStaticFileNeeded", isStaticFileNeeded); 
        formData.append("Visibility", visibility);
        //formData.append("IsPublic", IsPublic);
        //formData.append("IsPrivate", IsPrivate);
        //formData.append("IsSubscribed", IsSubscribed);

        $.ajax({
            url: "https://localhost:7134/api/Content/UploadAPost",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode === 200) {
                    Swal.fire('Success', response.Message, 'success');  
                    $("#postForm")[0].reset(); // clears textarea, select, file input
                    $("#preview").html(""); 
                } else {
                    Swal.fire('Error', response.Message, 'error');
                    $("#postForm")[0].reset(); // clears textarea, select, file input
                    $("#preview").html(""); 
                }
            },
            error: function (xhr) {
                console.error(xhr);
            }
        });
    })
    fetchAllowedExtensions();
    function fetchAllowedExtensions() {
        if (roleId && roleId.trim().toUpperCase() === "4FF01FAB-90B3-4D90-EF35-08DE0A1A8CAA") {
            return;
        }
        $.ajax({
            url: "https://localhost:7134/" + "api/Content/GetAllowedExtensionToCreator?RoleId=" + roleId,
            type: "GET",
            success: function (response) {
                if (response.StatusCode == 200 && response.Result) {
                    allowedExtensions = response.Result;
                }
                else {
                    Swal.fire('Error', response.Message, 'error');
                }
            },
            error: function (error) {
                console.error(error);
            }
        })
    }

    $(document).on("change", "#mediaFile", function () {
        const file = this.files[0];
        const fileName = file.name.toLowerCase();
        const matchedExt = allowedExtensions.find(ext => fileName.endsWith(ext.FileExtension.toLowerCase()));
        const fileSizeKB = file.size / 1024;

        if (!matchedExt) {
            Swal.fire('Error', `This file type is not allowed!`, 'error');
            $(this).val("");
            return;
        }
        if (fileSizeKB < matchedExt.MinimumSize || fileSizeKB > matchedExt.MaximumSize) {
            Swal.fire('Error', `File size should be between ${matchedExt.MinimumSize} KB and ${matchedExt.MaximumSize} KB`, 'error');
            $(this).val("");
            return;
        }
        var reader = new FileReader();
        reader.onload = function (e) {
            var fileData = e.target.result;
            if (file.type.startsWith("image/")) {
                $("#preview").html(`<img src="${fileData}" alt="Preview" style="max-width:300px; max-height:300px;">`);
            }
            else if (file.type.startsWith("video/")) {
                $("#preview").html(`<video controls style="max-width:300px; max-height:300px;">
                                    <source src="${fileData}" type="${file.type}">
                                    Your browser does not support the video tag.
                                </video>`);
            }
            else {
                $("#preview").html("<p>Preview not available for this file type.</p>");
            }
        };

        reader.readAsDataURL(file);
    })
})