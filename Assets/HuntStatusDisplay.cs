using UnityEngine;
using TMPro;

public class HuntStatusDisplayWaad : MonoBehaviour
{
    public TextMeshProUGUI statusText;
    public int targetCount = 5; // �� ��� ��������� ���� ������� ������
    public float huntDuration = 120f; // ����� ����� ��������

    private int huntedCount = 0;
    private float timer;

    void Start()
    {
        timer = huntDuration;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0);

        UpdateStatusText();
    }

    public void AddHunt() // ���� ��� ������ �� ��� ���� ���� �����
    {
        huntedCount++;
    }

    void UpdateStatusText()
    {
        string timeFormatted = string.Format("{0:00}:{1:00}", Mathf.Floor(timer / 60), timer % 60);
        statusText.text = $"?? Hunted: {huntedCount}/{targetCount}\n? Time Left: {timeFormatted}";
    }
}
