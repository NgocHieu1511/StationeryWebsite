document.addEventListener("DOMContentLoaded", () => {
    //  TẢI DANH SÁCH LIÊN HỆ
    function loadContacts() {
        return JSON.parse(localStorage.getItem("dsLienHe")) || [];
    }

    //  LƯU DANH SÁCH SAU KHI XÓA
    function saveContacts(list) {
        localStorage.setItem("dsLienHe", JSON.stringify(list));
    }

    //  HIỂN THỊ LÊN BẢNG
    function renderTable() {
        const list = loadContacts();
        const table = document.getElementById("tableBody");
        table.innerHTML = "";

        list.forEach((lh, index) => {
            table.innerHTML += `
                <tr>
                    <td>${lh.hoten}</td>
                    <td>${lh.email}</td>
                    <td>${lh.noidung}</td>
                    <td>
                        <button class="btn btn-sm btnXoa" data-index="${index}">
                            <i class="fa fa-trash"></i>
                        </button>
                    </td>
                </tr>
            `;
        });

        // Gắn sự kiện xóa sau khi bảng được tạo
        document.querySelectorAll(".btnXoa").forEach((btn) => {
            btn.addEventListener("click", function () {
                deleteContact(this.dataset.index);
            });
        });
    }

    //      XÓA LIÊN HỆ
    function deleteContact(i) {
        const list = loadContacts();

        if (confirm("Bạn chắc chắn muốn xóa liên hệ này?")) {
            list.splice(i, 1); // Xóa liên hệ
            saveContacts(list); // Lưu lại
            renderTable(); // Cập nhật lại giao diện
        }
    }
    //  GỌI HIỂN THỊ BẢNG LẦN ĐẦU
    renderTable();
});
