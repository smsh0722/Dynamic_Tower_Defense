using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuIconButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI costText;

    public void Awake()
    {
        if (costText != null )
            costText.gameObject.SetActive( false );
    }

    public void Setup(Sprite sprite, Action onClick, string text = "None")
    {
        icon.sprite = sprite;
        if (costText != null)
        {
            if (text.Equals("None"))
                costText.gameObject.SetActive(false);
            else
            {
                costText.gameObject.SetActive(true);
                costText.text = text;
            }
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick?.Invoke());
    }
}
