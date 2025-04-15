using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    public static ScoreView Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI scoreText;

    private int score = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}