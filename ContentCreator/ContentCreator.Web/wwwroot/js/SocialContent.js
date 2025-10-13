$(document).ready(function () {
    var rawHtml = "";
    for (var i = 1; i <= 2; i++) {
        rawHtml += `<div class="post-card">
            <div class="post-header">
                <img src="~/image/app-avatar-default.png" alt="User" class="post-avatar">
                <span class="post-username">${i}</span>
            </div>
            <div class="post-media">
                <img src="~/image/sample1.jpg" alt="Post Image">
            </div>
            <div class="post-actions">
                <i class="fa-regular fa-heart"></i>
                <i class="fa-regular fa-comment"></i>
                <i class="fa-regular fa-paper-plane"></i>
            </div>
            <div class="post-caption">
                <strong>nightingale1402</strong> enjoying the moment 💖✨
            </div>
        </div>`;
        if (i == 2) {
            $('#postSection').html(rawHtml);
        }
    }
})