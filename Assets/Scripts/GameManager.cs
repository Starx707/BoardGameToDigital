using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    //Card game data
    public List<Card> deck = new List<Card>();
    public Transform[] handDeckSlots;
    public Transform[] playSlots;
    public List<Card> availableHandSlots = new List<Card>();
    public List<Card> availablePlaySlots = new List<Card>();
    bool newTurn = true;

    //Enemy variables
    public Transform[] enemyHandSlotsPos;
    public Transform[] enemyPlaySlotsPos;
    public List<GameObject> enemyCards = new List<GameObject>();
    public List<Card> enemyHandSlots = new List<Card>();
    public List<Card> enemyPlaySlots = new List<Card>();
    [SerializeField] private Sprite cardBack;
    private Sprite _cardFront;

    [SerializeField] private float showTime = 5;

    //Player data


    //Roaming map data


    //UI data
    public TMP_Text deckAmount;

    //Timer
    [SerializeField] TextMeshProUGUI TimerText;
    [SerializeField] float remainingTime;

    private void Start()
    {
        StartCoroutine(EnemyTurnStart());
    }

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
    }

    //-->Return to enemy hand


    //-->Open bestiary


    //-->Bell rang
    // flip enemy cards
    // Battle
    private void RingBell()
    {

    }


    //----- "Enemy"
    IEnumerator EnemyTurnStart()
    {
        ShowCards();
        yield return new WaitForSeconds(showTime);
        ChangeCardSprite(cardBack);
        yield return new WaitForSeconds(0.5f);
        foreach (Card c in enemyHandSlots)
        {
            c.StartMove(enemyPlaySlotsPos[0].position);
        }

        ShuffleHand(enemyHandSlots);
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < enemyHandSlots.Count; i++)
        {
            enemyHandSlots[i].StartMove(enemyPlaySlotsPos[i].position);
        }
        


    }

    //Card show
    private void ShowCards()
    {
        for (int i = 0; i < enemyHandSlotsPos.Length; i++)
        {
            var c = Instantiate(enemyCards[Random.Range(0, enemyCards.Count)]);
            c.transform.position = enemyHandSlotsPos[i].position;
            enemyHandSlots.Add(c.GetComponent<Card>());
        }
    }

    // Randomly shuffle the enemy hand and play them
    public List<Card> ShuffleHand(List<Card> hand)
    {
        // Fisher-Yates shuffle argorithm
        for (int i = hand.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = hand[i];
            hand[i] = hand[j];
            hand[j] = temp;
        }
        return hand;
    }


    private void ChangeCardSprite(Sprite s)
    {

        for (int i = 0; i < enemyHandSlots.Count; i++)
        {
            var sr = enemyHandSlots[i].gameObject.GetComponent<SpriteRenderer>();
            //Get the card through [i]. .gameObject to reference to it .GetComponent is to get whatever is in inspector

            sr.sprite = s;
        }
    }


    //---- Battle System

    // Single Battle
    private Tuple<bool, bool> SingleBattle(Card A, Card B)
    {
        bool aliveA = A.hp - B.dmg > 0;
        bool aliveB = B.hp - A.dmg > 0;

        return new Tuple<bool, bool>(aliveA, aliveB);
    }

    // Battle
    private void Battles(){
        List<Tuple<bool, bool>> battles = new List<Tuple<bool, bool>>();
        for (int i = 0; i < 4; i++)
        {
            battles.Add(SingleBattle(availablePlaySlots[i], enemyPlaySlots[i]));
        }
        for (int i = 0; i < battles.Count; i++)
        {
            if (battles[i].Item1 == false)
            {
                Destroy(availablePlaySlots[i].gameObject);
            }
            else
            {
                ReturnCardToHand(availablePlaySlots[i].gameObject);
            }

            if (battles[i].Item2 == false)
            {
                Destroy(enemyPlaySlots[i]);
            }
            else
            {
                //Return to enemy hand (make the function smart ass :3
            }
        }
    }

    //---- General

    //Turn over
    //set newTurn to true

    //Game won


    //Game lost



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
    }
}