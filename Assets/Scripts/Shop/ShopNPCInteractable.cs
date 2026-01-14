using UnityEngine;

/// <summary>
/// Script gắn vào NPC merchant để tương tác và mở cửa hàng.
/// - Hiển thị "press f to interact" khi player vào tầm
/// - Khi press F → mở ShopUI
/// </summary>
public class ShopNPCInteractable : MonoBehaviour
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
    [Tooltip("Tin nhắn prompt hiển thị khi vào tầm")]
    [SerializeField] private string promptMessage = "Press F to open shop";
    [Tooltip("Tốc độ gõ prompt")]
    [SerializeField] private GameChatSpeed promptSpeed = GameChatSpeed.Fast;
    [Tooltip("Thời gian prompt tồn tại. Đặt 0 để prompt tồn tại mãi.")]
    [SerializeField] private float promptStayDuration = 0f;
    
    [Header("Display Settings")]
    [Tooltip("Offset so với NPC")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, 0f);
    [Tooltip("Có bám theo NPC không")]
    [SerializeField] private bool followTarget = true;
    
    private bool _isPlayerInRange = false;
    private GameObject _playerObject;
    private bool _hasShownPrompt = false;
    private bool _isInteracting = false;
    public float deviation = 5f;
    
    private ShopUI shopUI;
    
    void Start()
    {
        // Tìm ShopUI trong scene
        shopUI = FindObjectOfType<ShopUI>();
        if (shopUI == null)
        {
            Debug.LogError("ShopNPCInteractable: Không tìm thấy ShopUI trong scene!");
        }
    }
    
    private void Update()
    {
        // Kiểm tra khoảng cách với player
        CheckPlayerDistance();
        
        // Xử lý input F
        if (_isPlayerInRange && !_isInteracting && Input.GetKeyDown(interactKey))
        {
            Interact();
        }
        
        // Xử lý đóng shop bằng button 5 (hoặc có thể dùng ESC)
        if (shopUI != null && shopUI.IsShopOpen() && Input.GetKeyDown(KeyCode.Alpha5))
        {
            shopUI.CloseShop();
            _isInteracting = false;
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
        
        // Đóng shop nếu đang mở
        if (shopUI != null && shopUI.IsShopOpen())
        {
            shopUI.CloseShop();
            _isInteracting = false;
        }
    }
    
    private void ShowPrompt()
    {
        if (GameChatManager.instance == null || string.IsNullOrEmpty(promptMessage))
            return;
        
        _hasShownPrompt = true;
        
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
    }
    
    /// <summary>
    /// Gọi khi player nhấn F để interact
    /// </summary>
    public void Interact()
    {
        if (_isInteracting || shopUI == null)
            return;
        
        _isInteracting = true;
        
        // Ẩn prompt
        HidePrompt();
        
        // Mở shop UI
        shopUI.OpenShop();
    }
    
    void OnDrawGizmosSelected()
    {
        // Vẽ interaction range trong editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
