$(() => {

    $("#new-simcha").on('click', function () {
        $(".modal").modal()
    });

    $("#new-person").on('click', function () {
        $(".add-person").modal()
    }); 

    $(".new-fund").on('click', function () {
        const cId = $(this).data('contributer-id');
        $('[name="contributerid"]').val(cId);
        $(".add-fund").modal()
    });

    $(function () {
        $(".contribute").bootstrapToggle({
            on: 'Yes',
            off: 'No'
        });
    });
})