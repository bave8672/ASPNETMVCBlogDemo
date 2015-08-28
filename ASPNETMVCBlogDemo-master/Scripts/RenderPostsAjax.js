var loading = false;

function renderPartial(ofs, q, target) {
    loading = true;
    $.ajax({
        type: "POST",
        url: "Posts/RenderPartial",
        data: { offset: ofs, query: q },
        dataType: "html",
        success: function (data) {
            loading = false;
            if (data) {
                $(target).html($(target).html() + data); //Data is added
            }
            else { loadedAll = true; } // Every post has been loaded, so stop sending requests
            $(".ajaxLoader").hide();
            $(".ajaxMessage").html("<br/>No more posts.");
        }
    });
}
