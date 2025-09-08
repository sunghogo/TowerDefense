using UnityEngine;
using TMPro;

public class UpdatedTMP : MonoBehaviour
{
    TMP_Text tmp;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (gameObject.CompareTag("Score"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.Score.ToString();
            GameManager.OnScoreChanged += UpdateText;
        }
        else if (gameObject.CompareTag("High Score"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.HighScore.ToString();
            GameManager.OnHighScoreChanged += UpdateText;
        }
        else if (gameObject.CompareTag("HP"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.Health.ToString();
            GameManager.OnHealthChanged += UpdateText;
        }
        else if (gameObject.CompareTag("GOLD"))
        {
            if (GameManager.Instance) tmp.text = GameManager.Instance.Gold.ToString();
            GameManager.OnGoldChanged += UpdateText;
        }

        GameManager.OnGameOver += HandleGameOver;
        GameManager.OnGameStart += HandleGameStart;
    }

    void OnDestroy()
    {
        if (gameObject.CompareTag("Score"))  GameManager.OnScoreChanged -= UpdateText;
        else if (gameObject.CompareTag("High Score")) GameManager.OnHighScoreChanged -= UpdateText;
        else if (gameObject.CompareTag("HP"))  GameManager.OnHealthChanged -= UpdateText;
        else if (gameObject.CompareTag("GOLD")) GameManager.OnGoldChanged -= UpdateText;
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

    void UpdateText(int number)
    {
        tmp.text = $"{gameObject.tag}: {number}";
    }
}
