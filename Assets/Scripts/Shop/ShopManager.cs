using UnityEngine;
using System;

/// <summary>
/// Hệ thống quản lý cửa hàng - mua items bằng tiền
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("Item Prices")]
    [SerializeField] private int healCost = 100;
    [SerializeField] private float healAmount = 20f;
    
    [SerializeField] private int grenadeCost = 500;
    [SerializeField] private int molotovCost = 500;
    [SerializeField] private int c4Cost = 1000;
    
    [Header("Starting Money")]
    [SerializeField] private int startingMoney = 10000;
    
    private int currentMoney = 0;
    
    // Events
    public event Action<int> OnMoneyChanged; // newMoneyAmount
    public event Action<string> OnPurchaseSuccess; // itemName
    public event Action<string> OnPurchaseFailed; // reason
    
    // Singleton pattern
    public static ShopManager Instance { get; private set; }
    
    // Property để lấy số tiền hiện có (cho UI)
    public int MyWallet => currentMoney;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentMoney = startingMoney;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    /// <summary>
    /// Thêm tiền vào ví
    /// </summary>
    public void AddMoney(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("ShopManager: Không thể thêm số tiền âm!");
            return;
        }
        
        currentMoney += amount;
        OnMoneyChanged?.Invoke(currentMoney);
        Debug.Log($"ShopManager: Thêm {amount} tiền. Tổng: {currentMoney}");
    }
    
    /// <summary>
    /// Mua hồi máu
    /// </summary>
    public bool BuyHeal()
    {
        if (currentMoney < healCost)
        {
            OnPurchaseFailed?.Invoke("Không đủ tiền!");
            Debug.LogWarning($"ShopManager: Không đủ tiền để mua hồi máu! Cần {healCost}, có {currentMoney}");
            return false;
        }
        
        // Tìm Player
        Health playerHealth = GetPlayerHealth();
        if (playerHealth == null)
        {
            OnPurchaseFailed?.Invoke("Không tìm thấy Player!");
            Debug.LogError("ShopManager: Không tìm thấy Player Health component!");
            return false;
        }
        
        // Kiểm tra player đã chết chưa
        if (playerHealth.IsDead)
        {
            OnPurchaseFailed?.Invoke("Player đã chết!");
            Debug.LogWarning("ShopManager: Không thể hồi máu cho player đã chết!");
            return false;
        }
        
        // Trừ tiền
        currentMoney -= healCost;
        OnMoneyChanged?.Invoke(currentMoney);
        
        // Hồi máu
        playerHealth.Heal(healAmount);
        
        OnPurchaseSuccess?.Invoke("Hồi máu");
        Debug.Log($"ShopManager: Mua hồi máu thành công! Hồi {healAmount} máu. Tiền còn lại: {currentMoney}");
        return true;
    }
    
    /// <summary>
    /// Mua Grenade
    /// </summary>
    public bool BuyGrenade()
    {
        if (currentMoney < grenadeCost)
        {
            OnPurchaseFailed?.Invoke("Không đủ tiền!");
            Debug.LogWarning($"ShopManager: Không đủ tiền để mua Grenade! Cần {grenadeCost}, có {currentMoney}");
            return false;
        }
        
        if (InventorySystem.Instance == null)
        {
            OnPurchaseFailed?.Invoke("Hệ thống inventory không khả dụng!");
            Debug.LogError("ShopManager: InventorySystem.Instance không tồn tại!");
            return false;
        }
        
        // Kiểm tra inventory đầy chưa
        if (InventorySystem.Instance.IsFull(WeaponType.Grenade))
        {
            OnPurchaseFailed?.Invoke("Grenade đã đầy!");
            Debug.LogWarning("ShopManager: Grenade đã đạt max stack!");
            return false;
        }
        
        // Trừ tiền
        currentMoney -= grenadeCost;
        OnMoneyChanged?.Invoke(currentMoney);
        
        // Thêm vào inventory
        InventorySystem.Instance.AddItem(WeaponType.Grenade, 1);
        
        OnPurchaseSuccess?.Invoke("Grenade");
        Debug.Log($"ShopManager: Mua Grenade thành công! Tiền còn lại: {currentMoney}");
        return true;
    }
    
    /// <summary>
    /// Mua Molotov
    /// </summary>
    public bool BuyMolotov()
    {
        if (currentMoney < molotovCost)
        {
            OnPurchaseFailed?.Invoke("Không đủ tiền!");
            Debug.LogWarning($"ShopManager: Không đủ tiền để mua Molotov! Cần {molotovCost}, có {currentMoney}");
            return false;
        }
        
        if (InventorySystem.Instance == null)
        {
            OnPurchaseFailed?.Invoke("Hệ thống inventory không khả dụng!");
            Debug.LogError("ShopManager: InventorySystem.Instance không tồn tại!");
            return false;
        }
        
        // Kiểm tra inventory đầy chưa
        if (InventorySystem.Instance.IsFull(WeaponType.Molotov))
        {
            OnPurchaseFailed?.Invoke("Molotov đã đầy!");
            Debug.LogWarning("ShopManager: Molotov đã đạt max stack!");
            return false;
        }
        
        // Trừ tiền
        currentMoney -= molotovCost;
        OnMoneyChanged?.Invoke(currentMoney);
        
        // Thêm vào inventory
        InventorySystem.Instance.AddItem(WeaponType.Molotov, 1);
        
        OnPurchaseSuccess?.Invoke("Molotov");
        Debug.Log($"ShopManager: Mua Molotov thành công! Tiền còn lại: {currentMoney}");
        return true;
    }
    
    /// <summary>
    /// Mua C4
    /// </summary>
    public bool BuyC4()
    {
        if (currentMoney < c4Cost)
        {
            OnPurchaseFailed?.Invoke("Không đủ tiền!");
            Debug.LogWarning($"ShopManager: Không đủ tiền để mua C4! Cần {c4Cost}, có {currentMoney}");
            return false;
        }
        
        if (InventorySystem.Instance == null)
        {
            OnPurchaseFailed?.Invoke("Hệ thống inventory không khả dụng!");
            Debug.LogError("ShopManager: InventorySystem.Instance không tồn tại!");
            return false;
        }
        
        // Kiểm tra inventory đầy chưa
        if (InventorySystem.Instance.IsFull(WeaponType.C4))
        {
            OnPurchaseFailed?.Invoke("C4 đã đầy!");
            Debug.LogWarning("ShopManager: C4 đã đạt max stack!");
            return false;
        }
        
        // Trừ tiền
        currentMoney -= c4Cost;
        OnMoneyChanged?.Invoke(currentMoney);
        
        // Thêm vào inventory
        InventorySystem.Instance.AddItem(WeaponType.C4, 1);
        
        OnPurchaseSuccess?.Invoke("C4");
        Debug.Log($"ShopManager: Mua C4 thành công! Tiền còn lại: {currentMoney}");
        return true;
    }
    
    /// <summary>
    /// Tìm và lấy Health component của Player
    /// </summary>
    private Health GetPlayerHealth()
    {
        // Tìm bằng tag trước
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        
        // Nếu không tìm thấy, thử tìm bằng component PlayerMovement
        if (player == null)
        {
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null)
            {
                player = playerMovement.gameObject;
            }
        }
        
        if (player != null)
        {
            return player.GetComponent<Health>();
        }
        
        return null;
    }
    
    /// <summary>
    /// Set số tiền (dùng cho testing hoặc save/load)
    /// </summary>
    public void SetMoney(int amount)
    {
        currentMoney = Mathf.Max(0, amount);
        OnMoneyChanged?.Invoke(currentMoney);
    }
}
