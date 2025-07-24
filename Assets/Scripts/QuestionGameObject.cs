using Data;
using DG.Tweening;
using UnityEngine;

public class QuestionGameObject: MonoBehaviour
{
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private QuestionData questionData;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private bool _isWaitingForAnswer = false;
    
    private TweenCallback<QuestionGameObject> onCompleteCallback;
    
    public QuestionData QuestionData => questionData;
    private float _remainingTime;
    private Tween _timerTween;

    public void Initialize(QuestionData data, TweenCallback<QuestionGameObject> onComplete = null)
    {
        questionData = data;
        onCompleteCallback = onComplete;
    }

    public void StartMoving(Vector2 targetPosition)
    {
        transform.DOLocalMove(targetPosition, questionData.timeToAnswer, false)
            .SetEase(Ease.Linear)
            .OnComplete(() => { onCompleteCallback?.Invoke(this); });

        _remainingTime = questionData.timeToAnswer;
        int loops = Mathf.FloorToInt(questionData.timeToAnswer / 0.3f);
        _timerTween?.Kill();
        _timerTween = DOVirtual.DelayedCall(0.3f, () =>
        {
            _remainingTime -= 0.3f;
        }).SetLoops(loops, LoopType.Restart);
    }
    

    public void SetSelected(bool isSelected)
    {
        if (selectedVisual != null)
        {
            selectedVisual.SetActive(isSelected);
        }
    }
    
    public void SetSelected(bool isSelected, Color color)
    {
        SetSelected(isSelected);
        selectedVisual.GetComponent<SpriteRenderer>().color = color;
    }

    public void StopAllTwens()
    {
        DOTween.Kill(transform);
        onCompleteCallback = null;
    }

    public void SetWaitingForAnswer(bool isWaiting)
    {
        _isWaitingForAnswer = isWaiting;
        spriteRenderer.color = isWaiting ? Color.yellow: Color.white;
    }
    
    public void StopAndDestroy(bool isCorrect)
    {
        Debug.Log("#QGM action StopAndDestroy");
        StopAllTwens();
        
        if (isCorrect)
        {
            // Handle correct answer visual feedback
            spriteRenderer.color = Color.green; // Example: change color to green
        }
        else
        {
            // Handle incorrect answer visual feedback
            spriteRenderer.color = Color.red; // Example: change color to red
        }
        
        Destroy(gameObject, 1f); // Adjust the delay as needed
    }
    
    public bool IsWaitingForAnswer()
    {
        return _isWaitingForAnswer;
    }

    public float GetRemainingTime()
    {
        return _remainingTime;
    }

    public QuestionData GetQuestionData()
    {
        return questionData;
    }
}