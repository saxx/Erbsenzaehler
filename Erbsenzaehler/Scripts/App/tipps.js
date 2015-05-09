$(document).ready(function () {
    $(".alert[data-tipp]").each(function (index, elem) {
        elem = $(elem);
        var key = "hide_tipp_" + elem.data("tipp");

        if (!localStorage.getItem(key)) {
            elem.removeClass("hidden");
        }

        elem.find("a[data-dismiss]").click(function() {
            localStorage.setItem(key, true);
        });
    });
});