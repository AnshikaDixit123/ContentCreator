$(document).ready(function () {
    const userId = localStorage.getItem("UserId");  
    GetPost();
    function GetPost() {
        var rawHtml = "";

        $.ajax({
            url: "https://localhost:7134/api/Content/GetPost?userId=" + userId,
            type: "GET",
            success: function (response) {
                console.log("API Response:", response); // Debug log
                if (response.StatusCode === 200 && response.Result?.length > 0) {
                    var posts = response.Result;
                    for (var i = 0; i < posts.length; i++) {
                        var data = posts[i];
                        var isLiked = data.IsLiked === true;

                        if (data.IsReshared && data.OriginalPost) {
                            // This is a reshared post - show nested cards
                            rawHtml += `
                        <div class="post-card" style="border: 1px solid #ddd; border-radius: 12px; margin: 15px 0; padding: 15px; background: white;">
                            <!-- Reshare Header -->
                            <div class="post-header" style="display: flex; align-items: center; margin-bottom: 15px;">
                                <img src="/image/app-avatar-default.png" alt="User" class="post-avatar" style="width: 40px; height: 40px; border-radius: 50%; margin-right: 12px;">
                                <div>
                                    <span class="post-username" style="font-weight: bold; font-size: 16px;">${data.UserName}</span>
                                    <div style="font-size: 13px; color: #666; display: flex; align-items: center;">
                                        <i class="fa-solid fa-retweet" style="color: #1d9bf0; margin-right: 6px;"></i>
                                        Shared
                                    </div>
                                </div>
                            </div>

                            <!-- Original Post Card (nested inside) -->
                            <div class="original-post-container" style="border: 1px solid #e0e0e0; border-radius: 8px; padding: 12px; background: #fafafa; margin: 10px 0;">
                                <!-- Original Poster Header -->
                                <div class="post-header" style="display: flex; align-items: center; margin-bottom: 10px;">
                                    <img src="/image/app-avatar-default.png" alt="User" class="post-avatar" style="width: 32px; height: 32px; border-radius: 50%; margin-right: 10px;">
                                    <span class="post-username" style="font-weight: 500; font-size: 14px;">${data.OriginalPost.UserName}</span>
                                </div>

                                <!-- Original Post Description -->
                                <div class="post-caption" style="font-size: 14px; color: #333; margin-bottom: ${data.OriginalPost.Media ? '10px' : '0'};">
                                    ${data.OriginalPost.PostDescription || ""}
                                </div>

                                <!-- Original Post Media -->
                                <div class="post-media">
                                    ${data.OriginalPost.Media ? `
                                        ${data.OriginalPost.Media.toLowerCase().endsWith('.mp4') || data.OriginalPost.Media.toLowerCase().endsWith('.mov') || data.OriginalPost.Media.toLowerCase().endsWith('.avi') ?
                                        `<video controls style="max-width: 100%; border-radius: 6px;">
                                                <source src="${data.OriginalPost.Media}" type="video/mp4">
                                                Your browser does not support the video tag.
                                            </video>`
                                        :
                                        `<img src="${data.OriginalPost.Media}" alt="Post Image" style="max-width: 100%; border-radius: 6px; display: block;"
                                                 onerror="this.style.display='none';">`
                                    }
                                    ` : ""}
                                </div>
                            </div>

                            <!-- Post Actions for the reshared post -->
                            <div class="post-actions" style="display: flex; gap: 20px; padding: 10px 0; border-top: 1px solid #eee; margin-top: 10px;">
                                <i class="${isLiked ? "fa-solid" : "fa-regular"} fa-heart postLikes" data-postid="${data.PostId}" style="cursor:pointer; color:${isLiked ? "red" : ""}; font-size: 18px;"></i>
                                <span class="like-count" style="font-size: 14px;">${data.LikeCount || 0}</span>
                                <i class="fa-regular fa-comment postComment" data-postid="${data.PostId}" style="cursor:pointer; font-size: 18px;"></i>
                                <i class="fa-regular fa-paper-plane re-share" data-postid="${data.PostId}" style="cursor:pointer; font-size: 18px;"></i>
                            </div>
                        </div>`;
                        } else {
                            // This is an original post
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
                                    ${data.Media.toLowerCase().endsWith('.mp4') || data.Media.toLowerCase().endsWith('.mov') || data.Media.toLowerCase().endsWith('.avi') ?
                                        `<video controls style="max-width: 100%; border-radius: 6px;">
                                            <source src="${data.Media}" type="video/mp4">
                                            Your browser does not support the video tag.
                                        </video>`
                                        :
                                        `<img src="${data.Media}" alt="Post Image" class="post-image"
                                             onerror="this.style.display='none';">`
                                    }
                                ` : ""}
                            </div>
                            <div class="post-actions">
                                <i class="${isLiked ? "fa-solid" : "fa-regular"} fa-heart postLikes" data-postid="${data.PostId}" style="cursor:pointer; color:${isLiked ? "red" : ""};"></i>
                                <span class="like-count">${data.LikeCount || 0}</span>
                                <i class="fa-regular fa-comment postComment" data-postid="${data.PostId}" style="cursor:pointer;"></i>
                                <i class="fa-regular fa-paper-plane re-share" data-postid="${data.PostId}"></i>
                            </div>
                        </div>`;
                        }
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
        formdata.append("IsLiked", !isLiked); 

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
        var postId = $("#commentModal").attr("data-postid");
        var commentText = $("#newComment").val().trim(); 
        var userId = localStorage.getItem("UserId");
        console.log("Preparing to send comment:", { postId, commentText, userId });
        if (!commentText) {            
            return;
        }
        var formData = new FormData();
        formData.append("PostId", postId);
        formData.append("UserId", userId);
        formData.append("Comment", commentText);

        $.ajax({
            url: "https://localhost:7134/api/Content/PostComments",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                console.log("✅ API Success:", res);
                $("#newComment").val("");
                GetComments(postId); 
            },
            error: function (xhr, status, error) {
                console.error("❌ API Error:", status, error);
                console.error("Response text:", xhr.responseText);
            }
        }).done(function () {
            console.log("AJAX request sent!");
        });
    });

    function GetComments(postId) {
        $("#commentsList").empty();

        $.ajax({
            url: `https://localhost:7134/api/content/getcomments?postId=${postId}`,
            type: "GET",
            success: function (response) {
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
            if ($repliesList.length === 0) {
                console.error("replies-list not found inside comment block");
                return;
            }

            $repliesList.html('<div style="text-align: center; color: #6c757d; padding: 10px;">Loading replies...</div>');
            const apiUrl = `https://localhost:7134/api/content/GetReplies?commentId=${commentId}`;
            const response = await $.ajax({
                url: apiUrl,
                method: 'GET'
            });
            const isSuccess = response.isSuccess ?? response.IsSuccess;
            const replies = response.result ?? response.Result;

            if (isSuccess && Array.isArray(replies) && replies.length > 0) {
                let repliesHtml = '';

                replies.forEach(reply => {
                    const replyDate = reply.commentedAt
                        ? new Date(reply.commentedAt).toLocaleString()
                        : (reply.CommentedAt ? new Date(reply.CommentedAt).toLocaleString() : "Unknown date");

                    repliesHtml += `
                    <div class="reply mb-2" style="padding: 8px; background: white; border-radius: 6px; border: 1px solid #e9ecef;">
                        <div style="display:flex; justify-content:space-between; align-items:flex-start;">
                            <strong style="color: #333; font-size: 14px;">
                                ${reply.userName || reply.UserName || "Unknown User"}
                            </strong>
                            <span style="font-size: 11px; color: #6c757d;">${replyDate}</span>
                        </div>
                        <div style="margin-top: 4px; color: #495057; font-size: 13px; line-height: 1.3;">
                            ${reply.comment || reply.Comment}
                        </div>
                    </div>
                `;
                });

                $repliesList.html(repliesHtml);
            } else {
                $repliesList.html('<div style="text-align: center; color: #6c757d; padding: 10px;">No replies yet</div>');
            }

        } catch (error) {
            console.error('Error loading replies:', error);
            $comment.find('.replies-list')
                .html('<div style="text-align: center; color: #dc3545; padding: 10px;">Error loading replies</div>');
        }
    }
    function postReply(commentId, replyText, $comment) {
        $comment.find('.post-reply-btn').prop('disabled', true).text('Posting...');
        var postId = $("#commentModal").attr("data-postid");
        var userIdValue = localStorage.getItem("UserId");

        if (!postId || !userIdValue) {
            console.error("Missing postId or userId:", postId, userIdValue);
            Swal.fire("Error", "Missing user or post information", "error");
            $comment.find('.post-reply-btn').prop('disabled', false).text('Post Reply');
            return;
        }
        var formData = new FormData();
        formData.append("PostId", postId);
        formData.append("UserId", userIdValue);
        formData.append("Comment", replyText);
        formData.append("ParentId", commentId);

        $.ajax({
            url: "https://localhost:7134/api/Content/PostComments",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                console.log("✅ Reply API Response:", response);
                if (response.StatusCode == 200 || response.IsSuccess) {
                    $comment.find('.reply-text').val('');
                    loadReplies(commentId, $comment);

                } else {
                    Swal.fire("Error", response.Message || "Failed to post reply", "error");
                }
            },
            error: function (error) {
                console.error('❌ Failed to post reply:', error);
                Swal.fire("Error", "Failed to post reply", "error");
            },
            complete: function () {
                $comment.find('.post-reply-btn').prop('disabled', false).text('Post Reply');
            }
        });
    }

    $(document).on("click", ".re-share", function () {
        const userId = localStorage.getItem("UserId");
        const parentId = $(this).data("postid");

        var formData = new FormData();
        formData.append("SharedBy", userId);
        formData.append("ParentId", parentId);

        $.ajax({
            url: "https://localhost:7134/api/Content/ReShare",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (res) {
                if (res.isSuccess || res.StatusCode == 200) {
                    Swal.fire("Success", "Post reshared successfully!", "success");
                    // Refresh the posts to show the reshared post
                    GetPost();
                } else {
                    Swal.fire("Error", res.message || "Failed to reshare post", "error");
                }
            },
            error: function (xhr, status, error) {
                console.error("API Error:", status, error);
                Swal.fire("Error", "Failed to reshare post", "error");
            }
        });
    });
});