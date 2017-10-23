$(document).ready(function () {
    if ($(window).width() < 800) {
        $('.data-table').DataTable({
            responsive: true
        });
    }
    else
        $('.data-table').DataTable();
});