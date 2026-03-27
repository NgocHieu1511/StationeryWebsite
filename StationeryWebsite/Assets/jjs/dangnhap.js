document.addEventListener("DOMContentLoaded", () => {
    // Tạo sẵn admin nếu chưa có
    let accounts = JSON.parse(localStorage.getItem("accounts")) || [];

    const adminEmail = "admin@gmail.com";
    const adminPassword = "123456";

    // Kiểm tra có admin chưa
    if (!accounts.some((acc) => acc.role === "admin")) {
        const admin = {
            hoten: "Admin",
            email: adminEmail,
            password: adminPassword,
            role: "admin",
        };
        accounts.push(admin);
        localStorage.setItem("accounts", JSON.stringify(accounts));
    }

    const fdn = document.getElementById("Frm-DN");

    fdn.addEventListener("submit", (e) => {
        e.preventDefault();

        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value.trim();

        // Lấy lại account mới nhất
        const accounts = JSON.parse(localStorage.getItem("accounts")) || [];

        // Tìm tài khoản khớp email + pass
        const user = accounts.find((acc) => acc.email === email && acc.password === password);

        if (!user) {
            return alert("Email hoặc mật khẩu không đúng!");
        }

        // Lưu người đăng nhập hiện tại
        localStorage.setItem("currentUser", JSON.stringify(user));

        // Điều hướng theo role
        if (user.role === "admin") {
            alert("Đăng nhập thành công! ADMIN");
            window.location.href = "admin.html";
        } else {
            alert("Đăng nhập thành công! USER");
            window.location.href = "TrangChu.html";
        }
    });
});
