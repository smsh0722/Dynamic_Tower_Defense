using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

// 게임의 전반적인 상태(플레이어 목숨, 재화 등)를 관리하는 싱글톤 매니저
public class GameManager : MonoBehaviour
{
    [Header("Game")]
    public static GameManager Instance { get; private set; }
    public Tilemap Tilemap;

    [Header("Player")]
    public int PlayerHealth = 20;
    public int CurrentMoney = 100000;

    private Camera mMainCamera;

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
        mMainCamera = Camera.main;
        // UI
        UIManager.Instance.UpdateHealth(PlayerHealth);
        UIManager.Instance.UpdateMoney(CurrentMoney);
    }

    void Update()
    {
        if ( WaveManager.Instance.IsWin() == true )
        {
            Win();
            return;
        }

        // Input Mouse
        if (Mouse.current != null)
        {
            // UI 위인지 확인
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            var mousePos = Mouse.current.position.ReadValue();
            var worldPos = mMainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -mMainCamera.transform.position.z));
            Vector3Int gridPosition = Tilemap.WorldToCell(worldPos);
            Debug.Log("Mouse gridPosition: " + gridPosition);

            // Hover UI
            MapManager.Instance.HightlightCell(worldPos);

            // 왼쪽 버튼 클릭
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Tower t;
                if (TowerManager.Instance.TryGetTower(gridPosition, out t))
                {
                    Debug.Log("Tower clicked!");
                    t.OnClick();
                }
                else 
                    TowerManager.Instance.TryPlaceTower(gridPosition);
            }
        }
    }

    public void ApplyDamage(int damage)
    {
        PlayerHealth -= damage;
        Debug.Log("Player Health: " + PlayerHealth);
        UIManager.Instance.UpdateHealth(PlayerHealth);

        if ( PlayerHealth <= 0)
        {
            Lose();
        }
    }

    public void AddMoney(int amount)
    {
        CurrentMoney += amount;
        UIManager.Instance.UpdateMoney(CurrentMoney);
    }
    public bool DeductMoney(int amount )
    {
        if (CurrentMoney < amount)
        {
            UIManager.Instance.NotifyMessage("Not enough Gold!");
            return false;
        }

        CurrentMoney -= amount;
        UIManager.Instance.UpdateMoney(CurrentMoney);
        
        return true;
    }

    public void Win()
    {
        UIManager.Instance.NotifyMessageAtCenter("You Win!", 100f);
    }

    public void Lose()
    {
        UIManager.Instance.NotifyMessageAtCenter("You Lose!", 100f);
    }
}