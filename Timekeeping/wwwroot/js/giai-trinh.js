$(document).ready(function () {
    // Khởi tạo Select2 cho dropdown kỳ báo cáo
    $('#kySelect').select2({
        placeholder: "Chọn kỳ",
        allowClear: true,
        width: 'resolve',
        language: {
            noResults: function () {
                return "Không tìm thấy";
            }
        }
    });

    // Khởi tạo Select2 cho các dropdown giải trình
    $('.giaitrinh-select').select2({
        placeholder: "Chọn lý do giải trình...",
        allowClear: true,
        width: 'resolve',
        language: {
            noResults: function () {
                return "Không tìm thấy lý do";
            }
        }
    });

    // Xử lý khi thay đổi lý do giải trình
    $('.giaitrinh-select').on('change', function () {
        var $this = $(this);
        var viphamid = $this.data('viphamid');
        var maky = $this.data('maky');
        var giaitrinhid = $this.val();

        if (viphamid && maky && giaitrinhid) {
            saveGiaiTrinh(viphamid, maky, giaitrinhid, $this);
        }
    });

    // Hàm lưu giải trình
    function saveGiaiTrinh(viphamid, maky, giaitrinhid, element) {
        $.ajax({
            url: '/GiaiTrinh/UpdateGiaiTrinh',
            type: 'POST',
            data: {
                viphamid: viphamid,
                maky: parseInt(maky),
                giaitrinhid: parseInt(giaitrinhid)
            },
            success: function (response) {
                if (response && response.success) {
                    showNotification('Lưu giải trình thành công!', 'success');
                    element.addClass('border-success').removeClass('border-danger');
                    setTimeout(function() {
                        element.removeClass('border-success');
                    }, 2000);
                } else {
                    showNotification(response.message || 'Có lỗi xảy ra khi lưu giải trình!', 'error');
                }
            },
            error: function (xhr, status, error) {
                let errorMessage = 'Có lỗi xảy ra khi lưu giải trình!';
                try {
                    if (xhr.responseText) {
                        const errorResponse = JSON.parse(xhr.responseText);
                        if (errorResponse.message) {
                            errorMessage = errorResponse.message;
                        }
                    }
                } catch (e) {
                    // Ignore parse error
                }
                
                showNotification(errorMessage, 'error');
                element.addClass('border-danger');
                setTimeout(function() {
                    element.removeClass('border-danger');
                }, 2000);
            }
        });
    }

    // Hàm hiển thị thông báo
    function showNotification(message, type) {
        var alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        var alertHtml = `
            <div class="alert ${alertClass} alert-dismissible fade show" role="alert" style="position: fixed; top: 20px; right: 20px; z-index: 9999; min-width: 300px;">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;
        
        $('body').append(alertHtml);
        
        // Tự động ẩn sau 3 giây
        setTimeout(function() {
            $('.alert').fadeOut();
        }, 3000);
    }

    $('#hetHanGiaiTrinhBtn').on('click', function () {
        if (confirm('Bạn có chắc chắn muốn đánh dấu hết hạn giải trình?')) {
            showNotification('Đã đánh dấu hết hạn giải trình!', 'success');
        }
    });
}); 