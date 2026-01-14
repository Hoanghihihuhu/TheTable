using System.Collections.Generic;
using UnityEngine;

public enum GameChatSpeed
{
    Slow,
    Normal,
    Fast
}

/// <summary>
/// Quản lý chat bubble trong game world.
/// Cách dùng:
/// GameChatManager.instance.SendChat("Xin chào", talkerTransform);
/// </summary>
public class GameChatManager : MonoBehaviour
{
    public static GameChatManager instance { get; private set; }

    [Header("Canvas Settings")]
    [Tooltip("Canvas game world (World Space hoặc Screen Space) dùng để chứa chat bubble.")]
    public RectTransform canvasRoot;

    [Header("Prefab & Pool Settings")]
    [Tooltip("Prefab chat bubble đã có sẵn (chứa TextMeshPro).")]
    public GameChatBubble messagePrefab;
    [Tooltip("Số lượng object tạo sẵn trong pool.")]
    public int initialPoolSize = 10;
    [Tooltip("Transform chứa các chat bubble rảnh rỗi. Nếu để trống sẽ tự tạo dưới canvasRoot.")]
    public Transform poolRoot;

    [Header("Typing Speed (seconds / character)")]
    public float slowTypingSpeed = 0.08f;
    public float normalTypingSpeed = 0.04f;
    public float fastTypingSpeed = 0.02f;

    [Header("Display Settings")]
    [Tooltip("Thời gian tồn tại sau khi gõ xong.")]
    public float defaultStayDuration = 2f;
    [Tooltip("Offset so với vị trí của talker (ví dụ (0, 2, 0) để hiện trên đầu).")]
    public Vector3 defaultOffset = new Vector3(0f, 2f, 0f);

    private readonly Queue<GameChatBubble> _pool = new Queue<GameChatBubble>();
    private readonly HashSet<GameChatBubble> _inPool = new HashSet<GameChatBubble>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Tạo poolRoot nếu chưa có
        if (poolRoot == null)
        {
            GameObject root = new GameObject("GameChatPool");
            Transform parent = canvasRoot != null ? canvasRoot : transform;
            root.transform.SetParent(parent, false);
            poolRoot = root.transform;
        }

        // Khởi tạo pool
        if (messagePrefab != null)
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameChatBubble bubble = Instantiate(messagePrefab, poolRoot);
                bubble.gameObject.SetActive(false);
                bubble.Manager = this;
                _pool.Enqueue(bubble);
                _inPool.Add(bubble);
            }
        }
    }

    /// <summary>
    /// Gửi chat với tốc độ mặc định (Normal), mặc định KHÔNG bám theo mục tiêu.
    /// </summary>
    public void SendChat(string message, Transform talker, bool followTarget = false)
    {
        SendChat(message, talker, GameChatSpeed.Normal, defaultStayDuration, null, followTarget);
    }

    /// <summary>
    /// Gửi chat với cấu hình chi tiết.
    /// </summary>
    /// <param name="message">Nội dung chat.</param>
    /// <param name="talker">Transform nhân vật.</param>
    /// <param name="speed">Tốc độ gõ: Slow/Normal/Fast.</param>
    /// <param name="stayDuration">Thời gian tồn tại sau khi gõ xong.</param>
    /// <param name="offset">Offset world-space so với talker. Nếu null dùng defaultOffset.</param>
    /// <param name="followTarget">Có bám theo talker hay không (mặc định false).</param>
    public void SendChat(string message, Transform talker, GameChatSpeed speed, float stayDuration, Vector3? offset = null, bool followTarget = false)
    {
        if (string.IsNullOrEmpty(message) || talker == null || messagePrefab == null)
            return;

        GameChatBubble bubble = GetBubbleFromPool();

        // Luôn spawn dưới canvas (game world)
        Transform parent = canvasRoot != null ? canvasRoot : poolRoot;
        bubble.transform.SetParent(parent, false);

        Vector3 worldOffset = offset ?? defaultOffset;

        // Thiết lập bám / không bám
        if (followTarget)
        {
            bubble.ConfigureFollow(talker, true, worldOffset);
        }
        else
        {
            bubble.ConfigureFollow(null, false, Vector3.zero);
            // Đặt cố định tại vị trí hiện tại của talker
            bubble.transform.position = talker.position + worldOffset;
        }

        float typingSpeed = GetTypingSpeed(speed);
        bubble.gameObject.SetActive(true);
        bubble.Show(message, typingSpeed, stayDuration);
    }

    private float GetTypingSpeed(GameChatSpeed speed)
    {
        switch (speed)
        {
            case GameChatSpeed.Slow:
                return slowTypingSpeed;
            case GameChatSpeed.Fast:
                return fastTypingSpeed;
            case GameChatSpeed.Normal:
            default:
                return normalTypingSpeed;
        }
    }

    private GameChatBubble GetBubbleFromPool()
    {
        // Bóc các phần tử null (phòng trường hợp object bị destroy)
        while (_pool.Count > 0)
        {
            GameChatBubble candidate = _pool.Dequeue();
            if (candidate == null) continue;

            _inPool.Remove(candidate);
            candidate.Manager = this;
            return candidate;
        }

        GameChatBubble bubble = Instantiate(messagePrefab, poolRoot);
        bubble.Manager = this;
        return bubble;
    }

    /// <summary>
    /// Được gọi bởi GameChatBubble khi kết thúc hiển thị.
    /// </summary>
    public void ReturnToPool(GameChatBubble bubble)
    {
        if (bubble == null) return;

        if (!bubble.gameObject.scene.IsValid())
        {
            return;
        }

        if (_inPool.Contains(bubble))
        {
            bubble.gameObject.SetActive(false);
            return;
        }

        bubble.transform.SetParent(poolRoot, false);
        bubble.gameObject.SetActive(false);
        _pool.Enqueue(bubble);
        _inPool.Add(bubble);
    }
}

