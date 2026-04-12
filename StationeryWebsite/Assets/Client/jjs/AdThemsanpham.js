// Lấy dsSanPham từ localStorage
function loadSanPham() {
    return JSON.parse(localStorage.getItem("dsSanPham")) || [];
}

function saveSanPham(list) {
    localStorage.setItem("dsSanPham", JSON.stringify(list));
}

// Chuyển ảnh sang Base64
function convertToBase64(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result);
        reader.onerror = (error) => reject(error);
        reader.readAsDataURL(file);
    });
}

document.getElementById("formSanPham").addEventListener("submit", async function (e) {
    e.preventDefault();

    // --- Lấy file ảnh ---
    const fileInput = document.getElementById("hinh").files[0];
    let imageBase64 = "";

    if (fileInput) {
        imageBase64 = await convertToBase64(fileInput);
    } else {
        alert("Vui lòng chọn hình!!!");
        return;
    }

    // --- Tạo sản phẩm ---
    let sp = {
        danhMuc: document.getElementById("danhMuc").value,
        id: Number(document.getElementById("id").value),
        ten: document.getElementById("ten").value,
        gia: Number(document.getElementById("gia").value),
        hinh: imageBase64, // LƯU BASE64
        maSP: document.getElementById("maSP").value,
        barcode: document.getElementById("id").value,
        thuongHieu: document.getElementById("thuongHieu").value,
        kichThuoc: document.getElementById("kichThuoc").value,
        Thongtinkhac: document.getElementById("Thongtinkhac").value,
        dinhLuong: document.getElementById("dinhLuong").value,
        dongGoi: document.getElementById("dongGoi").value,
    };

    // --- Lưu vào LocalStorage ---
    let ds = loadSanPham();
    let trungID = ds.some((item) => item.id === sp.id);

    if (trungID) {
        alert("ID đã tồn tại, vui lòng nhập ID khác!");
        return;
    }

    ds.push(sp);
    ds.sort((a, b) => a.id - b.id);
    saveSanPham(ds);

    alert("Thêm sản phẩm thành công!");
    this.reset();
});
