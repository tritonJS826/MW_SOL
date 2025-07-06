using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput: MonoBehaviour
{
    public static Action OnNextQuestionAction;
    public static Action<QuestionGameObject> OnQuestionClickedAction;
    
    private InputSystem_Actions controls;
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        controls = new InputSystem_Actions();
        controls.Enable();
        controls.Player.Next.performed += OnNextQuestion;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                QuestionGameObject questionGO = hit.collider.GetComponent<QuestionGameObject>();
                if (questionGO != null)
                {
                    OnQuestionClickedAction?.Invoke(questionGO);
                }
            }
        }  
    }

    private void OnNextQuestion(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnNextQuestionAction?.Invoke();
        }
    }
    
    
    
}