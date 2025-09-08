using UnityEngine;
using System;
using System.Collections.Generic;

public enum GameState
{
    StartingScreen,
    GameStart,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnHighScoreChanged;
    public static event Action<int> OnHealthChanged;
    public static event Action<int> OnGoldChanged;
    public static event Action OnNextLevel;
    public static event Action OnGameStart;
    public static event Action OnGameOver;
    public static event Action OnScreenStart;

    [field: Header("Refs")]
    [field: SerializeField] public Tower tower1Prefab { get; private set; }
    [field: SerializeField] public GameObject towerContainer { get; private set; }

    [field: Header("Game States")]
    [field: SerializeField] public bool StartingScreen { get; private set; }
    [field: SerializeField] public bool GameStart { get; private set; }
    [field: SerializeField] public bool GameOver { get; private set; }
    [field: SerializeField] public bool ChangeLevel { get; private set; } = false;
    [field: SerializeField] public bool CanBuyTower { get; private set; } = true;


    [field: Header("Shared Data")]
    [field: SerializeField] public int Score { get; private set; } = 0;
    [field: SerializeField] public int HighScore { get; private set; } = 0;
    [field: SerializeField] public int Health { get; private set; } = 10;
    [field: SerializeField] public int Gold { get; private set; } = 20;
    [field: SerializeField] public int Level { get; private set; } = 1;

    [field: SerializeField] public List<Enemy> Enemies { get; private set; } = new List<Enemy>();

    public void IncrementScore()
    {
        ++Score;
        OnScoreChanged?.Invoke(Score);
    }

    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }

    public void DecreaseHealth(int damage)
    {
        Health -= damage;
        OnHealthChanged?.Invoke(Health);
        if (Health <= 0) EndGame();
    }

    void ResetHealth()
    {
        Health = 10;
        OnHealthChanged?.Invoke(Health);
    }

    public void IncreaseGold(int dropped)
    {
        Gold += dropped;
        if (Gold >= tower1Prefab.GoldCost) CanBuyTower = true;
        OnGoldChanged?.Invoke(Gold);
    }

    public void DecreaesGold(int cost)
    {
        Gold -= cost;
        if (Gold < tower1Prefab.GoldCost) CanBuyTower = false;
        OnGoldChanged?.Invoke(Gold);
    }

    void ResetGold()
    {
        Gold = 20;
        OnGoldChanged?.Invoke(Gold);
    }

    void IncrementLevel()
    {
        ++Level;
        OnNextLevel?.Invoke();
    }

    void ResetLevel()
    {
        Level = 1;
        OnNextLevel?.Invoke();
    }

    void UpdateHighScore()
    {
        HighScore = Score;
        OnHighScoreChanged?.Invoke(HighScore);
    }

    public void AddEnemy(Enemy enemy) => Enemies.Add(enemy);

    public void RemoveEnemy(Enemy enemy)
    {
        if (Enemies.Contains(enemy)) Enemies.Remove(enemy);
        if (Enemies.Count == 0) IncrementLevel();
    }

    public bool BuyTower(Vector3 position)
    {
        if (GameStart && Gold >= tower1Prefab.GoldCost)
        {
            Instantiate(tower1Prefab.gameObject, new Vector3(position.x, position.y, tower1Prefab.gameObject.transform.position.z), Quaternion.identity, towerContainer.transform);
            DecreaesGold(tower1Prefab.GoldCost);
            return true;
        }
        return false;
    }

    public void StartGame()
    {
        StartingScreen = false;
        GameStart = true;
        GameOver = false;
        ChangeLevel = false;
        ResetScore();
        ResetHealth();
        ResetGold();
        ResetLevel();
        OnGameStart?.Invoke();
        OnScoreChanged?.Invoke(Score);
        OnHighScoreChanged?.Invoke(HighScore);
        OnHealthChanged?.Invoke(Health);
        OnGoldChanged?.Invoke(Gold);
    }

    public void EndGame()
    {
        StartingScreen = false;
        GameStart = false;
        GameOver = true;
        ChangeLevel = false;

        if (Score > HighScore)
        {
            UpdateHighScore();
        }
        OnGameOver?.Invoke();
    }

    public void StartScreen()
    {
        StartingScreen = true;
        GameStart = false;
        GameOver = false;
        ChangeLevel = false;
        OnScreenStart?.Invoke();
    }

    public void NextLevel()
    {
        ChangeLevel = true;
        OnNextLevel?.Invoke();
        ChangeLevel = false;
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Instance.StartGame();   
    }
}
