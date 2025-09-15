using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    private Transform mTarget;

    private Camera mMainCamera;

    private Vector3 mPositionOffset = new Vector3(0, 0.8f, 0);

    // 내 RectTransform
    private RectTransform mSelfRect;
    // 부모 Canvas의 RectTransform
    private RectTransform mCanvasRect;       
    private Canvas mParentCanvas;

    void Awake()
    {
        mSelfRect = GetComponent<RectTransform>();
        mParentCanvas = GetComponentInParent<Canvas>();
        if (mParentCanvas != null) mCanvasRect = mParentCanvas.GetComponent<RectTransform>();
        mMainCamera = Camera.main;
    }

    void Start()
    {
        mMainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 world = mTarget.position + mPositionOffset;

        // 월드 → 화면 좌표
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(mMainCamera, world);

        // 화면 → 캔버스 로컬(anchored) 좌표
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mCanvasRect, screen, mParentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mMainCamera,
            out localPoint
        );

        mSelfRect.anchoredPosition = localPoint;
    }

    public void Initialize(Transform targetTransform)
    {
        mTarget = targetTransform;
    }

    public void UpdateHP(float currentHP, float maxHP)
    {
        hpSlider.value = currentHP / maxHP;
    }
}