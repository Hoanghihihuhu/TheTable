using UnityEngine;
using DG.Tweening;

public class Scene2Gate : MonoBehaviour
{
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private float moveDuration = 1f; 
    
    private Vector3 originalPosition;
    private Tween moveTween;
    
    void Start()
    {
        originalPosition = transform.position;
    }
    
    public void Open()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
        }
        
        Debug.Log("Open");
        Vector3 targetPosition = originalPosition + Vector3.up * moveDistance;
        moveTween = transform.DOMoveY(targetPosition.y, moveDuration);
    }
    
    void OnDestroy()
    {
        if (moveTween != null)
        {
            moveTween.Kill();
        }
    }
}
