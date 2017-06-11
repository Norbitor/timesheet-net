function datepickersSetup() {
    $('#start-datepicker').datetimepicker({
        locale: 'pl',
        format: 'YYYY-MM-DD'
    });

    $('#finish-datepicker').datetimepicker({
        locale: 'pl',
        format: 'YYYY-MM-DD'
    });

    $("#start-datepicker").on("dp.change", function (e) {
        $('#finish-datepicker').data("DateTimePicker").minDate(e.date);
    });

    $("#finish-datepicker").on("dp.change", function (e) {
        $('#start-datepicker').data("DateTimePicker").maxDate(e.date);
    });
}