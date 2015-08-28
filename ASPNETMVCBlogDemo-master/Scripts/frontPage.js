var loadedAll = false;
var offset = 0; // 5 posts are initially loaded
renderPartial(offset, null, ".postsPartial");
offset = 5;
window.setTimeout(loadMoreIfScroll, 500);

function loadMoreIfScroll() {
    if (loading === false && !loadedAll && ($(window).innerHeight() + $(window).scrollTop()) >= $("body").height()) {
        renderPartial(offset, null, ".postsPartial");
        offset += 5;
    }
    window.setTimeout(loadMoreIfScroll, 500);
}