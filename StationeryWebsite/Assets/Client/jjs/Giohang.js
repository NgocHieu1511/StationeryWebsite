function hienThiGioHang() {
    const ds = document.getElementById("dsGioHang");
    const tongCongEl = document.getElementById("tongCong");
    let gioHang = JSON.parse(localStorage.getItem("gioHang")) || [];

    ds.innerHTML = "";
    let tongCong = 0;
    // xuất hiện thông báo trong bản khi chưa có giỏ hàng chưa có sp
    if (gioHang.length === 0) {
        ds.innerHTML = `<tr><td colspan="6">Chưa có sản phẩm trong giỏ hàng</td></tr>`;
    }
    //
    gioHang.forEach((sp, index) => {
        const tong = sp.gia * sp.sl;
        tongCong += tong;
        ds.innerHTML += `
            <tr>
                <td><img src="${sp.hinh}" class="anhSanPham"></td>
                <td>${sp.ten}</td>
                <td class="donGia">${sp.gia.toLocaleString()} đ</td>
                <td>
                    <div class="ChonSL">
                        <button class="btn-minus giam" data-index="${index}">-</button>
                       
                       <input type="number" class="inputSL" data-index="${index}" value="${sp.sl || 1}" min="1" step="1"/>

                        <button class="btn-plus tang" data-index="${index}">+</button>
                    </div>
                </td>
                <td class="donGia">${tong.toLocaleString()} đ</td>
                <td><button class="btnXoa" data-index="${index}"><i class="fa fa-trash"></i></button></td>
            </tr>
        `;
    });

    // cập nhật tổng cộng
    tongCongEl.textContent = tongCong.toLocaleString() + " đ";

    // Gán vào ô tổng tiền thanh toán nếu có
    const tongCongText = tongCongEl.textContent;
    const tongThanhToan = document.getElementById("tongThanhToan");
    if (tongThanhToan) tongThanhToan.textContent = tongCongText;

    // Nút tăng
    document.querySelectorAll(".tang").forEach((btn) =>
        btn.addEventListener("click", (e) => {
            const i = e.target.dataset.index;
            gioHang[i].sl++;
            localStorage.setItem("gioHang", JSON.stringify(gioHang));
            hienThiGioHang();
        })
    );

    // Nút giảm
    document.querySelectorAll(".giam").forEach((btn) =>
        btn.addEventListener("click", (e) => {
            const i = e.target.dataset.index;
            if (gioHang[i].sl > 1) gioHang[i].sl--;
            else gioHang.splice(i, 1);
            localStorage.setItem("gioHang", JSON.stringify(gioHang));
            hienThiGioHang();
        })
    );
    // nhập số lượng trực tiếp ko đc nhập số âm và 0
    document.querySelectorAll(".inputSL").forEach((input) => {
        input.addEventListener("input", (e) => {
            const i = e.target.dataset.index;
            let sl = parseInt(e.target.value);

            if (isNaN(sl) || sl < 1) {
                sl = 1;
                e.target.value = 1;
            }

            gioHang[i].sl = sl;
            localStorage.setItem("gioHang", JSON.stringify(gioHang));
            hienThiGioHang();
        });
    });

    // Nút xóa
    document.querySelectorAll(".btnXoa").forEach((btn) =>
        btn.addEventListener("click", (e) => {
            const i = e.target.dataset.index;
            gioHang.splice(i, 1);
            localStorage.setItem("gioHang", JSON.stringify(gioHang));
            hienThiGioHang();
        })
    );
}

