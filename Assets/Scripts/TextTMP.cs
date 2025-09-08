using UnityEngine;
using TMPro;

public class TextTMP : MonoBehaviour
{
    protected TMP_Text tmp;

    public void SetText(string value) {
        tmp.text = value;
    }

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameStart += HandleGameStart;
    }

    void OnDestroy()
    {
        GameManager.OnGameOver -= HandleGameOver;
        GameManager.OnGameStart -= HandleGameStart;
    }

    void HandleGameStart()
    {
        tmp.enabled = true;
    }

    void HandleGameOver()
    { 
        tmp.enabled = false;
    }
}
