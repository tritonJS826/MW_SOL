mergeInto(LibraryManager.library, {
    GameStarted: function() {
        window.dispatchReactUnityEvent("GameStarted");
    },
    GameFinished: function() {
        window.dispatchReactUnityEvent("GameFinished");
    },
    UserAnsweredQuestion: function(questionUuid, userAnswer) {
        window.dispatchReactUnityEvent("UserAnsweredQuestion", UTF8ToString(questionUuid), UTF8ToString(userAnswer));
    },
    UserCapturedTarget: function(questionUuid) {
        window.dispatchReactUnityEvent("UserCapturedTarget", UTF8ToString(questionUuid));
    },
});
