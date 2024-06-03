using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    //Card game data
    public List<Card> deck = new List<Card>();
    public Transform[] handDeckSlots;
    public Transform[] playSlots;
    public bool[] availableHandSlots;
    public bool[] availablePlaySlots;
    bool newTurn = true;

    //Player data


    //Roaming map data


    //UI data
    public TMP_Text deckAmount;

    //Timer
    [SerializeField] TextMeshProUGUI TimerText;
    [SerializeField] float remainingTime;

    //>> ------ Card game ------ <<
    //Drawing a card
    public void DrawCard()
    {
        if (newTurn == true)
        {
            if (deck.Count >= 1)
            {
                Card randomCard = deck[Random.Range(0, deck.Count)]; //get random card

                for (int i = 0; i < availableHandSlots.Length; i++) //check for available spot
                {
                    if (availableHandSlots[i] == true)
                    {
                        //spot open then set active
                        randomCard.gameObject.SetActive(true);
                        randomCard.cardIndex = i; //give index to the randomly placed card

                        //& put the card in the slot available and put it as unavailable
                        randomCard.transform.position = handDeckSlots[i].position;
                        availableHandSlots[i] = false;
                        deck.Remove(randomCard);
                        return;
                    }
                    else 
                    {
                        //sfx
                        //Debug.Log("Deck full");
                        //newTurn = false;//Should prevent player from getting more cards while having max amount reached
                    }
                }
            }
        }
        else if (newTurn == false)
        {
            //Tell player can't grab more cards until next turn
            Debug.Log("Max cards for turn reached");
        }
    }

    //Play a card
    public void PlayCard(GameObject card)
    {
        for (int i = 0; i < availablePlaySlots.Length; i++)
        {
            if (availablePlaySlots[i] == true)
            {
                card.transform.position = playSlots[i].position;
                availablePlaySlots[i] = false;
                Debug.Log("Played");
                return;
            }
        }
    }


    //Card back to hand
    public void ReturnCardToHand(GameObject card)
    {
        Debug.Log("Back in hand");
        for (int i = 0; i < availableHandSlots.Length; i++) 
        {
            if (availableHandSlots[i] == true)
            {
                //& put the card in the slot available and put it as unavailable
                card.transform.position = handDeckSlots[i].position;
                availablePlaySlots[i] = true;
                availableHandSlots[i] = false;
                return;
            }
        }
    }

    //Open bestiary


    //Bell rang



    //----- "Enemy"

    //Card show

    //Cards play


    //---- General

    //Turn over
    //set newTurn to true

    //Game won


    //Game lost


    //Card defeated


    //>> ------ Roaming area ------ <<

    //Increase deck
    /*Can't play without 10 cards in deck*/


    //>> ------ UI ------ <<


    //Update every frame
    private void Update()
    {

        deckAmount.text = deck.Count.ToString(); //cards in deck to UI

        //Timer
        remainingTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        TimerText.text = string.Format("{00:00}:{01:00}", minutes, seconds);
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
        }
        else if (remainingTime < 0)
        {
            remainingTime = 0;
            TimerText.text = "00:00";
            Debug.Log("Play time is over!");
        }
        //end timer
    }
}
