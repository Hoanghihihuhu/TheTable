using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// UI cửa hàng - quản lý hiển thị và tương tác với shop
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TextMeshProUGUI moneyText;
    
    [Header("Item Buttons")]
    [SerializeField] private Button healButton;
    [SerializeField] private Button grenadeButton;
    [SerializeField] private Button molotovButton;
    [SerializeField] private Button c4Button;
    [SerializeField] private Button closeButton;
    
    [Header("Item Price Displays")]
    [SerializeField] private TextMeshProUGUI healPriceText;
    [SerializeField] private TextMeshProUGUI grenadePriceText;
    [SerializeField] private TextMeshProUGUI molotovPriceText;
    [SerializeField] private TextMeshProUGUI c4PriceText;
    
    [Header("Error Message")]
    [SerializeField] private GameObject errorMessagePanel;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private float errorMessageDuration = 2f;
    
    [Header("Shake Settings")]
    [SerializeField] private float shakeStrength = 10f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeDuration = 0.5f;
    
    [Header("Color Settings")]
    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color insufficientMoneyColor = Color.red;
    
    [Header("Shop Manager Reference")]
    [SerializeField] private ShopManager shopManager;
    
    private bool isShopOpen = false;
    private Tween errorMessageTween;
    
    void Start()
    {
        // Tìm ShopManager nếu chưa được gán
        if (shopManager == null)
        {
            shopManager = FindObjectOfType<ShopManager>();
        }
        
        // Ẩn shop panel ban đầu
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        
        // Ẩn error message
        if (errorMessagePanel != null)
        {
            errorMessagePanel.SetActive(false);
        }
        
        // Setup button listeners
        if (healButton != null)
        {
            healButton.onClick.AddListener(OnHealButtonClicked);
        }
        
        if (grenadeButton != null)
        {
            grenadeButton.onClick.AddListener(OnGrenadeButtonClicked);
        }
        
        if (molotovButton != null)
        {
            molotovButton.onClick.AddListener(OnMolotovButtonClicked);
        }
        
        if (c4Button != null)
        {
            c4Button.onClick.AddListener(OnC4ButtonClicked);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }
        
        // Subscribe to ShopManager events
        if (shopManager != null)
        {
            shopManager.OnMoneyChanged += UpdateMoneyDisplay;
            shopManager.OnPurchaseSuccess += OnPurchaseSuccess;
            shopManager.OnPurchaseFailed += OnPurchaseFailed;
            
            // Update initial money display
            UpdateMoneyDisplay(shopManager.MyWallet);
        }
        else
        {
            Debug.LogWarning("ShopUI: Không tìm thấy ShopManager trong scene!");
        }
        
        // Update price displays
        UpdatePriceDisplays();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (shopManager != null)
        {
            shopManager.OnMoneyChanged -= UpdateMoneyDisplay;
            shopManager.OnPurchaseSuccess -= OnPurchaseSuccess;
            shopManager.OnPurchaseFailed -= OnPurchaseFailed;
        }
    }
    
    /// <summary>
    /// Mở cửa hàng
    /// </summary>
    public void OpenShop()
    {
        if (shopPanel == null) return;
        
        isShopOpen = true;
        shopPanel.SetActive(true);
        
        if (shopManager != null)
        {
            UpdateMoneyDisplay(shopManager.MyWallet);
        }
        
        UpdateButtonStates();
    }
    
    /// <summary>
    /// Đóng cửa hàng
    /// </summary>
    public void CloseShop()
    {
        if (shopPanel == null) return;
        
        isShopOpen = false;
        shopPanel.SetActive(false);
    }
    
    /// <summary>
    /// Cập nhật hiển thị số tiền
    /// </summary>
    private void UpdateMoneyDisplay(int money)
    {
        if (moneyText != null)
        {
            moneyText.text = money.ToString();
        }
        
        if (isShopOpen)
        {
            UpdateButtonStates();
        }
    }
    
    /// <summary>
    /// Cập nhật trạng thái các button (màu sắc dựa trên số tiền)
    /// </summary>
    private void UpdateButtonStates()
    {
        if (shopManager == null) return;
        
        int currentMoney = shopManager.MyWallet;
        
        // Update heal button
        UpdateButtonColor(healButton, currentMoney >= 100);
        
        // Update grenade button
        UpdateButtonColor(grenadeButton, currentMoney >= 500);
        
        // Update molotov button
        UpdateButtonColor(molotovButton, currentMoney >= 500);
        
        // Update c4 button
        UpdateButtonColor(c4Button, currentMoney >= 1000);
    }
    
    /// <summary>
    /// Cập nhật màu button dựa trên có đủ tiền không
    /// </summary>
    private void UpdateButtonColor(Button button, bool canAfford)
    {
        if (button == null) return;
        
        var colors = button.colors;
        colors.normalColor = canAfford ? normalButtonColor : insufficientMoneyColor;
        button.colors = colors;
    }
    
    /// <summary>
    /// Cập nhật hiển thị giá
    /// </summary>
    private void UpdatePriceDisplays()
    {
        if (healPriceText != null)
        {
            healPriceText.text = "100";
        }
        
        if (grenadePriceText != null)
        {
            grenadePriceText.text = "500";
        }
        
        if (molotovPriceText != null)
        {
            molotovPriceText.text = "500";
        }
        
        if (c4PriceText != null)
        {
            c4PriceText.text = "1000";
        }
    }
    
    /// <summary>
    /// Xử lý khi click button Heal
    /// </summary>
    private void OnHealButtonClicked()
    {
        if (shopManager == null) return;
        
        bool success = shopManager.BuyHeal();
        if (!success)
        {
            ShakeButton(healButton);
        }
    }
    
    /// <summary>
    /// Xử lý khi click button Grenade
    /// </summary>
    private void OnGrenadeButtonClicked()
    {
        if (shopManager == null) return;
        
        bool success = shopManager.BuyGrenade();
        if (!success)
        {
            ShakeButton(grenadeButton);
        }
    }
    
    /// <summary>
    /// Xử lý khi click button Molotov
    /// </summary>
    private void OnMolotovButtonClicked()
    {
        if (shopManager == null) return;
        
        bool success = shopManager.BuyMolotov();
        if (!success)
        {
            ShakeButton(molotovButton);
        }
    }
    
    /// <summary>
    /// Xử lý khi click button C4
    /// </summary>
    private void OnC4ButtonClicked()
    {
        if (shopManager == null) return;
        
        bool success = shopManager.BuyC4();
        if (!success)
        {
            ShakeButton(c4Button);
        }
    }
    
    /// <summary>
    /// Shake button khi không đủ tiền
    /// </summary>
    private void ShakeButton(Button button)
    {
        if (button == null) return;
        
        // Shake effect
        button.transform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, 90f, false, true)
            .SetEase(Ease.OutQuad);
    }
    
    /// <summary>
    /// Xử lý khi mua thành công
    /// </summary>
    private void OnPurchaseSuccess(string itemName)
    {
        // Có thể thêm feedback tích cực ở đây nếu cần
        UpdateButtonStates();
    }
    
    /// <summary>
    /// Xử lý khi mua thất bại
    /// </summary>
    private void OnPurchaseFailed(string reason)
    {
        ShowErrorMessage(reason);
    }
    
    /// <summary>
    /// Hiển thị thông báo lỗi
    /// </summary>
    private void ShowErrorMessage(string message)
    {
        if (errorMessagePanel == null || errorMessageText == null) return;
        
        // Kill previous tween if exists
        if (errorMessageTween != null && errorMessageTween.IsActive())
        {
            errorMessageTween.Kill();
        }
        
        // Show error message
        errorMessageText.text = message;
        errorMessagePanel.SetActive(true);
        
        // Auto hide after duration
        errorMessageTween = DOVirtual.DelayedCall(errorMessageDuration, () =>
        {
            if (errorMessagePanel != null)
            {
                errorMessagePanel.SetActive(false);
            }
        });
    }
    
    /// <summary>
    /// Kiểm tra shop có đang mở không
    /// </summary>
    public bool IsShopOpen()
    {
        return isShopOpen;
    }
}
