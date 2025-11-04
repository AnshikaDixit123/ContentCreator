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
        //alert(postId)
        $("#commentModal").attr("data-postid", postId);
        GetComments(postId)
    })
    $(document).on("click", "#postCommentBtn", function () {
        var postId = $("#commentModal").attr("data-postid");
        var comment = $("#newComment").val();
        parentId = $(this).data("parentid") || null;

        var formData = new FormData();
        formData.append("PostId", postId);
        formData.append("UserId", userId);
        formData.append("Comment", comment);
        formData.append("ParentId", null);

        $.ajax({
            url: "https://localhost:7134/api/Content/PostComments",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.StatusCode == 200) {
                    $("#newComment").val("");
                    GetComments(postId);
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
    function GetComments(postId) {
        $("#commentsList").empty();

        $.ajax({
            url: `https://localhost:7134/api/content/getcomments?postId=${postId}`,
            type: "GET",
            success: function (response) {
                console.log("Response from API:", response);

                if (!response || !response.IsSuccess) {
                    const noDataHtml = `
                <div style="padding: 10px; background: #f9f9f9; color: #777; text-align: center;">
                    ${response?.Message || 'No comments found for this post.'}
                </div>`;
                    $("#commentsList").append(noDataHtml);
                    return;
                }

                const comments = Array.isArray(response.Result) ? response.Result : [];

                if (comments.length === 0) {
                    const noDataHtml = `
                <div style="padding: 10px; background: #f9f9f9; color: #777; text-align: center;">
                    No comments found for this post.
                </div>`;
                    $("#commentsList").append(noDataHtml);
                    return;
                }

                let html = "";
                comments.forEach(cmt => {
                    const commentId = cmt.Id || "";
                    // Use UserName instead of UserId
                    const userName = cmt.UserName || "Unknown User";

                    const text = cmt.Comment || "(No text)";
                    const date = cmt.CommentedAt
                        ? new Date(cmt.CommentedAt).toLocaleString()
                        : "Unknown date";

                    html += `
    <div class="comment mb-3" data-comment-id="${commentId}" style="max-width: 600px; border: 1px solid #e0e0e0; border-radius: 8px; padding: 12px; background: #f8f9fa; margin: 0 auto; box-shadow: 0 1px 3px rgba(0,0,0,0.05);">
        <div style="display:flex; justify-content:space-between; align-items:flex-start;">
            <strong style="color: #333;">${userName}</strong>
            <span style="font-size: 12px; color: #6c757d;">${date}</span>
        </div>
        <div style="margin-top: 8px; margin-bottom: 12px; color: #495057; line-height: 1.4;">${text}</div>
        
        <div style="display: flex; gap: 12px; align-items: center;">
            <button class="reply-btn" style="background: none; border: none; color: #65676b; font-weight: 600; padding: 4px 8px; cursor: pointer; border-radius: 4px; font-size: 13px;">
                Reply
            </button>
            <button class="view-replies-btn" style="background: none; border: none; color: #007bff; font-weight: 600; padding: 4px 8px; cursor: pointer; border-radius: 4px; font-size: 13px;">
                View Replies
            </button>
        </div>
        
        <!-- Replies Section -->
        <div class="replies-container" style="margin-top: 12px; display: none;">
            <div class="replies-list"></div>
            
            <!-- Reply Input -->
            <div class="reply-input mt-2">
                <input type="text" class="reply-text" placeholder="Write a reply..." style="width: 100%; padding: 8px; border: 1px solid #ddd; border-radius: 4px; font-size: 13px;">
                <div style="margin-top: 8px; display: flex; gap: 8px;">
                    <button class="post-reply-btn" style="background: #007bff; color: white; border: none; padding: 6px 12px; border-radius: 4px; font-size: 12px; cursor: pointer;">
                        Post Reply
                    </button>
                    <button class="cancel-reply-btn" style="background: #6c757d; color: white; border: none; padding: 6px 12px; border-radius: 4px; font-size: 12px; cursor: pointer;">
                        Cancel
                    </button>
                </div>
            </div>
        </div>
    </div>
`;
                }); 
                $("#commentsList").append(html);
            },
            error: function (xhr, status, error) {
                console.error("Error loading comments:", error);
                Swal.fire("Error", "Failed to load comments", "error");
            }
        });
    }
    $(document).on("click", ".reply-btn", function () {
        const $comment = $(this).closest('.comment');
        const $repliesContainer = $comment.find('.replies-container');
        const commentId = $comment.data('comment-id');

        if ($repliesContainer.is(':hidden')) {
            $repliesContainer.show();
            loadReplies(commentId, $comment);
            $comment.find('.reply-text').focus();
        } else {
            $repliesContainer.hide();
        }
    })
    $(document).on("click", ".post-reply-btn", function () {
        const $comment = $(this).closest('.comment');
        const replyText = $comment.find('.reply-text').val().trim();
        const commentId = $comment.data('comment-id');

        if (replyText) {
            postReply(commentId, replyText, $comment);
        } else {
            Swal.fire("Warning", "Please enter a reply", "warning");
        }
    });
    async function loadReplies(commentId, $comment) {
        try {
            const $repliesList = $comment.find('.replies-list');
            $repliesList.html('<div style="text-align: center; color: #6c757d; padding: 10px;">Loading replies...</div>');

            // You'll need to create this endpoint in your API
            const response = await $.ajax({
                url: `https://localhost:7134/api/content/getreplies?commentId=${commentId}`,
                method: 'GET'
            });

            if (response.isSuccess && response.result && response.result.length > 0) {
                let repliesHtml = '';
                response.result.forEach(reply => {
                    const replyDate = reply.CommentedAt
                        ? new Date(reply.CommentedAt).toLocaleString()
                        : "Unknown date";

                    repliesHtml += `
                        <div class="reply mb-2" style="padding: 8px; background: white; border-radius: 6px; border: 1px solid #e9ecef;">
                            <div style="display:flex; justify-content:space-between; align-items:flex-start;">
                                <strong style="color: #333; font-size: 14px;">${reply.UserName || "Unknown User"}</strong>
                                <span style="font-size: 11px; color: #6c757d;">${replyDate}</span>
                            </div>
                            <div style="margin-top: 4px; color: #495057; font-size: 13px; line-height: 1.3;">${reply.Comment}</div>
                        </div>
                    `;
                });
                $repliesList.html(repliesHtml);
            } else {
                $repliesList.html('<div style="text-align: center; color: #6c757d; padding: 10px;">No replies yet</div>');
            }
        } catch (error) {




            console.error('Error loading replies:', error);
            $comment.find('.replies-list').html('<div style="text-align: center; color: #dc3545; padding: 10px;">Error loading replies</div>');
        }
    }
    async function postReply(commentId, replyText, $comment) {
        try {
            $comment.find('.post-reply-btn').prop('disabled', true).text('Posting...');

            const formData = new FormData();
            formData.append("PostId", currentPostId);
            formData.append("UserId", userId);
            formData.append("Comment", replyText);
            formData.append("ParentId", commentId); // This makes it a reply

            const response = await $.ajax({
                url: "https://localhost:7134/api/Content/PostComments",
                type: "POST",
                data: formData,
                contentType: false,
                processData: false
            });

            if (response.StatusCode == 200) {
                $comment.find('.reply-text').val('');
                await loadReplies(commentId, $comment);
                Swal.fire("Success", response.Message, "success");
            } else {
                Swal.fire("Error", response.Message, "error");
            }
        } catch (error) {
            console.error('Failed to post reply:', error);
            Swal.fire("Error", "Failed to post reply", "error");
        } finally {
            $comment.find('.post-reply-btn').prop('disabled', false).text('Post Reply');
        }
    }
});