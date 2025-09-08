using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [Header("Refs")]
    [SerializeField] GameObject orcPrefab;

    [Header("Spawn Number")]
    [SerializeField] float startingSpawnsPerWave = 10f;
    [SerializeField] float spawnsPerWave = 10f;
    [SerializeField] float remainingSpawns = 0f;
    
    [Header("Spawn Rate")]
    [SerializeField] float spawnsPerSecond = 1f;
    [SerializeField] int startingWave = 1;
    [SerializeField] int wave = 1;
    [SerializeField] int waveActivation = 1;


    [Header("Level")]
    [SerializeField] int startingLevels = 0;
    [SerializeField] int levels = 0;
    [SerializeField] int levelUpMilestones = 5;

    float secondsPerSpawn;
    float timer = 0f;

    public void Spawn()
    {
        var enemy = Instantiate(orcPrefab, transform).GetComponent<Enemy>();
        enemy.name = $"{enemy.name} {spawnsPerWave - remainingSpawns + 1}";
        enemy.LevelUp(levels);
        GameManager.Instance.AddEnemy(enemy);
    }

    void ResetSpawns()
    {
        if (wave % waveActivation == 0) remainingSpawns = spawnsPerWave;
        else remainingSpawns = 0;
    }

    void Reset()
    {
        wave = startingWave;
        levels = startingLevels;
        spawnsPerWave = startingSpawnsPerWave;
        ResetSpawns();
    }

    void ProcessSpawns()
    {
        if (!GameManager.Instance.GameStart || remainingSpawns <= 0) return;

        if (timer >= secondsPerSpawn)
        {
            Spawn();
            --remainingSpawns;
            if (remainingSpawns == 0) ++spawnsPerWave;
            timer = 0f;
        }
        timer += Time.fixedDeltaTime;
    }

    void HandleGameStart()
    {
        Reset();
    }

    void HandleNextLevel()
    {
        ++wave;
        if (GameManager.Instance.Level % levelUpMilestones == 0)
        {
            ++levels;
            spawnsPerWave = startingSpawnsPerWave;
        }
        ResetSpawns();
    }

    void Awake()
    {
        secondsPerSpawn = 1 / spawnsPerSecond;

        GameManager.OnGameStart += HandleGameStart;
        GameManager.OnNextLevel += HandleNextLevel;
    }

    void OnDestroy()
    {
        GameManager.OnGameStart -= HandleGameStart;
        GameManager.OnNextLevel -= HandleNextLevel;
    }

    void FixedUpdate()
    {
        ProcessSpawns();
    }
}
