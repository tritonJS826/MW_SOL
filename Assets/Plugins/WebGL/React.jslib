mergeInto(LibraryManager.library, {
    _GameStart: function(userName, score) {
        window.dispatchReactUnityEvent("GameStart", UTF8ToString(userName), score);
    },
});
