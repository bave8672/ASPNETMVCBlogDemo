var loading = false;
var loadedAll = false;
var offset = 0; // 5 posts are initially loaded
renderPartial(offset, null, ".postsPartial");
offset = 5;
window.setTimeout(loadMoreIfScroll, 500);

function loadMoreIfScroll() {
    if (loading === false && !loadedAll && $(window).innerHeight() + $(window).scrollTop() >= $("html").height()) {
        renderPartial(offset, null, ".postsPartial");
        offset += 5;
    }
    window.setTimeout(loadMoreIfScroll, 500);
}

function renderPartial(ofs, q, target) {
    loading = true;
    $.ajax({
        type: "POST",
        url: "Posts/RenderPartial",
        data: { offset: ofs, query: q },
        dataType: "html",
        success: function (data) {
            loading = false;
            if (/\S/.test(data)) {
                $(target).html($(target).html() + data); // Data is added
            }
            else {
                loadedAll = true;
                $(".ajaxLoader").hide();
                $(".ajaxMessage").html("<br/>No more posts.");
            } // Every post has been loaded, so stop sending requests
        }
    });
}
