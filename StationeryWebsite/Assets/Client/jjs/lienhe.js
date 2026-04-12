document.addEventListener("DOMContentLoaded", () => {
    const form = document.querySelector(".LienHeForm");
    const camonMsg = document.querySelector(".camon");

    // Ẩn dòng cảm ơn lúc đầu
    camonMsg.style.display = "none";

    form.addEventListener("submit", function (e) {
        e.preventDefault();

        let hoten = document.getElementById("hoten").value.trim();
        let email = document.getElementById("email").value.trim();
        let noidung = document.getElementById("noidung").value.trim();

        // Lấy danh sách liên hệ cũ
        let dsLienHe = JSON.parse(localStorage.getItem("dsLienHe")) || [];

        // Tạo object mới
        let lienHeMoi = {
            hoten: hoten,
            email: email,
            noidung: noidung,
            time: new Date().toLocaleString("vi-VN"),
        };

        // Thêm vào danh sách
        dsLienHe.push(lienHeMoi);

        // Lưu lại vào localStorage
        localStorage.setItem("dsLienHe", JSON.stringify(dsLienHe));

        // Hiện dòng cảm ơn
        camonMsg.style.display = "block";

        // Reset form
        form.reset();
    });
});
