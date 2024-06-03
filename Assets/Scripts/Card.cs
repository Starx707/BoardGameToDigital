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
            //Move to play area
            Debug.Log("Played");
            Gm.PlayCard(this.gameObject);
            hasBeenPlayed = true;
            Gm.availableHandSlots[cardIndex] = true;
        }
        else if (hasBeenPlayed == true)
        {
            //Move back to hand
            Gm.ReturnCardToHand(this.gameObject);
            hasBeenPlayed = false;
            Gm.availablePlaySlots[cardIndex] = true;
        }
    }
}
