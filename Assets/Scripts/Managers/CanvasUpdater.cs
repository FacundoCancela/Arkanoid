using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class CanvasUpdater : CustomMonoBehaviour
{
    [SerializeField] private GameObject pauseGameCanvas;
    [SerializeField] private GameObject gameEndedCanvas;
    [SerializeField] private Image wonTextImage;
    [SerializeField] private Image lostTextImage;

    [SerializeField] private GameObject[] heartImages;

    [SerializeField] private Image[] scoreImages;
    [SerializeField] private Sprite[] spriteNumbers;

    [SerializeField] private AudioClip onPauseGameSFX;
    [SerializeField] private AudioClip onLoseGameSFX;
    [SerializeField] private AudioClip onWinGameSFX;

    public override void CustomOnEnable()
    {
        GameManager.Instance.OnGameEndedDelegate += OnGameEnded;
        GameManager.Instance.ActivePaddleController.OnPauseInputPressedDelegate += PauseGame;
        GameManager.Instance.ActivePaddleController.PaddleModel.OnLifeChangedDelegate += UpdateHealthPointsUI;
        GameManager.Instance.OnScoreChangedDelegate += UpdateScoreUI;
    }

    public override void CustomOnDisable()
    {
        GameManager.Instance.OnGameEndedDelegate -= OnGameEnded;
        GameManager.Instance.ActivePaddleController.OnPauseInputPressedDelegate -= PauseGame;
        GameManager.Instance.ActivePaddleController.PaddleModel.OnLifeChangedDelegate -= UpdateHealthPointsUI;
        GameManager.Instance.OnScoreChangedDelegate -= UpdateScoreUI;
    }

    private void UpdateHealthPointsUI(int lifePoints)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < lifePoints)
            {
                heartImages[i].SetActive(true);
            }
            else
                heartImages[i].SetActive(false);
        }
    }
    private void UpdateScoreUI(int newScore)
    {
        string scoreStr = newScore.ToString();
        int digitValue;

        for (int i = 0; i < scoreImages.Length; i++)
        {
            if (i < scoreStr.Length)
            {
                digitValue = (int)char.GetNumericValue(scoreStr[i]);
                scoreImages[i].sprite = spriteNumbers[digitValue];
                scoreImages[i].gameObject.SetActive(true);
            }
            else
                scoreImages[i].gameObject.SetActive(false);
        }
    }
    
    private void PauseGame()
    {
        if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
        pauseGameCanvas.SetActive(!pauseGameCanvas.activeSelf);
        AudioManager.Instance.PlayOneShot(onPauseGameSFX);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseGameCanvas.SetActive(false);
        AudioManager.Instance.PlayOneShot(onPauseGameSFX);
    }

    private void OnGameEnded(bool won)
    {
        if (!gameEndedCanvas.activeSelf)
            gameEndedCanvas.SetActive(true);

        wonTextImage.enabled = won;
        lostTextImage.enabled = !won;

        if (won)
            AudioManager.Instance.PlayOneShot(onWinGameSFX);
        else
            AudioManager.Instance.PlayOneShot(onLoseGameSFX);

        Time.timeScale = 0;
    }
}