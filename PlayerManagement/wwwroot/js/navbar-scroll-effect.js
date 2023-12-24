$(document).ready(function () {
    $(document).on('scroll', function () {
        var $nav = $("#mainNavbar");
        if ($(this).scrollTop() > 50) {
            $nav.addClass('scrolled');
        } else {
            $nav.removeClass('scrolled');
        }
    });
});