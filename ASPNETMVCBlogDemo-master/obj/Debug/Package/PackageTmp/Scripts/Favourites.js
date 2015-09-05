/**
* Client-side handling for favourites
*/

// Bind action to button making sure not to bind multiple times
$(".favouriteButton").unbind('click');
$(".favouriteButton").bind('click', function () {
    return favourite(this)
});

function favourite(jqEl) {
    var favouriting;
    if ($(jqEl).html().indexOf('glyphicon-heart">') !== -1) {
        $(jqEl).html('<span class="glyphicon glyphicon-heart-empty"></span>');
        favouriting = false;
        $(jqEl).parent().children('.favouritesCount').html(
            Number($(jqEl).parent().children('.favouritesCount').html()) - 1
        );
    }
    else {
        $(jqEl).html('<span class="glyphicon glyphicon-heart"></span>');
        favouriting = true;
        $(jqEl).parent().children('.favouritesCount').html(
            Number($(jqEl).parent().children('.favouritesCount').html()) + 1
        );
    }
    handleFavourite($(jqEl).closest('.favouritePartial').attr('id'), favouriting);
}

function handleFavourite(postId, favouriting) {
    $.ajax({
        type: 'POST',
        url: $('#getFavouritePath').val(),
        data: {
            'postId': Number(postId),
            'favouriting': favouriting
        }
    });
}
