$(() => {
    $('#new-contributor').on('click', function () {
        $('#first-name').val('');
        $('#last-name').val('');
        $('#phone-number').val('');
    });

    $('#add').on('click', function () {
        $('#contributor').modal('hide');
    });

    $('.edit').on('click', function () {
        $('#modal-title').text('Update Contributor');
        const id = $(this).data('id');
        $('#contributor-form').attr('action', `/contributors/updatecontributor?id=${id}`);
        $('#row').append(`<input type="hidden" name="id" value="${id}" />`);
        $('#add').text('Update');
        $('#f-date').hide();
        $('#f-initial-deposit').hide();
        const firstName = $(this).data('first-name');
        const lastName = $(this).data('last-name');
        const phoneNumber = $(this).data('phone-number');
        const alwaysInclude = $(this).data('always-include');
        $('#first-name').val(firstName);
        $('#last-name').val(lastName);
        $('#phone-number').val(phoneNumber);
        $('#always-include').prop('checked', alwaysInclude === 'True');
        $('#contributor').modal();
    });

    $('.deposit').on('click', function () {
        $('#deposit-amount').val('');
        const contributorId = $(this).data('contributor-id');
        $('#contributor-id').val(contributorId);
        $('#deposit').modal();
    });
});