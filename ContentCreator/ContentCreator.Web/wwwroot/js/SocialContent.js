$(document).ready(function () {
    const userId = localStorage.getItem("UserId");
     GetPost();
    function GetPost() {
        var rawHtml = "";

    $.ajax({
        url: "https://localhost:7134/api/Content/GetPost",
        type: "GET",
        success: function (response) {
                    if (response.StatusCode === 200 && response.Result?.length > 0) {
                        var posts = response.Result;

        for (var i = 0; i < posts.length; i++) {
                            var data = posts[i];

        rawHtml += `
        <div class="post-card">
            <div class="post-header">
                <img src="/image/app-avatar-default.png" alt="User" class="post-avatar">
                    <span class="post-username">${data.UserName || "Unknown User"}</span>
            </div>
            <div class="post-caption">
                ${data.PostDescription || ""}
            </div>
            <div class="post-media">
                ${data.Media ? `
        <img src="${data.Media}" alt="Post Image" class="post-image" 
             onerror="this.style.display='none'; this.nextElementSibling.style.display='block';">
        <video controls preload="metadata" class="post-video" style="display:none;">
            <source src="${data.Media}" type="video/mp4">
            Your browser does not support the video tag.
        </video>
    ` : ""}
            </div>
            <div class="post-actions">
                <i class="fa-regular fa-heart" id="postLikes" style="cursor:pointer;"></i>
                <span class="like-count">${data.LikeCount}</span>
                <i class="fa-regular fa-comment"></i>
                <i class="fa-regular fa-paper-plane"></i>
            </div>
        </div>`;
                        }

    // Append all posts at once
    $('#postSection').html(rawHtml);
                } else {
        $('#postSection').html("<p style='text-align:center;color:#777;'>No posts found.</p>");
                }
        },

    error: function (error) {
        console.warn(error);
    Swal.fire('Error', 'Could not load posts', 'error');
            }
        });
    }

    $(document).on("click", "#postLikes", function () {
        $(this).toggleClass("liked");
        var postId = $icon.data("postid");
        if ($(this).hasClass("liked")) {
            $(this).removeClass("fa-regular").addClass("fa-solid");

            var formdata = new FormData();
            formdata.append("PostId", postId);
            formdata.append("UserId", userId);
            formdata.append("IsLiked", true);

            $.ajax({
                url: "https://localhost:7134/" + "api/Content/PostLikes",
                type: "POST",
                data: formdata,
                contentType: false,
                processData: false,
                success: function (response) {
                    if (response.StatusCode == 200) {
                        console.log("Post liked successfully");
                    }
                    else {
                        Swal.fire('Error',"Post Couldn't be liked", 'error');
                    }
                },
                error: function (error) {

                    console.warn(error)
                    Swal.fire('Error', response.Message, 'error');
                }
            });
        }
        else {
            $(this).removeClass("fa-solid").addClass("fa-regular");
        }
    });
});
