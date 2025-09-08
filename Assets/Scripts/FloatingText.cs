using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [Header("Text Properties")]
    [SerializeField] Color textColor;
    [SerializeField] float lifetime = 0.5f;
    [SerializeField] float dropSpeed = 1f;
    [SerializeField] bool fallingText = true;

    TMP_Text tmp;

    public void SetText(string value)
    {
        tmp.text = value;
    }

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        tmp.color = textColor;
        GameManager.OnGameOver += HandleGameOver;
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnDestroy()
    {
        GameManager.OnGameOver -= HandleGameOver;
    }

    void HandleGameOver()
    {
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        float direction = fallingText ? -1f : 1f;
        transform.Translate(0f, direction * dropSpeed * Time.fixedDeltaTime, 0f);
    }
}
