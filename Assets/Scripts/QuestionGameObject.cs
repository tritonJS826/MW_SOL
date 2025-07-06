using DG.Tweening;
using UnityEngine;

public class QuestionGameObject : MonoBehaviour
{
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private QuestionSO questionData;
    
    private TweenCallback onCompleteCallback;

    public QuestionSO QuestionData => questionData;

    public void Initialize(QuestionSO question, TweenCallback onComplete = null)
    {
        questionData = question;
        onCompleteCallback = onComplete;
    }

    public void StartMoving(Vector2 targetPosition)
    {
        transform.DOLocalMove(targetPosition, questionData.SpeedTime, false)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (onCompleteCallback != null)
                {
                    onCompleteCallback.Invoke();
                }
                Destroy(gameObject);
            });
    }


    public void SetSelected(bool isSelected)
    {
        if (selectedVisual != null)
        {
            selectedVisual.SetActive(isSelected);
        }
        
    }
    

}