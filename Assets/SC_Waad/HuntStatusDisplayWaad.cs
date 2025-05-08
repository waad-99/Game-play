using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int normalKills = 0;
    public int specialKills = 0;

    public int normalTarget = 10;
    public int specialTarget = 3;

    public float gameDuration = 120f;
    private float remainingTime;

    public TextMeshProUGUI normalKillText;
    public TextMeshProUGUI specialKillText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI winText;

    public GameObject[] enemyPrefabsToRemove; // ← نحددهم من الـ Inspector

    private bool gameEnded = false;

    void Start()
    {
        remainingTime = gameDuration;
        UpdateUI();

        if (winText != null)
            winText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (gameEnded)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            GameOver();
        }

        UpdateTimerUI();
    }

    public void AddNormalKill()
    {
        if (gameEnded) return;

        normalKills++;
        UpdateUI();
        CheckWin();
    }

    public void AddSpecialKill()
    {
        if (gameEnded) return;

        specialKills++;
        UpdateUI();
        CheckWin();
    }

    void UpdateUI()
    {
        if (normalKillText != null)
            normalKillText.text = $"{normalKills}/{normalTarget}";

        if (specialKillText != null)
            specialKillText.text = $"{specialKills}/{specialTarget}";
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    void CheckWin()
    {
        if (normalKills >= normalTarget && specialKills >= specialTarget)
        {
            GameWin();
        }
    }

    void GameWin()
    {
        gameEnded = true;

        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "Win";
        }

        Debug.Log("Game Won!");
        RemoveEnemiesByPrefab();
    }

    void GameOver()
    {
        gameEnded = true;

        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "Lose";
        }

        Debug.Log("Game Over!");
        RemoveEnemiesByPrefab();
    }

    void RemoveEnemiesByPrefab()
    {
        if (enemyPrefabsToRemove == null || enemyPrefabsToRemove.Length == 0)
            return;

        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            foreach (GameObject enemyPrefab in enemyPrefabsToRemove)
            {
                if (obj.name.Contains(enemyPrefab.name))
                {
                    obj.SetActive(false); // أو استخدمي Destroy(obj) إذا تبين حذفهم
                }
            }
        }
    }

    public void RestartGame()
    {
        normalKills = 0;
        specialKills = 0;
        remainingTime = gameDuration;
        gameEnded = false;
        UpdateUI();
        UpdateTimerUI();

        if (winText != null)
            winText.gameObject.SetActive(false);

        Debug.Log("Game Restarted!");
    }

    public void OnButtonPressed()
    {
        Debug.Log("Button Pressed!");
    }
}
