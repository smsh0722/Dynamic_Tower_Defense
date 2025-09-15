using UnityEngine;

public class FollowWorldUI : MonoBehaviour
{
    [SerializeField] private Vector3 worldOffset = Vector3.zero;
    private Transform target;
    private RectTransform rect;
    private Camera cam;
    private Canvas canvas;

    public void Setup(Transform target, Vector3 offset)
    {
        this.target = target;
        this.worldOffset = offset;
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null || rect == null || canvas == null) return;

        Vector3 world = target.position + worldOffset;
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(cam, world);

        RectTransform canvasRect = canvas.transform as RectTransform;
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screen,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam,
            out local
        );
        rect.anchoredPosition = local;
    }
}
