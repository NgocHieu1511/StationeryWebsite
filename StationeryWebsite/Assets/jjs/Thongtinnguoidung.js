// load thông tin của người dùng hiện tại
function loadThongTinNguoiDung() {
    let currentUser = JSON.parse(localStorage.getItem("currentUser"));
    if (!currentUser) return;

    document.getElementById("hoten").value = currentUser.hoten;
    document.getElementById("email").value = currentUser.email;
}

document.addEventListener("DOMContentLoaded", loadThongTinNguoiDung);
//  đổi tên, mật khẩu
function luuThongTin() {
    let accounts = JSON.parse(localStorage.getItem("accounts")) || [];
    let currentUser = JSON.parse(localStorage.getItem("currentUser"));

    if (!currentUser) {
        alert("Chưa đăng nhập!");
        return;
    }

    let hotenMoi = document.getElementById("hoten").value.trim();
    let oldPass = document.getElementById("oldPassword").value;
    let newPass = document.getElementById("newPassword").value;
    let confirmPass = document.getElementById("confirmPassword").value;

    // tìm user trong accounts
    let index = accounts.findIndex((acc) => acc.email === currentUser.email);

    if (index === -1) {
        alert("Không tìm thấy tài khoản!");
        return;
    }

    // cập nhật tên
    accounts[index].hoten = hotenMoi;
    currentUser.hoten = hotenMoi;

    //nếu người dùng có nhập mật khẩu
    if (oldPass || newPass || confirmPass) {
        // kiểm tra mật khẩu hiện tại
        if (oldPass !== accounts[index].password) {
            alert("Mật khẩu hiện tại không đúng!");
            return;
        }

        // kiểm tra mật khẩu mới
        if (newPass.length < 1) {
            alert("Mật khẩu mới không được để trống!");
            return;
        }

        // xác nhận mật khẩu
        if (newPass !== confirmPass) {
            alert("Xác nhận mật khẩu không khớp!");
            return;
        }

        // cập nhật mật khẩu
        accounts[index].password = newPass;
        currentUser.password = newPass;
    }

    // lưu lại localStorage
    localStorage.setItem("accounts", JSON.stringify(accounts));
    localStorage.setItem("currentUser", JSON.stringify(currentUser));

    alert("Cập nhật thông tin thành công!");

    // làm trống ô mật khẩu
    document.getElementById("oldPassword").value = "";
    document.getElementById("newPassword").value = "";
    document.getElementById("confirmPassword").value = "";
}
// Thống kê tổng quan mua hàng của người dùng

function xacDinhHocKyNamHoc(ngayMua) {
    const [ngay] = ngayMua.split(" ");
    const [, m, y] = ngay.split("/").map(Number);

    let namBatDau, namKetThuc, hocKy;

    if (m >= 9 && m <= 12) {
        hocKy = "HK1";
        namBatDau = y;
        namKetThuc = y + 1;
    } else if (m >= 1 && m <= 5) {
        hocKy = "HK2";
        namBatDau = y - 1;
        namKetThuc = y;
    } else {
        hocKy = "HK_HÈ";
        namBatDau = y - 1;
        namKetThuc = y;
    }

    return `${hocKy}-${namBatDau}-${namKetThuc}`;
}

function layDanhMucTheoId(id) {
    const dsSanPham = JSON.parse(localStorage.getItem("dsSanPham")) || [];
    const sp = dsSanPham.find((x) => x.id === id);
    return sp ? sp.danhMuc : "Sản phẩm khác";
}

function thongKeChiTieuTheoHocKy() {
    const currentUser = JSON.parse(localStorage.getItem("currentUser"));
    if (!currentUser) return {};

    const donHang = JSON.parse(localStorage.getItem(`donHang_${currentUser.email}`)) || [];

    let ketQua = {};

    donHang.forEach((dh) => {
        const hocKyNamHoc = xacDinhHocKyNamHoc(dh.ngayMua);

        if (!ketQua[hocKyNamHoc]) ketQua[hocKyNamHoc] = {};

        dh.danhSachSanPham.forEach((sp) => {
            const danhMuc = layDanhMucTheoId(sp.id);
            const tien = sp.gia * sp.sl;

            if (!ketQua[hocKyNamHoc][danhMuc]) {
                ketQua[hocKyNamHoc][danhMuc] = 0;
            }
            ketQua[hocKyNamHoc][danhMuc] += tien;
        });
    });

    // CHỈ LẤY 2 NĂM HỌC GẦN NHẤT
    const hocKySapXep = Object.keys(ketQua).sort((a, b) => {
        const [, , y1] = a.split("-");
        const [, , y2] = b.split("-");
        return Number(y2) - Number(y1);
    });

    const hocKyGioiHan = hocKySapXep.slice(0, 6);

    let ketQuaCuoi = {};
    hocKyGioiHan.reverse().forEach((hk) => {
        ketQuaCuoi[hk] = ketQua[hk];
    });

    return ketQuaCuoi;
}

function veBieuDoHocKy() {
    const data = thongKeChiTieuTheoHocKy();

    const hocKy = Object.keys(data).sort();
    if (hocKy.length === 0) return;

    const danhMucSet = new Set();
    hocKy.forEach((hk) => {
        Object.keys(data[hk]).forEach((dm) => danhMucSet.add(dm));
    });

    const danhMuc = [...danhMucSet];

    const datasets = danhMuc.map((dm, index) => ({
        label: dm,
        data: hocKy.map((hk) => data[hk][dm] || 0),
        backgroundColor: `hsl(${index * 60}, 70%, 60%)`,
    }));

    new Chart(document.getElementById("bieuDoHocKy"), {
        type: "bar",
        data: {
            labels: hocKy,
            datasets: datasets,
        },
        options: {
            responsive: true,
            plugins: {
                legend: { position: "top" },
                tooltip: {
                    callbacks: {
                        label: (ctx) => `${ctx.dataset.label}: ${ctx.raw.toLocaleString()} đ`,
                    },
                },
            },
            scales: {
                x: { stacked: true },
                y: {
                    stacked: true,
                    beginAtZero: true,
                    ticks: {
                        callback: (value) => value.toLocaleString() + " đ",
                    },
                },
            },
        },
    });
}

document.addEventListener("DOMContentLoaded", veBieuDoHocKy);
