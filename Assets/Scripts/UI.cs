using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] bool InGame;
    [SerializeField] private GameObject _warningPanel;
    [SerializeField] GameObject _audioM;

    [Header("---- Audio ---")]
    [SerializeField] private AudioClip _btnPressed;
    [SerializeField] private AudioClip _pageTurn;
    [SerializeField] private AudioClip _pauseSFX;

    //>> ------ Main menu ------ <<
    public void PlayGame()
    {
        StateMachine.gameState = false;
        SceneManager.LoadSceneAsync("CardGame");
    }

    public void PlayTutorialGame()
    {
        StateMachine.gameState = true;
        SceneManager.LoadSceneAsync("CardGame");
    }

    //>> ------ Card game ------ <<
    public void ReturnToMainMenu()
    {
        StateMachine.gameState = false;
        SceneManager.LoadSceneAsync("MainMenu");
        Time.timeScale = 1;
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

    //>> ------ SFXx ------ <<
    public void ButtonSoundEffect() //Only keep if want same sound for all btns
    {
        _audioM.GetComponent<AudioManager>().PlaySFX(_btnPressed);
    }

    public void PageTurnSoundEffect()
    {
        _audioM.GetComponent<AudioManager>().PlaySFX(_pageTurn);
    }

    public void PausedSoundEffect()
    {
        _audioM.GetComponent<AudioManager>().PlaySFX(_pauseSFX);
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
