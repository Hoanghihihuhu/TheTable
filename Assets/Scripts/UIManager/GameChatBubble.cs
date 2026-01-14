using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Component gắn trên prefab chat bubble, xử lý typing, bám mục tiêu & vòng đời.
/// </summary>
public class GameChatBubble : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI messageText;
    public CanvasGroup canvasGroup;

    [Header("Fade Settings")]
    public float fadeOutDuration = 0.3f;

    [HideInInspector]
    public GameChatManager Manager;

    // Theo dõi mục tiêu (tùy chọn)
    private Transform _followTarget;
    private Vector3 _followOffset;
    private bool _isFollowing;

    private Coroutine _routine;

    private void OnEnable()
    {
        // Reset alpha mỗi lần dùng lại
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }
    }

    private void Update()
    {
        if (_isFollowing && _followTarget != null)
        {
            // Canvas game world: dùng trực tiếp world position
            transform.position = _followTarget.position + _followOffset;
        }
    }

    /// <summary>
    /// Thiết lập bám theo 1 Transform hay không.
    /// </summary>
    public void ConfigureFollow(Transform target, bool follow, Vector3 offset)
    {
        _followTarget = target;
        _isFollowing = follow;
        _followOffset = offset;

        if (_isFollowing && _followTarget != null)
        {
            transform.position = _followTarget.position + _followOffset;
        }
    }

    public void Show(string message, float typingSpeed, float stayDuration)
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }

        _routine = StartCoroutine(ShowRoutine(message, typingSpeed, stayDuration));
    }

    private IEnumerator ShowRoutine(string message, float typingSpeed, float stayDuration)
    {
        if (messageText != null)
        {
            messageText.text = "";

            // Hiệu ứng typing
            for (int i = 0; i <= message.Length; i++)
            {
                messageText.text = message.Substring(0, i);
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        // Đợi sau khi gõ xong
        if (stayDuration > 0f)
        {
            yield return new WaitForSeconds(stayDuration);
        }

        // Fade out nếu có CanvasGroup
        if (canvasGroup != null && fadeOutDuration > 0f)
        {
            float t = 0f;
            float startAlpha = canvasGroup.alpha;

            while (t < fadeOutDuration)
            {
                t += Time.deltaTime;
                float lerp = Mathf.Clamp01(t / fadeOutDuration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, lerp);
                yield return null;
            }
        }

        // Trả về pool
        Manager?.ReturnToPool(this);
    }
}