using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CapsuleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public int enemyCount;
    }

    [Header("🌀 Spawning Settings")]
    public GameObject[] normalCapsulePrefabs;
    public GameObject[] specialCapsulePrefabs;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 3f;
    public Wave[] waves;

    [Header("🎮 UI Settings")]
    public TextMeshProUGUI buttonText; // نص الزر
    public string startGameText = "Start Game";
    public string restartGameText = "Restart Game";

    private int currentWaveIndex = 0;
    private int enemiesAlive = 0;
    private bool waveInProgress = false;
    private bool gameStarted = false;
    private bool gameEnded = false;

    private GameManagerWaad2 gameManager;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        gameManager = FindObjectOfType<GameManagerWaad2>();

        // تعيين النص أول مرة
        if (buttonText != null)
        {
            buttonText.text = startGameText;
        }
    }

    public void ToggleGame()
    {
        if (!gameStarted && !gameEnded)
        {
            StartGame();
        }
        else
        {
            RestartSpawner(); // ⬅️ هذا هو التغيير الوحيد
        }
    }

    void StartGame()
    {
        Debug.Log("Game Started!");
        gameStarted = true;
        gameEnded = false;
        currentWaveIndex = 0;

        if (buttonText != null)
        {
            buttonText.text = restartGameText;
        }

        StartWave();
    }

    void RestartGame()
    {
        Debug.Log("Game Restarted!");

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();

        gameStarted = false;
        gameEnded = false;
        currentWaveIndex = 0;
        enemiesAlive = 0;
        waveInProgress = false;

        StartGame();
    }

    void StartWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("All waves finished!");
            gameEnded = true;

            if (buttonText != null)
            {
                buttonText.text = restartGameText;
            }

            return;
        }

        Wave wave = waves[currentWaveIndex];
        enemiesAlive = wave.enemyCount;
        waveInProgress = true;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject prefabToSpawn;
            bool isSpecial = Random.value < 0.2f;

            if (isSpecial && specialCapsulePrefabs.Length > 0)
                prefabToSpawn = specialCapsulePrefabs[Random.Range(0, specialCapsulePrefabs.Length)];
            else
                prefabToSpawn = normalCapsulePrefabs[Random.Range(0, normalCapsulePrefabs.Length)];

            GameObject enemy = Instantiate(prefabToSpawn, spawn.position, Quaternion.identity);
            activeEnemies.Add(enemy);

            CapsuleHealth health = enemy.GetComponent<CapsuleHealth>();
            if (health != null)
            {
                health.spawner = this;
                health.gameManager = gameManager;
                health.isSpecial = isSpecial;
            }
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0 && waveInProgress)
        {
            waveInProgress = false;
            currentWaveIndex++;
            Invoke(nameof(StartWave), timeBetweenWaves);
        }
    }

    public void OnGameOver()
    {
        gameEnded = true;

        if (buttonText != null)
        {
            buttonText.text = restartGameText;
        }
    }

    public void RestartSpawner()
    {
        Debug.Log("تم إعادة تشغيل السبونر!");

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();

        gameStarted = false;
        gameEnded = false;
        currentWaveIndex = 0;
        enemiesAlive = 0;
        waveInProgress = false;

        if (buttonText != null)
        {
            buttonText.text = startGameText;
        }

        // استدعاء دالة RestartGame من GameManager لتصفير القيم
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }

        // ما نبدأ اللعبة على طول عشان يبين كأنها توها مفتوحة
    }
}
