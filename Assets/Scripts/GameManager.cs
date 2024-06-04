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
    public List<Card> availableHandSlots = new List<Card>();
    public List<Card> availablePlaySlots = new List<Card>();
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

                if (availableHandSlots.Count < 5)
                {
                    randomCard.gameObject.SetActive(true);
                    randomCard.transform.position = handDeckSlots[availableHandSlots.Count].position;
                    availableHandSlots.Add(randomCard);
                    deck.Remove(randomCard);
                    return;
                }

                /*
                for (int i = 0; i < availableHandSlots.Count; i++) //check for available spot
                {
                    if (availableHandSlots[i] == true)
                    {
                        //spot open then set active
                        randomCard.gameObject.SetActive(true);
                        randomCard.cardIndex = i; //give index to the randomly placed card

                        //& put the card in the slot available and put it as unavailable
                        randomCard.transform.position = handDeckSlots[i].position;
                        availableHandSlots[i] = randomCard;
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
                */
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
        if (availablePlaySlots.Count < 4)
        {
            card.GetComponent<Card>().StartMove(playSlots[availablePlaySlots.Count].position);
            availableHandSlots.Remove(card.GetComponent<Card>());
            availablePlaySlots.Add(card.GetComponent<Card>());
            Debug.Log("Played");
        }

        for (int i = 0; i < availableHandSlots.Count; i++)
        {
            availableHandSlots[i].GetComponent<Card>().StartMove(handDeckSlots[i].position);
        }

        /*
        for (int i = 0; i < availablePlaySlots.Count; i++)
        {
            if (availablePlaySlots[i] == true)
            {
                card.transform.position = playSlots[i].position;
                availablePlaySlots.Remove(card.GetComponent<Card>());
                Debug.Log("Played");
                return;
            }
        }*/
    }


    //Card back to hand
    public void ReturnCardToHand(GameObject card)
    {
        Debug.Log("Back in hand");
        if (availableHandSlots.Count < 5)
        {
            card.GetComponent<Card>().StartMove(handDeckSlots[availableHandSlots.Count].position);

            availablePlaySlots.Remove(card.GetComponent<Card>());
            availableHandSlots.Add(card.GetComponent<Card>());
        }

        /*for (int i = 0; i < availableHandSlots.Count; i++) 
        {
            if (availableHandSlots[i] == true)
            {
                //& put the card in the slot available and put it as unavailable
                var ind = System.Array.IndexOf(availablePlaySlots, card);
                Debug.Log(ind);
                card.transform.position = handDeckSlots[i].position;
                availablePlaySlots[ind] = true;
                availableHandSlots[i] = false;
                return;
            }
        }*/
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