$(() => {
    $('#like-button').on('click', function () {
        const id = $('.row').data('id');
        $.get('/home/like', { id }, function () {
            $(this).prop('disabled', true);
        });
    });

    setInterval(function () {
        const id = $('.row').data('id');
        $.get('/home/getlikes', { id }, function (likes) {
            $('#likes-count').text(likes); 
        });
    }, 1000);
});