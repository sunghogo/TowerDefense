using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private RectTransform maskRectTransform;
    [SerializeField] private RectTransform imageRectTransform;

    [SerializeField, Range(0f, 1f)] private float fill = 1f;

    void HandleGameOver()
    {
        imageRectTransform.gameObject.GetComponent<Image>().enabled = false;
    }

    void HandleGameSart()
    {
        imageRectTransform.gameObject.GetComponent<Image>().enabled = true;
    }

    void Awake()
    {
        if (imageRectTransform != null)
        {
            imageRectTransform.anchorMin = new Vector2(0f, 0.5f);
            imageRectTransform.anchorMax = new Vector2(0f, 0.5f);
            imageRectTransform.pivot = new Vector2(0f, 0.5f);
            imageRectTransform.anchoredPosition = new Vector2(0f, imageRectTransform.anchoredPosition.y);
        }

        UpdateBar();
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameStart += HandleGameSart;
    }

    void OnDestroy()
    {
        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnGameStart -= HandleGameSart;
    } 

    private void UpdateBar()
    {
        if (imageRectTransform == null || maskRectTransform == null) return;
        float fullWidth = maskRectTransform.rect.width;
        float targetWidth = Mathf.Max(0f, fullWidth * fill);

        imageRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, targetWidth);
    }
    
    public void SetValue(float current, float max)
    {
        if (max <= 0f) max = 1f;
        fill = Mathf.Clamp01(current / max);
        UpdateBar();
    }
    
}
