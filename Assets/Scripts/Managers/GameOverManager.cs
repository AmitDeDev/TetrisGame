using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("Text & button Positions")]
    public RectTransform bgImageRect;
    public RectTransform gameOverRect;
    public RectTransform finalScoreRect;
    public Button replayButtonRect;

    [Header("Movement settings")] 
    public Vector2 bgImageTargetPos;
    public Vector2 gameOverTargetPos;
    public Vector2 scoreTargetPos;
    public float moveDuration = 1f;
    public float delayBetween = 1f;

    public bool IsGameOver { get; private set; } = false;

    void Awake()
    {
        Instance = this;
        
        bgImageRect.gameObject.SetActive(false);
        gameOverRect.gameObject.SetActive(false);
        finalScoreRect.gameObject.SetActive(false);
        replayButtonRect.gameObject.SetActive(false);
    }

    public void TriggerGameOver(int finalScore)
    {
        if (IsGameOver) return;
        IsGameOver = true;
        
        bgImageRect.anchoredPosition = new Vector2(0, -Screen.height);
        bgImageRect.gameObject.SetActive(true);
        bgImageRect.DOAnchorPos(bgImageTargetPos, moveDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // show & animate Game Over
                gameOverRect.anchoredPosition = new Vector2(0, -Screen.height);
                gameOverRect.gameObject.SetActive(true);
                gameOverRect.DOAnchorPos(gameOverTargetPos, moveDuration)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        // show & animate score
                        DOVirtual.DelayedCall(delayBetween, () =>
                        {
                            var scoreText = finalScoreRect.GetComponent<TextMeshProUGUI>();
                            if (scoreText != null) scoreText.text = $"Score: {finalScore}";
                            finalScoreRect.anchoredPosition = new Vector2(0, -Screen.height);
                            finalScoreRect.gameObject.SetActive(true);
                            finalScoreRect.DOAnchorPos(scoreTargetPos, moveDuration)
                                .SetEase(Ease.OutBack)
                                .OnComplete(() =>
                                {
                                    replayButtonRect.gameObject.SetActive(true);
                                });
                        });
                    });
            });
    }
    
    public void OnReplayPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
