using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject rootPanel;
    [SerializeField] private GameObject materialPanel;
    [SerializeField] private GameObject effectPanel;
    [SerializeField] private Button backButtonToRoot;

    [Header("Root Buttons")]
    [SerializeField] private Button btnMaterial;
    [SerializeField] private Button btnEffect;
    [SerializeField] private Button ESCButton;

    [Header("Lists (Assign in Inspector)")]
    [SerializeField] private Transform materialGridParent; 
    [SerializeField] private Transform effectGridParent;  
    [SerializeField] private MenuIconButton iconButtonPrefab;

    private Tower owner;
    private FollowWorldUI follow; // 위치 추적용

    [SerializeField] private List<MaterialData> materials;
    [SerializeField] private List<EffectData> effects;

    public void Init(Tower tower)
    {
        owner = tower;

        // 버튼 이벤트
        btnMaterial.onClick.AddListener(() => ShowPanel(materialPanel));
        btnEffect.onClick.AddListener(() => ShowPanel(effectPanel));
        backButtonToRoot.onClick.AddListener(() => ShowPanel(rootPanel));
        ESCButton.onClick.AddListener(() => SelectionManager.Instance.CloseMenu());

        // 아이템들 동적 생성
        BuildMaterialButtons();
        BuildEffectButtons();

        // 시작은 루트
        ShowPanel(rootPanel);
    }

    public void FollowTarget(Transform target, Vector3 offset)
    {
        follow = gameObject.AddComponent<FollowWorldUI>();
        follow.Setup(target, offset);
    }

    void ShowPanel(GameObject target)
    {
        rootPanel.SetActive(target == rootPanel);
        materialPanel.SetActive(target == materialPanel);
        effectPanel.SetActive(target == effectPanel);
        backButtonToRoot.gameObject.SetActive(target != rootPanel);
    }

    void BuildMaterialButtons()
    {
        foreach (Transform child in materialGridParent) Destroy(child.gameObject);
        foreach (var mat in materials)
        {
            var icon = Instantiate(iconButtonPrefab, materialGridParent);
            icon.Setup(mat.iconSprite , () =>
            {
                if (GameManager.Instance.DeductMoney(mat.cost) == true)
                    owner.ApplyMaterial(mat);

                SelectionManager.Instance.CloseMenu();
            }, "$"+mat.cost.ToString() );
        }
    }

    void BuildEffectButtons()
    {
        foreach (Transform child in effectGridParent) Destroy(child.gameObject);
        foreach (var ef in effects)
        {
            var icon = Instantiate(iconButtonPrefab, effectGridParent);
            icon.Setup(ef.IconSprite , () =>
            {
                if ( GameManager.Instance.DeductMoney(ef.Cost) == true )
                    owner.ApplyEffect(ef);

                SelectionManager.Instance.CloseMenu();
            }, "$"+ef.Cost.ToString());
        }
    }
}
