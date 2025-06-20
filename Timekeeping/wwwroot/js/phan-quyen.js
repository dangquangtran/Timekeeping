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