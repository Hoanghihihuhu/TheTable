using UnityEngine;

/// <summary>
/// Script gắn vào NPC để tương tác với player.
/// - Hiển thị "press f to interact" khi player vào tầm (chỉ 1 lần, bằng GameChatManager)
/// - Reset khi player ra khỏi tầm
/// - Khi press F → hiển thị chat bằng GameChatManager
/// </summary>
public class NPCGameChatInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Phím để tương tác")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;
    
    [Header("Proximity Settings")]
    [Tooltip("Khoảng cách để phát hiện player")]
    [SerializeField] private float interactionRange = 3f;
    [Tooltip("Tag của Player GameObject")]
    [SerializeField] private string playerTag = "Player";

    [Header("Prompt Settings")]
    [Tooltip("Tin nhắn prompt hiển thị khi vào tầm (ví dụ: 'Press F to interact')")]
    [SerializeField] private string promptMessage = "Press F to interact";
    [Tooltip("Tốc độ gõ prompt")]
    [SerializeField] private GameChatSpeed promptSpeed = GameChatSpeed.Fast;
    [Tooltip("Thời gian prompt tồn tại (sẽ tự fade sau thời gian này). Đặt 0 để prompt tồn tại mãi.")]
    [SerializeField] private float promptStayDuration = 0f;

    [Header("Chat Settings")]
    [Tooltip("Danh sách tin nhắn chat khi interact")]
    [SerializeField] private string[] chatMessages = new string[0];
    [Tooltip("Tốc độ gõ chat")]
    [SerializeField] private GameChatSpeed chatSpeed = GameChatSpeed.Normal;
    [Tooltip("Thời gian mỗi tin nhắn tồn tại")]
    [SerializeField] private float chatStayDuration = 3f;

    [Header("Display Settings")]
    [Tooltip("Offset so với NPC (dùng cho cả prompt và chat)")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);
    [Tooltip("Có bám theo NPC không (dùng cho cả prompt và chat)")]
    [SerializeField] private bool followTarget = true;

    [Header("Events")]
    [Tooltip("Gọi khi player bắt đầu interact")]
    public UnityEngine.Events.UnityEvent OnInteractStarted;
    [Tooltip("Gọi khi tất cả chat hoàn thành")]
    public UnityEngine.Events.UnityEvent OnAllChatsCompleted;

    private bool _isPlayerInRange = false;
    private GameObject _playerObject;
    private bool _hasShownPrompt = false;
    private bool _isInteracting = false;
    public float deviation = 5f; 

    private void Update()
    {
        // Kiểm tra khoảng cách với player
        CheckPlayerDistance();

        // Xử lý input F
        if (_isPlayerInRange && !_isInteracting && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
    }

    private void CheckPlayerDistance()
    {
        if (_playerObject == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag(playerTag);
            if (foundPlayer == null) return;
            _playerObject = foundPlayer;
        }

        float distance = Vector3.Distance(transform.position, _playerObject.transform.position);
        bool wasInRange = _isPlayerInRange;
        _isPlayerInRange = distance <= interactionRange + deviation;

        // Player vào tầm
        if (!wasInRange && _isPlayerInRange)
        {
            OnPlayerEnterRange(_playerObject);
        }
        // Player ra khỏi tầm
        else if (wasInRange && !_isPlayerInRange)
        {
            OnPlayerExitRange();
        }
    }

    private void OnPlayerEnterRange(GameObject player)
    {
        _isPlayerInRange = true;
        _playerObject = player;

        // Hiển thị prompt (chỉ 1 lần)
        if (!_hasShownPrompt && GameChatManager.instance != null)
        {
            ShowPrompt();
        }
    }

    private void OnPlayerExitRange()
    {
        _isPlayerInRange = false;
        _playerObject = null;

        // Ẩn prompt và reset
        HidePrompt();
        _hasShownPrompt = false;
    }

    private void ShowPrompt()
    {
        if (GameChatManager.instance == null || string.IsNullOrEmpty(promptMessage))
            return;

        _hasShownPrompt = true;

        // Nếu promptStayDuration = 0, prompt sẽ tồn tại mãi (999 giây)
        // Nếu > 0, prompt sẽ tự fade sau thời gian đó
        float stayDuration = promptStayDuration > 0f ? promptStayDuration : 999f;
        
        GameChatManager.instance.SendChat(
            promptMessage,
            transform,
            promptSpeed,
            stayDuration,
            offset,
            followTarget
        );
    }

    private void HidePrompt()
    {
        // Prompt sẽ tự fade sau stayDuration hoặc khi được return pool
        // Không cần làm gì thêm vì GameChatManager tự quản lý pool
    }

    /// <summary>
    /// Gọi khi player nhấn F để interact
    /// </summary>
    public void Interact()
    {
        if (_isInteracting || GameChatManager.instance == null || chatMessages.Length == 0)
            return;

        _isInteracting = true;

        // Ẩn prompt
        HidePrompt();

        // Fire event
        OnInteractStarted?.Invoke();

        // Hiển thị chat messages
        ShowChatMessages();
    }

    private void ShowChatMessages()
    {
        if (chatMessages.Length == 0)
        {
            _isInteracting = false;
            OnAllChatsCompleted?.Invoke();
            return;
        }

        // Gửi từng message
        for (int i = 0; i < chatMessages.Length; i++)
        {
            string message = chatMessages[i];
            float delay = i * (chatStayDuration + 0.5f); // Delay giữa các message

            if (delay > 0f)
            {
                StartCoroutine(SendChatDelayed(message, delay));
            }
            else
            {
                SendChatMessage(message);
            }
        }

        // Tính tổng thời gian để fire event khi hoàn thành
        float totalDuration = chatMessages.Length * (chatStayDuration + 0.5f);
        StartCoroutine(WaitForChatsComplete(totalDuration));
    }

    private System.Collections.IEnumerator SendChatDelayed(string message, float delay)
    {
        yield return new WaitForSeconds(delay);
        SendChatMessage(message);
    }

    private void SendChatMessage(string message)
    {
        if (GameChatManager.instance == null || string.IsNullOrEmpty(message))
            return;

        GameChatManager.instance.SendChat(
            message,
            transform,
            chatSpeed,
            chatStayDuration,
            offset,
            followTarget
        );
    }

    private System.Collections.IEnumerator WaitForChatsComplete(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        _isInteracting = false;
        OnAllChatsCompleted?.Invoke();
    }

    void OnDrawGizmosSelected()
    {
        // Vẽ interaction range trong editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
