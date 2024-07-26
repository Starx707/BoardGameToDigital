using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] bool InGame;
    [SerializeField] private GameObject _warningPanel;
    [SerializeField] private GameManager _gm;

    //>> ------ Main menu ------ <<
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("CardGame");
        _gm.GetComponent<GameManager>().tutorialActive = false;
    }

    public void PlayTutorialGame()
    {
        _gm.GetComponent<GameManager>().tutorialActive = true;
        _gm.GetComponent<GameManager>().TutorialSelected();
        SceneManager.LoadSceneAsync("CardGame");
    }

    //>> ------ Card game ------ <<
    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
        _gm.GetComponent<GameManager>().tutorialActive = false;
        Time.timeScale = 1;
    }

    public void OpenBestiary()
    {
        //open book sound
    }

    //>> ------ Generally ------ <<
    public void QuitGame()
    {
        if (InGame == true)
        {
            QuitWarning();
        }
        else
        {
            Application.Quit();
        }
    }

    public void SFXBtn() //Only keep if want same sound for all btns
    {

    }


    //>> ------ Card game ------ <<
    public void PlayAgain()
    {
        SceneManager.LoadSceneAsync("CardGame");
    }

    private void QuitWarning()
    {
        _warningPanel.SetActive(true);
    }

    public void AgreeToWarning()
    {
        Application.Quit();
    }
}
