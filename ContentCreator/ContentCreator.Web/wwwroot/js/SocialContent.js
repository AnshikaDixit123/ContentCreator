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
                                <i class="fa-regular fa-comment postComment" data-postid="${data.PostId}" style="cursor:pointer;"></i>
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

    $(document).on("click", ".postLikes", function () {
        const $icon = $(this);
        var postId = $icon.data("postid");
        const $likeCount = $icon.closest(".post-actions").find(".like-count");;
        let currentLikes = parseInt($likeCount.text()) || 0;
        const isLiked = $icon.hasClass("fa-solid");

        const formdata = new FormData();
        formdata.append("PostId", postId);
        formdata.append("UserId", userId);
        formdata.append("IsLiked", !isLiked); // toggle state

        $.ajax({
            url: "https://localhost:7134/api/Content/PostLikes",
            type: "POST",
            data: formdata,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    if (!isLiked) {
                        $icon.removeClass("fa-regular")
                            .addClass("fa-solid")
                            .css("color", "red");
                        $likeCount.text(currentLikes + 1);
                    } else {
                        $icon.removeClass("fa-solid")
                            .addClass("fa-regular")
                            .css("color", "");
                        $likeCount.text(Math.max(0, currentLikes - 1));
                    }

                } else {
                    Swal.fire("Error", "Post couldn't be liked", "error");
                }
            },
            error: function (error) {
                console.warn(error);
                Swal.fire("Error", "Something went wrong", "error");
            }
        });
    });
    $(document).on("click", ".postComment", function () {
        var postId = $(this).data("postid");
        $("#commentModal").modal("show"); 
        $("#commentModal").attr("data-postid", postId);
        GetComments(postId)
    })
    $(document).on("click", "#postCommentBtn", function () {
        debugger
        var postId = $("#commentModal").attr("data-postid");
        var comment = $("#newComment").val();
        parentId = $(this).data("parentid") || null;

        var formData = new FormData();
        formData.append("PostId", postId);
        formData.append("UserId", userId);
        formData.append("Comment", comment);
        if (parentId) {
            formData.append("ParentId", parentId);
        }

        $.ajax({
            url: "https://localhost:7134/api/Content/PostComments",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    $("#newComment").val(""); 
                } else {
                    Swal.fire("Error", response.Message, "error");
                }
            },
            error: function (error) {
                console.warn(error);
                Swal.fire("Error", "Something went wrong", "error");
            }
        })
    })
    //function GetComments(postId) {
    //    $.ajax({
    //        url: `https://localhost:7134/api/Content/GetComments?postId=${postId}`,
    //        type: "GET",
    //        success: function (response) {
    //            if (response.StatusCode === 200 && response.Result.length > 0) {
    //                $("#commentsList").empty();

    //                response.Result.forEach(function (comment) {
    //                    if (comment.ParentId == null) {
    //                        var html = `
    //                        <div class="comment mb-3">
    //                            <div class="d-flex align-items-center mb-1">
    //                                <img src="https://i.pravatar.cc/30?u=${comment.UserId}" 
    //                                     class="rounded-circle me-2" width="30" height="30" />
    //                                <strong>${comment.UserName || "User"}</strong>
    //                                <small class="text-muted ms-2">${comment.CreatedAt || ""}</small>
    //                            </div>
    //                            <p class="mb-1 ms-4">${comment.Comment}</p>
    //                            <button class="btn btn-sm btn-link text-secondary p-0 ms-4 replyBtn" 
    //                                    data-commentid="${comment.Id}">Reply</button>
    //                        </div>`;
    //                        $("#commentsList").append(html);
    //                    }
    //            } else {

    //            }
    //        },
    //        error: function (error) {
    //            console.warn(error);
    //            Swal.fire("Error", "Failed to load comments", "error");
    //        }
    //    });
    //}
});
