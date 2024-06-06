using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    [SerializeField] bool InGame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //>> ------ Main menu ------ <<
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("CardGame");
    }

    //>> ------ Card game ------ <<
    public void ReturnToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }



    //---- SFX
    public void PauzeGameSFX(){} //Can delete if not all same sfx

    public void ContinueGameSFX(){ } //Can delete if not all same sfx



    //>> ------ Generally ------ <<
    public void QuitGame()
    {
        if (InGame == true)
        {
            //Send warning first
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
        //Add warning
    }

    public void AgreeToWarning()
    {
        Application.Quit();
    }
}
