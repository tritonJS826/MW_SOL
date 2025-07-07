mergeInto(LibraryManager.library, {
    function _GameStart (userName, score) {
        window.dispatchReactUnityEvent("GameStart", UTF8ToString(userName), score);
    },
});
