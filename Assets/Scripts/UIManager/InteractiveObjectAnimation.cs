using UnityEngine;
using DG.Tweening;

/// <summary>
/// Script tạo hiệu ứng xoay và nhấp nhô cho các vật thể có thể tương tác.
/// Sử dụng DOTween để tạo animation mượt mà.
/// </summary>
public class InteractiveObjectAnimation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Bật/tắt hiệu ứng xoay")]
    public bool enableRotation = true;
    
    [Tooltip("Tốc độ xoay (độ/giây)")]
    public float rotationSpeed = 90f;
    
    [Tooltip("Trục xoay (1 = X, 2 = Y, 3 = Z)")]
    public Vector3 rotationAxis = Vector3.up;
    
    [Header("Bounce Settings")]
    [Tooltip("Bật/tắt hiệu ứng nhấp nhô")]
    public bool enableBounce = true;
    
    [Tooltip("Độ cao nhấp nhô (đơn vị Unity)")]
    public float bounceHeight = 0.5f;
    
    [Tooltip("Thời gian một chu kỳ nhấp nhô (giây)")]
    public float bounceDuration = 1f;
    
    [Tooltip("Easing cho hiệu ứng nhấp nhô")]
    public Ease bounceEase = Ease.InOutSine;
    
    [Header("Start Delay")]
    [Tooltip("Độ trễ trước khi bắt đầu animation (giây)")]
    public float startDelay = 0f;
    
    private Vector3 originalPosition;
    private Tween bounceTween;
    private Tween rotationTween;
    
    void Start()
    {
        originalPosition = transform.position;
        
        if (startDelay > 0f)
        {
            Invoke(nameof(StartAnimations), startDelay);
        }
        else
        {
            StartAnimations();
        }
    }
    
    private void StartAnimations()
    {
        if (enableRotation)
        {
            StartRotation();
        }
        
        if (enableBounce)
        {
            StartBounce();
        }
    }
    
    private void StartRotation()
    {
        // Xoay liên tục sử dụng DORotate với SetLoops(-1) để lặp vô hạn
        rotationTween = transform.DORotate(rotationAxis * 360f, 360f / rotationSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
    }
    
    private void StartBounce()
    {
        // Tạo hiệu ứng nhấp nhô lên xuống
        Vector3 topPosition = originalPosition + Vector3.up * bounceHeight;
        
        bounceTween = transform.DOMoveY(topPosition.y, bounceDuration)
            .SetEase(bounceEase)
            .SetLoops(-1, LoopType.Yoyo);
    }
    
    /// <summary>
    /// Dừng tất cả animations
    /// </summary>
    public void StopAnimations()
    {
        if (rotationTween != null)
        {
            rotationTween.Kill();
            rotationTween = null;
        }
        
        if (bounceTween != null)
        {
            bounceTween.Kill();
            bounceTween = null;
        }
    }
    
    /// <summary>
    /// Bắt đầu lại animations
    /// </summary>
    public void RestartAnimations()
    {
        StopAnimations();
        StartAnimations();
    }
    
    /// <summary>
    /// Tạm dừng animations (có thể tiếp tục)
    /// </summary>
    public void PauseAnimations()
    {
        if (rotationTween != null)
        {
            rotationTween.Pause();
        }
        
        if (bounceTween != null)
        {
            bounceTween.Pause();
        }
    }
    
    /// <summary>
    /// Tiếp tục animations sau khi tạm dừng
    /// </summary>
    public void ResumeAnimations()
    {
        if (rotationTween != null)
        {
            rotationTween.Play();
        }
        
        if (bounceTween != null)
        {
            bounceTween.Play();
        }
    }
    
    void OnDestroy()
    {
        StopAnimations();
    }
    
    void OnDisable()
    {
        // Tạm dừng khi object bị disable
        PauseAnimations();
    }
    
    void OnEnable()
    {
        // Tiếp tục khi object được enable lại
        if (rotationTween != null || bounceTween != null)
        {
            ResumeAnimations();
        }
        else if (originalPosition != Vector3.zero)
        {
            // Nếu chưa có animation, khởi động lại
            StartAnimations();
        }
    }
}
