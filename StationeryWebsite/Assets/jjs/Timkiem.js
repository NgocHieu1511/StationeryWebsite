document.addEventListener("DOMContentLoaded", () => {
    const dsSanPham = JSON.parse(localStorage.getItem("dsSanPham")) || [];
    const inputTim = document.querySelector(".search-box input");
    const formTimKiem = document.querySelector(".search-box");

    // Hàm loại bỏ dấu  để tìm tiếng Việt
    function removeDiacritics(str) {
        return str
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "")
            .replace(/đ/g, "d")
            .replace(/Đ/g, "D");
    }

    formTimKiem.addEventListener("submit", function (e) {
        e.preventDefault();

        const tuKhoaRaw = inputTim.value.trim();
        if (tuKhoaRaw === "") return;

        // Lưu vào localStorage để trang SanPhamChung dùng lại
        localStorage.setItem("tuKhoaTimKiem", tuKhoaRaw);

        // Chuyển qua trang sản phẩm chung
        window.location.href = "../Html/SanPhamChung.html";
    });
});
