using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    bool hasBeenPlayed;
    public int cardIndex;
    private GameManager Gm;

    private void Start()
    {
        Gm = FindAnyObjectByType<GameManager>();
    }

    private void OnMouseDown()
    {
        if(hasBeenPlayed == false)
        {

            hasBeenPlayed = true;
        }
        else if (hasBeenPlayed == true)
        {

            hasBeenPlayed = false;
        }
    }
}
