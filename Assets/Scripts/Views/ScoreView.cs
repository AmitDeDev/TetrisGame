using TMPro;
using UnityEngine;
using DG.Tweening;

public class ScoreView : MonoBehaviour
{
    public static ScoreView Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI scoreText;

    private int score = 0;
    public int CurrentScore => score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
            scoreText.color = Color.green;
            scoreText.transform.DOScaleY(0.6f, 0.8f).SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    scoreText.color = Color.yellow;
                    scoreText.transform.DOScaleY(1f, 0.8f).SetEase(Ease.OutBounce)
                        .OnComplete(
                            () =>
                            {
                                scoreText.color = Color.white;
                                scoreText.text = $"Score: {score}";
                            });
                    
                });
        }
    }
}