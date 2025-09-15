using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }
    [SerializeField] private Canvas mainCanvas;                  // Screen Space - Overlay
    [SerializeField] private TowerMenuController menuPrefab;
    private TowerMenuController mCurrentMenu;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowMenuForTower(Tower tower)
    {
        CloseMenu();

        // ����
        mCurrentMenu = Instantiate(menuPrefab, mainCanvas.transform);
        mCurrentMenu.Init(tower);

        // ����潺ũ�� ��ǥ�� anchoredPosition����
        mCurrentMenu.FollowTarget(tower.transform, offset: new Vector3(0, 1.2f, 0));
    }

    public void CloseMenu()
    {
        if (mCurrentMenu != null)
        {
            Destroy(mCurrentMenu.gameObject);
            mCurrentMenu = null;
        }
    }
}
