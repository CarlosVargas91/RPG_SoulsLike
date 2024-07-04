using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_MainMenu : MonoBehaviour
{
    [SerializeField] private string sceneName = "MainScene";
    [SerializeField] private GameObject continueButton;
    [SerializeField] UI_FadeScript fadeScreen;
    
    private void Start()
    {
        if (SaveManager.instance.hasSaveData() == false)
            continueButton.SetActive(false);
    }
    public void continueGame()
    {
        StartCoroutine(loadSceneWithFadeEffect(1.5f));
    }

    public void newGame()
    {
        SaveManager.instance.deleteSavedData();
        StartCoroutine(loadSceneWithFadeEffect(1.5f));
    }

    //public void exitGame() => Application.Quit();

    IEnumerator loadSceneWithFadeEffect(float _delay)
    {
        fadeScreen.fadeOut();

        yield return new WaitForSeconds(_delay);

        SceneManager.LoadScene(sceneName);
    }
}
