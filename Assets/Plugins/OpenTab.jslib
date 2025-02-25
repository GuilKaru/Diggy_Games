mergeInto(LibraryManager.library, {
    OpenNewTab: function (url) {
        window.open(UTF8ToString(url), '_blank');
    }
});