window.addEventListener('DOMContentLoaded', event => {
    // Mã JavaScript s? d?ng th? vi?n Simple-DataTables ?? qu?n lý d? li?u trên trang web
    // https://github.com/fiduswriter/Simple-DataTables/wiki

    const datatablesSimple = document.getElementById('datatablesSimple');
    if (datatablesSimple) {
        new simpleDatatables.DataTable(datatablesSimple);
    }
});
