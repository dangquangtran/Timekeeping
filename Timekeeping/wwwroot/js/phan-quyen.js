$(document).ready(function () {
    $('#filter-msnv').select2({
        placeholder: "Tất cả",
        allowClear: true,
        width: 'resolve',
        language: {
            noResults: function () {
                return "Không tìm thấy";
            }
        }
    });

    $('#filter-msnv').on('change', function () {
        var msnv = $(this).val();
        $.ajax({
            url: '/Permission/GetDanhSachAccount',
            type: 'GET',
            data: {
                pageIndex: 1,
                pageSize: 10,
                msnv: msnv
            },
            success: function (res) {
                var html = '';
                if (res.accounts && res.accounts.length > 0) {
                    res.accounts.forEach(function (item) {
                        html += '<tr>'
                            + '<td>' + (item.maNhanVien ?? '') + '</td>'
                            + '<td>' + (item.userPortal ?? '') + '</td>'
                            + '<td>' + (item.hoTen ?? '') + '</td>'
                            + '<td>' + (item.donVi ?? '') + '</td>'
                            + '<td>' + (item.chucDanh ?? '') + '</td>'
                            + '<td>' + (item.thuocNhomND ?? '') + '</td>'
                            + '</tr>';
                    });
                } else {
                    html = '<tr><td colspan="6" class="text-center text-danger">Không có dữ liệu</td></tr>';
                }
                $('#accounts-tbody').html(html);
            }
        });
    });

    $('#select-mans').select2({
        placeholder: "Chọn mã nhân sự",
        allowClear: true,
        width: 'resolve',
        language: {
            noResults: function () {
                return "Không tìm thấy";
            }
        }
    });
    $('#select-nhomnd').select2({
        placeholder: "Chọn nhóm ND",
        allowClear: true,
        width: 'resolve',
        language: {
            noResults: function () {
                return "Không tìm thấy";
            }
        }
    });
    $('.form-create').on('reset', function () {
        setTimeout(function () {
            $('#select-mans').val('').trigger('change');
            $('#select-nhomnd').val('').trigger('change');
        }, 0);
    });
});