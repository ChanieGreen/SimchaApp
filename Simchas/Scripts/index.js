$(() => {
    $('#new-simcha').on('click', function () {
        $('#simcha-name').val('');
        $('#simcha-date').val('');
    });

    $('#add').on('click', function () {
        $('#add-simcha').modal('hide');
    });
});