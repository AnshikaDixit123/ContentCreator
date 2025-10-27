$(document).ready(function () {
    const userId = localStorage.getItem("UserId");   

    GetPost();

    function GetPost() {
        var rawHtml = "";

        $.ajax({
            url: "https://localhost:7134/api/Content/GetPost?userId=" + userId,
            type: "GET",
            success: function (response) {
                if (response.StatusCode === 200 && response.Result?.length > 0) {
                    var posts = response.Result;
                    for (var i = 0; i < posts.length; i++) {
                        var data = posts[i];
                        var isLiked = data.IsLiked === true;
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
                                ${data.Media
                                ? `
                                    <img src="${data.Media}" alt="Post Image" class="post-image"
                                         onerror="this.style.display='none'; this.nextElementSibling.style.display='block';">
                                    <video controls preload="metadata" class="post-video" style="display:none;">
                                        <source src="${data.Media}" type="video/mp4">
                                        Your browser does not support the video tag.
                                    </video>`
                                : ""
                            }
                            </div>

                            <div class="post-actions">
                                <i class="${isLiked ? "fa-solid" : "fa-regular"} fa-heart postLikes" data-postid="${data.PostId}" style="cursor:pointer;; color:${isLiked ? "red" : ""};"></i>
                                <span class="like-count">${data.LikeCount || 0}</span>
                                <i class="fa-regular fa-comment"></i>
                                <i class="fa-regular fa-paper-plane"></i>
                            </div>
                        </div>`;
                    }

                    $("#postSection").html(rawHtml);
                } else {
                    $("#postSection").html("<p style='text-align:center;color:#777;'>No posts found.</p>");
                }
            },
            error: function (error) {
                console.warn(error);
                Swal.fire("Error", "Could not load posts", "error");
            },
        });
    }

    // Handle like click
    $(document).on("click", ".postLikes", function () {
        const $icon = $(this);
        const postId = $icon.data("postid");
        const $likeCount = $icon.siblings(".like-count");
        let currentLikes = parseInt($likeCount.text()) || 0;
        const isLiked = $icon.hasClass("fa-solid");
       
        const formdata = new FormData();
        formdata.append("PostId", postId);
        formdata.append("UserId", userId);
        formdata.append("IsLiked", !isLiked); // toggle state

        $.ajax({
            url: "https://localhost:7134/" + "api/Content/PostLikes",
            type: "POST",
            data: formdata,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {                    
                    if (!isLiked) {
                        $icon.removeClass("fa-regular").addClass("fa-solid").css("color", "red");
                        $likeCount.text(currentLikes + 1);
                    } else {
                        $icon.removeClass("fa-solid").addClass("fa-regular").css("color", "");
                        $likeCount.text(currentLikes - 1 > 0 ? currentLikes - 1 : 0);
                    }
                } else {
                    Swal.fire("Error", "Post couldn't be liked", "error");
                }
            },
            error: function (error) {
                console.warn(error);
                Swal.fire("Error", "Something went wrong", "error");
            },
        });
    });
});
