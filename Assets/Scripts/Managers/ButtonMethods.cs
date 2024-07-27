using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonMethods : CustomMonoBehaviour
{
    [SerializeField] private AudioClip onButtonPressSFX;

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
        AudioManager.Instance.PlayOneShot(onButtonPressSFX);
    }

    public void ActivateCanvas(Canvas canvas)
    {
        canvas.enabled = true;
        AudioManager.Instance.PlayOneShot(onButtonPressSFX);
    }
    
    public void DeactivateCanvas(Canvas canvas)
    {
        canvas.enabled = false;
        AudioManager.Instance.PlayOneShot(onButtonPressSFX);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
