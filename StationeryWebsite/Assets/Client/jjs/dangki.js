document.addEventListener("DOMContentLoaded", () => {
    const fdk = document.getElementById("Frm-DK");

    fdk.addEventListener("submit", (e) => {
        e.preventDefault();

        const hoten = document.getElementById("hoten").value.trim();
        const email = document.getElementById("email").value.trim();
        const password = document.getElementById("password").value.trim();

        if (!hoten || !email || !password) return alert("Vui lòng điền đủ thông tin!");

        // Lấy danh sách tài khoản
        let accounts = JSON.parse(localStorage.getItem("accounts")) || [];

        // Kiểm tra email tồn tại
        if (accounts.some((acc) => acc.email === email)) {
            return alert("Email đã được sử dụng!");
        }

        // Tạo tài khoản người dùng
        const newUser = {
            hoten,
            email,
            password,
            role: "user",
        };

        accounts.push(newUser);
        localStorage.setItem("accounts", JSON.stringify(accounts));

        alert("Đăng ký thành công! Bạn có thể đăng nhập.");
        fdk.reset();

        window.location.href = "../html/dangnhap.html";
    });
});
