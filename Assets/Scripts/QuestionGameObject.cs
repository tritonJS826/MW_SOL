using Data;
using DG.Tweening;
using UnityEngine;

public class QuestionGameObject: MonoBehaviour
{
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private QuestionData questionData;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private TweenCallback<QuestionGameObject> onCompleteCallback;


    public QuestionData QuestionData => questionData;
    
    

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
    }


    public void SetSelected(bool isSelected)
    {
        if (selectedVisual != null)
        {
            selectedVisual.SetActive(isSelected);
        }
        
    }

    public void StopAllTwens()
    {
        DOTween.Kill(transform);
        onCompleteCallback = null;
    }

    public void StopAndDestroy(bool isCorrect)
    {
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
    

}