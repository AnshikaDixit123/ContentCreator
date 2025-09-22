$(document).ready(function () {
    const constUserNameLayout = localStorage.getItem("UserName");
    const constRoleTypeLayout = localStorage.getItem("RoleType");
    $(document).on('click', '#btnLogout', function () {
        localStorage.clear();
        if (constRoleTypeLayout == "Admin") {
            location.href = "https://localhost:7024/";
        }
        else if (constRoleTypeLayout == "ContentCreator") {
            location.href = "https://localhost:7024/account/login";
        }
        else {
            location.href = "https://localhost:7024/account/enduserlogin";
        }
    })
})