$(document).ready(function () {
    $('#bell-icon').click(function () {
        $.get('/Notification/Index', function (data) {
            $('#notificationModal .modal-body').html(data);
            $('#notificationModal').modal('show');
        });
    });
});