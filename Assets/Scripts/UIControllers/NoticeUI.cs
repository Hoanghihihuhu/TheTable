using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class NoticeUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Vector3 startScale = new Vector3(0.5f, 0.5f, 0.5f);

    private Tween currentTween;

    private void OnEnable()
    {
        GameEvents.OnShowNotice += ShowNotice;
    }

    private void OnDisable()
    {
        GameEvents.OnShowNotice -= ShowNotice;
    }

    private void Start()
    {
        canvasGroup.alpha = 0f;
        transform.localScale = startScale;
    }

    private void ShowNotice(NoticeData data)
    {
        if (currentTween != null && currentTween.IsActive()) currentTween.Kill();

        messageText.text = data.message;
        messageText.color = data.color;
        messageText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(canvasGroup.GetComponent<RectTransform>());
        if (data.sound && audioSource) audioSource.PlayOneShot(data.sound);

        canvasGroup.alpha = 0f;
        transform.localScale = startScale;

        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, 0.25f));
        seq.Join(transform.DOScale(data.scale, 0.35f).SetEase(Ease.OutBack));
        seq.AppendInterval(data.duration);
        seq.Append(canvasGroup.DOFade(0f, 0.3f));
        seq.Join(transform.DOScale(0.8f, 0.3f).SetEase(Ease.InBack));

        currentTween = seq;
    }
}
