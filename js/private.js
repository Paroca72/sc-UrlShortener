// Select navbar item
$(function () {

    // Name
    var getName = function (url) {
        return (url == null) ? "" : url.substring(url.lastIndexOf('/') + 1);
    };

    // Check for childs
    var checkForChilds = function (owner, name) {
        var find = false;
        if ($(owner).hasClass('dropdown')) {
            $('ul > li > a', owner).each(function () {
                if (getName(this.href) == name) find = true;
            });
        }
        return find;
    };

    // nav bar
    $('#MainNavigationBar > li').each(function (el) {
        var href = (this.childNodes.length > 0) ? this.childNodes[0].href : '';
        var url = $(location).attr('href');

        href = getName(href);
        url = getName(url);

        if (href == url || checkForChilds(this, url)) {
            $(this).addClass("active");
        }
    });
});

// Refresh navbar
var refreshNavbar = function () {
    try {
        $find('ctl00_RAPMainMenu').ajaxRequest();
    } catch (e) { };
};

// Refresh scrollspy
var refreshScrollSpy = function () {
    $('[data-spy="scroll"]').each(function () {
        var $spy = $(this).scrollspy('refresh');
    });
};

// Export ajaxified grid
function onAjaxExportRequest(sender, args) {
    if (args.get_eventTarget().indexOf("ExportToExcelButton") >= 0 ||
        args.get_eventTarget().indexOf("ExportToWordButton") >= 0 ||
        args.get_eventTarget().indexOf("ExportToPdfButton") >= 0 ||
        args.get_eventTarget().indexOf("ExportToCsvButton") >= 0) {
        args.set_enableAjax(false);
    }
}

// Sliding panels
$(function () {
    $(".sliding-panel").each(function () {
        $(this)
            .prepend("<span class='glyphicon glyphicon-circle-arrow-left pull-right' style='margin-top: 14px'></span>")
            .css('cursor', 'pointer')
            .click(function () {
                var next = $(this).next("div");
                next.slideToggle();
                $("span:first-child", this).toggleClass("glyphicon glyphicon-circle-arrow-left");
                $("span:first-child", this).toggleClass("glyphicon glyphicon-circle-arrow-down");
            });
        $(this)
            .next("div")
            .hide();
    });
});

// Loading panel
var showLoadingPanel = function () {
    $("#globalLoadingPanel").show();
};

var hideLoadingPanel = function () {
    $("#globalLoadingPanel").hide();
};

