var markdown = new MarkdownDeep.Markdown();
var titleInput = $(".titleInput");
var titleOutput = $(".titleOutput");
var input = $(".mdInput");
var output = $(".mdPreview");
input.keyup(function () {
    titleOutput.text(titleInput.val());
    output.html(markdown.Transform(input.val()));
});