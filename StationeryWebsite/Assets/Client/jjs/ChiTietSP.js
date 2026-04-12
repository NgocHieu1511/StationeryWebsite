document.addEventListener("DOMContentLoaded", () => {
    // Lấy ID từ URL
    const urlParams = new URLSearchParams(window.location.search);
    const id = parseInt(urlParams.get("id"));

    const dsSanPham = JSON.parse(localStorage.getItem("dsSanPham")) || [];

    // Tìm sản phẩm theo ID
    const sp = dsSanPham.find((x) => x.id === id);

    if (!sp) {
        alert("Không tìm thấy sản phẩm!");
        window.location.href = "TrangSanPham.html";
        return;
    }

    // ĐỔ DỮ LIỆU VÀO HTML
    document.getElementById("ct-img").src = sp.hinh;
    document.getElementById("ct-ten").textContent = sp.ten;
    document.getElementById("ct-gia").textContent = sp.gia.toLocaleString() + " đ";
    document.getElementById("ct-ma").textContent = `Mã sản phẩm: ${sp.id}`;

    // Hiển thị thông số dạng mảng li
    const thongSoHTML = `
    <li>Thương hiệu: ${sp.thuongHieu}</li>
    <li>Kích thước: ${sp.kichThuoc}</li>
    <li>Định lượng: ${sp.dinhLuong}</li>
    <li>Đóng gói: ${sp.dongGoi}</li>
    <li>Thông tin khác: ${sp.Thongtinkhac}</li>
`;

    document.getElementById("ct-thongso").innerHTML = thongSoHTML;

    //Xử lý tăng giảm số lượng
    const inputSL = document.getElementById("ct-sl");
    document.querySelector(".btn-plus").onclick = () => {
        inputSL.value = parseInt(inputSL.value) + 1;
    };
    document.querySelector(".btn-minus").onclick = () => {
        let sl = parseInt(inputSL.value);
        if (sl > 1) inputSL.value = sl - 1;
    };
    // Chặn nhập sai số lượng
    inputSL.addEventListener("input", () => {
        let sl = parseInt(inputSL.value);
        if (isNaN(sl) || sl < 1) {
            inputSL.value = 1;
        }
    });

    // Nút Thêm giỏ hàng
    document.getElementById("btn-add-cart").onclick = () => {
        themGioHang(sp.id, parseInt(inputSL.value));
    };

    //SẢN PHẨM TƯƠNG TỰ
    function taoTheSPTuongTu(x) {
        return `
            <div class="card-sanpham">
                <a href="TrangChiTietSanPham.html?id=${x.id}"> 
                    <img src="${x.hinh}" class="anh-sp" alt="${x.ten}" />
                </a>
                
                <p class="ten-sp">${x.ten}</p>
                <h4 class="gia-sp">Giá: ${x.gia.toLocaleString()} đ</h4>
                <a href="TrangChiTietSanPham.html?id=${x.id}" class="nut-xem">
                    Xem sản phẩm
                </a>
            </div>
        `;
    }
    const listTuongTu = dsSanPham.filter((x) => x.danhMuc === sp.danhMuc && x.id !== sp.id).slice(0, 5);

    document.getElementById("spTuongTu").innerHTML = listTuongTu.map(taoTheSPTuongTu).join("");
});
