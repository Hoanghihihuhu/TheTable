using UnityEngine;

/// <summary>
/// Vật phẩm tương tác để giả lập enemy bị tiêu diệt cho KillEnemiesStep.
/// - Gắn script này vào 1 vật phẩm trong scene.
/// - Khi player vào tầm và bấm F, script phát sự kiện OnEnemyKilled 1 lần duy nhất.
/// - Sau đó vẫn cho phép bấm F nhiều lần để đọc thông tin (chat), nhưng không tăng số kill nữa.
/// </summary>
public class QuestItemKillProxy : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Phím để tương tác")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Tooltip("Khoảng cách để player có thể tương tác")]
    [SerializeField] private float interactionRange = 3f;

    [Tooltip("Tag của Player GameObject")]
    [SerializeField] private string playerTag = "Player";

    [Header("Prompt Settings")]
    [Tooltip("Tin nhắn prompt khi vào tầm (ví dụ: 'Press F to interact')")]
    [SerializeField] private string promptMessage = "Press F to interact";
    [Tooltip("Tốc độ gõ prompt")]
    [SerializeField] private GameChatSpeed promptSpeed = GameChatSpeed.Fast;
    [Tooltip("Thời gian prompt tồn tại (0 = tồn tại lâu, sẽ bị thay bởi prompt mới khi vào lại)")]
    [SerializeField] private float promptStayDuration = 0f;
    [Tooltip("Offset prompt so với vật phẩm")]
    [SerializeField] private Vector3 promptOffset = new Vector3(0f, 2.5f, 0f);
    [Tooltip("Prompt có bám theo vật phẩm không")]
    [SerializeField] private bool promptFollowTarget = true;

    [Header("Kill Step Settings")]
    [Tooltip("enemyId sẽ gửi cho QuestEventSystem.TriggerEnemyKilled.\nPhải trùng với targetEnemyID trong KillEnemiesStep (hoặc để trống nếu KillEnemiesStep cho phép any enemy).")]
    [SerializeField] private string enemyId = "ItemEnemyProxy";

    [Tooltip("Chỉ phát sự kiện kill 1 lần duy nhất")]
    [SerializeField] private bool fireKillEventOnlyOnce = true;

    [Header("Info Chat Settings")]
    [Tooltip("Có dùng GameChatManager để hiển thị thông tin khi bấm F không?")]
    [SerializeField] private bool useGameChat = true;

    [Tooltip("Các dòng thông tin sẽ hiển thị khi bấm F")]
    [SerializeField] private string[] infoMessages = new string[] { "Một vật phẩm kỳ lạ...", "Có vẻ liên quan tới nhiệm vụ." };

    [Tooltip("Tốc độ gõ chat khi đọc thông tin")]
    [SerializeField] private GameChatSpeed infoChatSpeed = GameChatSpeed.Normal;

    [Tooltip("Thời gian mỗi dòng chat tồn tại")]
    [SerializeField] private float infoChatStayDuration = 3f;

    [Tooltip("Offset hiển thị chat so với vật phẩm")]
    [SerializeField] private Vector3 infoChatOffset = new Vector3(0f, 2f, 0f);

    [Tooltip("Chat có bám theo vật phẩm không")]
    [SerializeField] private bool infoChatFollowTarget = true;

    private GameObject _player;
    private bool _playerInRange = false;
    private bool _killEventFired = false;
    private bool _hasShownPrompt = false;
    private Coroutine _infoRoutine;

    private void Update()
    {
        UpdatePlayerInRange();

        if (_playerInRange && Input.GetKeyDown(interactKey))
        {
            HandleInteract();
        }
    }

    private void UpdatePlayerInRange()
    {
        if (_player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag(playerTag);
            if (found == null) return;
            _player = found;
        }

        float distance = Vector3.Distance(transform.position, _player.transform.position);
        bool wasInRange = _playerInRange;
        _playerInRange = distance <= interactionRange;

        if (!wasInRange && _playerInRange)
        {
            ShowPrompt();
        }
        else if (wasInRange && !_playerInRange)
        {
            _hasShownPrompt = false;
        }
    }

    private void HandleInteract()
    {
        // Phát sự kiện enemy killed (chỉ 1 lần nếu được cấu hình)
        if (!_killEventFired || !fireKillEventOnlyOnce)
        {
            QuestEventSystem.TriggerEnemyKilled(enemyId);
            if (fireKillEventOnlyOnce)
            {
                _killEventFired = true;
            }
        }

        // Luôn cho phép đọc thông tin (gửi tuần tự)
        if (useGameChat && GameChatManager.instance != null && infoMessages != null && infoMessages.Length > 0)
        {
            if (_infoRoutine != null) StopCoroutine(_infoRoutine);
            _infoRoutine = StartCoroutine(SendInfoSequential());
        }
    }

    private void ShowPrompt()
    {
        if (_hasShownPrompt || GameChatManager.instance == null || string.IsNullOrEmpty(promptMessage))
            return;

        _hasShownPrompt = true;
        float stay = promptStayDuration > 0f ? promptStayDuration : 999f;
        GameChatManager.instance.SendChat(
            promptMessage,
            transform,
            promptSpeed,
            stay,
            promptOffset,
            promptFollowTarget
        );
    }

    private System.Collections.IEnumerator SendInfoSequential()
    {
        float spacing = infoChatStayDuration + 0.5f;
        for (int i = 0; i < infoMessages.Length; i++)
        {
            string msg = infoMessages[i];
            if (!string.IsNullOrEmpty(msg))
            {
                GameChatManager.instance.SendChat(
                    msg,
                    transform,
                    infoChatSpeed,
                    infoChatStayDuration,
                    infoChatOffset,
                    infoChatFollowTarget
                );
            }
            if (i < infoMessages.Length - 1 && spacing > 0f)
            {
                yield return new WaitForSeconds(spacing);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}

