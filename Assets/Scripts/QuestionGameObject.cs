using DG.Tweening;
using UnityEngine;

public class QuestionGameObject : MonoBehaviour
{
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private QuestionSO questionData;
    
    private TweenCallback<QuestionGameObject> onCompleteCallback;

    public QuestionSO QuestionData => questionData;

    public void Initialize(QuestionSO question, TweenCallback<QuestionGameObject> onComplete = null)
    {
        questionData = question;
        onCompleteCallback = onComplete;
    }

    public void StartMoving(Vector2 targetPosition)
    {
        transform.DOLocalMove(targetPosition, questionData.SpeedTime, false)
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
    

}