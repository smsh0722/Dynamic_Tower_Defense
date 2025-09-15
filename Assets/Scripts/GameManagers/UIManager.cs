using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Refs")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI notifyText;
    [SerializeField] private TextMeshProUGUI centerText;

    private const float NOTIFY_DEFAULT_DURATION = 1.2f;
    private float mNotifyLeftTime = 0f;
    private float mCenterNotifyLeftTime = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        UpdateHealth(GameManager.Instance.PlayerHealth);
        UpdateMoney(GameManager.Instance.CurrentMoney);
        if (centerText != null)
            centerText.gameObject.SetActive(false);
    }
    private void Update()
    {
        mNotifyLeftTime -= Time.deltaTime;
        mCenterNotifyLeftTime -= Time.deltaTime;
        if (mNotifyLeftTime <= 0f)
            notifyText.text = "";
        if (mCenterNotifyLeftTime <= 0f)
        {
            centerText.text = "";
            centerText.gameObject.SetActive(false);
        }

    }

    public void UpdateHealth(int value)
    {
        healthText.text = $"HP: {value}";
    }

    public void UpdateMoney(int value)
    {
        moneyText.text = $"$ {value:N0}";
    }

    public void NotifyMessage( string message, float duration = 0.0f )
    {
        notifyText.text = message;
        if (duration > 0.0f)
            mNotifyLeftTime = duration;
        else
            mNotifyLeftTime = NOTIFY_DEFAULT_DURATION;
    }
    public void NotifyMessageAtCenter( string message, float duration = 0.0f )
    {
        centerText.gameObject.SetActive(true);
        centerText.text = message;
        if (duration > 0.0f)
            mCenterNotifyLeftTime = duration;
        else
            mCenterNotifyLeftTime = NOTIFY_DEFAULT_DURATION;
    }   
}
