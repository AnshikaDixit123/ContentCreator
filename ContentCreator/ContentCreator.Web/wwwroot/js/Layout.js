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
    $(document).ready(function () {
        // Get the current path in lowercase
        var path = document.location.pathname.toLowerCase();
        const adminPaths = [
            "/allusers",
            "/createuser",
            "/region",
            "/roles",
            "/fileextensions" 
        ];
        // Show content button only on /login page
        if (path.endsWith("/dashboard") || path.endsWith("/uploadcontent")) {
            $("#content").removeClass('d-none');
        }
        else if (adminPaths.some(p => path.endsWith(p))) {
            $("#superAdmin").removeClass('d-none');
        }
    });

})