document.addEventListener("DOMContentLoaded", hienThiGioHang);
// XỬ LÝ LOGIC
document.addEventListener("DOMContentLoaded", () => {
    const formBox = document.querySelector(".thongtin-box");
    const successBox = document.querySelector(".thongbao-thanhtoan");
    const paymentBox = document.getElementById("payment-box");
    const btnThanhToan = document.getElementById("btnThanhToan");
    const formThanhToan = document.getElementById("formThanhToan");
    const dsGioHangTbody = document.getElementById("dsGioHang");
    const maDonHangEl = document.getElementById("maDonHang");
    const tongCongEls = document.querySelectorAll("#tongCong");

    // Ẩn form và thông báo thành công lúc ban đầu
    if (formBox) formBox.style.display = "none";
    if (successBox) successBox.style.display = "none";

    // Nút Thanh toán
    btnThanhToan &&
        btnThanhToan.addEventListener("click", () => {
            //  KIỂM TRA NGƯỜI DÙNG ĐÃ ĐĂNG NHẬP CHƯA
            const currentUser = JSON.parse(localStorage.getItem("currentUser"));
            if (!currentUser) {
                alert("Bạn cần đăng nhập trước khi thanh toán!");
                window.location.href = "../html/dangnhap.html";
                return;
            }

            const gioHang = JSON.parse(localStorage.getItem("gioHang")) || [];
            if (!gioHang || gioHang.length === 0) {
                alert("Giỏ hàng của bạn đang trống!");
                return;
            }

            //  Khi đã đăng nhập  HIỆN FORM ĐẶT HÀNG
            if (formBox) {
                formBox.style.display = "block";
                window.scrollTo({ top: formBox.offsetTop - 20, behavior: "smooth" });
            }

            if (successBox) successBox.style.display = "none";
        });

    // Hàm kiểm tra email
    function isValidEmail(email) {
        return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    }

    // Hàm kiểm tra số điện thoại
    function isValidPhone(phone) {
        return /^[0-9]{7,12}$/.test(phone.replace(/\s+/g, ""));
    }

    // Xử lý submit form Đặt hàng
    formThanhToan &&
        formThanhToan.addEventListener("submit", (e) => {
            e.preventDefault();

            const inputs = Array.from(formThanhToan.querySelectorAll(".custom-input")).filter((el) => el.tagName.toLowerCase() !== "textarea");

            // thứ tự input
            const requiredInputs = inputs.slice(0, 6);

            let valid = true;
            let firstInvalid = null;

            requiredInputs.forEach((inp) => (inp.style.border = ""));

            requiredInputs.forEach((inp, idx) => {
                const val = inp.value.trim();

                if (val === "") {
                    valid = false;
                    if (!firstInvalid) firstInvalid = inp;
                    inp.style.border = "1px solid red";
                } else {
                    if (inp.type === "email" && !isValidEmail(val)) {
                        valid = false;
                        if (!firstInvalid) firstInvalid = inp;
                        inp.style.border = "1px solid red";
                    } else if (idx === 4 && !isValidPhone(val)) {
                        valid = false;
                        if (!firstInvalid) firstInvalid = inp;
                        inp.style.border = "1px solid red";
                    } else {
                        inp.style.border = "1px solid #ced4da";
                    }
                }
            });

            if (!valid) {
                alert("Vui lòng điền đúng và đầy đủ thông tin.");
                if (firstInvalid) firstInvalid.focus();
                return;
            }

            // Tạo mã đơn hàng
            const randomCode = "MDH" + Math.floor(100000 + Math.random() * 900000);

            // Lấy người dùng hiện tại
            const currentUserLS = JSON.parse(localStorage.getItem("currentUser"));
            const emailNguoiMua = currentUserLS.email;

            // Tạo key theo từng tài khoản
            const keyDonHang = "donHang_" + emailNguoiMua;

            // Lấy danh sách đơn hàng cũ
            let danhSachDonHang = JSON.parse(localStorage.getItem(keyDonHang)) || [];
            // Lấy dữ liệu từ form
            const hoten = document.getElementById("hoten").value.trim();
            const diachi = document.getElementById("diachi").value.trim();
            const sdt = document.getElementById("sdt").value.trim();
            const ghichu = document.getElementById("ghichu").value.trim();

            // Lấy tổng tiền thanh toán bỏ chữ đ, bỏ dấu chấm
            let tongGiaText = document.getElementById("tongThanhToan").textContent.replace(/[^\d]/g, "");
            let tongGiaTri = parseInt(tongGiaText) || 0;

            // Lấy giỏ hàng hiện tại
            const gioHangHienTai = JSON.parse(localStorage.getItem("gioHang")) || [];

            // Tạo ngày mua ngày giờ VN
            const ngayMua = new Date();
            const ngay = String(ngayMua.getDate()).padStart(2, "0");
            const thang = String(ngayMua.getMonth() + 1).padStart(2, "0");
            const nam = ngayMua.getFullYear();
            const gio = String(ngayMua.getHours()).padStart(2, "0");
            const phut = String(ngayMua.getMinutes()).padStart(2, "0");
            const giay = String(ngayMua.getSeconds()).padStart(2, "0");

            const ngayMuaText = `${ngay}/${thang}/${nam} ${gio}:${phut}:${giay}`;

            // Tạo object đơn hàng
            const donHang = {
                maDonHang: randomCode,
                email: emailNguoiMua,
                hoTen: hoten,
                diaChi: diachi,
                soDienThoai: sdt,
                ghiChu: ghichu,
                ngayMua: ngayMuaText,
                tongGia: tongGiaTri,
                trangThai: "Đang xử lý",
                danhSachSanPham: gioHangHienTai,
            };

            // Thêm đơn hàng vào mảng
            danhSachDonHang.unshift(donHang);

            // Lưu vào LocalStorage
            localStorage.setItem(keyDonHang, JSON.stringify(danhSachDonHang));

            // Xóa giỏ hàng
            localStorage.removeItem("gioHang");

            // Cập nhật bảng giỏ hàng về rỗng
            try {
                hienThiGioHang();
            } catch (err) {
                console.warn("Không thể làm mới giỏ hàng:", err);
            }

            // Reset tổng tiền về 0
            tongCongEls.forEach((el) => (el.textContent = "0"));

            // Hiện mã đơn
            if (maDonHangEl) maDonHangEl.textContent = randomCode;

            // Hiện thông báo thành công
            if (successBox) successBox.style.display = "block";

            // Ẩn form
            if (formBox) formBox.style.display = "none";

            // Cuộn xuống thông báo
            if (successBox) {
                window.scrollTo({ top: successBox.offsetTop - 10, behavior: "smooth" });
            }
        });
});

function themGioHang(id, soLuongThem = 1) {
    let gioHang = JSON.parse(localStorage.getItem("gioHang")) || [];
    let dsSanPham = JSON.parse(localStorage.getItem("dsSanPham")) || [];

    let sp = dsSanPham.find((x) => x.id === id);
    if (!sp) {
        alert("Sản phẩm không tồn tại!");
        return;
    }

    let item = gioHang.find((x) => x.id === id);

    if (item) {
        item.sl += soLuongThem;
    } else {
        gioHang.push({
            id: sp.id,
            ten: sp.ten,
            gia: Number(sp.gia),
            hinh: sp.hinh,
            sl: soLuongThem,
        });
    }

    localStorage.setItem("gioHang", JSON.stringify(gioHang));
    alert("Đã thêm vào giỏ hàng!");
}
