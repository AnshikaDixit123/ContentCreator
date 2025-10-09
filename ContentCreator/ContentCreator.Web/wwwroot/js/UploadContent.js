$(document).ready(function () {
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
        if (mediaFile) formData.append("File", mediaFile);
        formData.append("IsPublic", visibility);
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
                } else {
                    Swal.fire('Error', response.Message, 'error');
                }
            },
            error: function (xhr) {
                console.error(xhr);
            }
        });

    })
})