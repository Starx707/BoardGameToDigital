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

    private bool _GameOver = false;
    private int _defeatedCards = 0;
    [SerializeField] private int GoalDefeatedCards;

    //Enemy variables
    public Transform[] enemyHandSlotsPos;
    public Transform[] enemyPlaySlotsPos;
    public List<GameObject> enemyCards = new List<GameObject>();
    public List<Card> enemyHandSlots = new List<Card>();
    public List<Card> enemyPlaySlots = new List<Card>();
    [SerializeField] private Sprite cardBack;

    [SerializeField] private float showTime = 5;

    //Player data


    //Roaming map data


    //UI data
    public TMP_Text deckAmount;
    [SerializeField] Button Bell;
    private bool _isBattling = false;
    public TMP_Text cardsBeatenTxt;
    [SerializeField] private TMP_Text _result;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _playedByEnemy;

    //Timer
    [SerializeField] TextMeshProUGUI TimerText;
    [SerializeField] float remainingTime;

    private void Start()
    {
        StartCoroutine(EnemyTurnStart());
        Bell.interactable = false;
    }

    //>> ------ Card game ------ <<
    //Drawing a card
    public void DrawCard()
    {
            if (deck.Count >= 1)
            {
                Card randomCard = deck[Random.Range(0, deck.Count)]; //get random card

                if (availablePlaySlots.Count + availableHandSlots.Count < 5)
                {
                    if (availableHandSlots.Count < 5)
                    {
                        randomCard.gameObject.SetActive(true);
                        randomCard.transform.position = handDeckSlots[availableHandSlots.Count].position;
                        availableHandSlots.Add(randomCard);
                        deck.Remove(randomCard);
                        return;
                    }
                }
                else
                {
                    //Notify player, max cards reached
                    Debug.Log("Max cards reached");
                }
            }
    }

    //Play a card
    public bool PlayCard(GameObject card)
    {
        if (availablePlaySlots.Count >= 4) return false;

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
        return true;
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
    private void ReturnToEnemyHand(GameObject card)
    {
        Debug.Log("Back in enemy hand");
        if (availableHandSlots.Count < 5)
        {
            card.GetComponent<Card>().StartMove(enemyHandSlotsPos[enemyHandSlots.Count].position);

            enemyPlaySlots.Remove(card.GetComponent<Card>());
            enemyHandSlots.Add(card.GetComponent<Card>());
        }
    }

    //-->Open bestiary


    //-->Bell rang
    public void RingBell()
    {
        Bell.interactable = false;
        _isBattling = true;

        Debug.Log("Bell rang");
        foreach (Card card in enemyPlaySlots)
        {
            ChangeCardSprite(card._cardFront, card);
        }

        StartCoroutine(Battles());
        
    }

    //----- "Enemy"
    IEnumerator EnemyTurnStart()
    {
        ShowCards();
        yield return new WaitForSeconds(showTime);
        foreach (Card card in enemyHandSlots)
        {
            ChangeCardSprite(cardBack, card);
        }
        yield return new WaitForSeconds(0.5f);
        foreach (Card c in enemyHandSlots)
        {
            c.StartMove(enemyHandSlotsPos[1].position);
        }

        ShuffleHand(enemyHandSlots);
        yield return new WaitForSeconds(2f);
        for (int i = 0; i < enemyHandSlots.Count; i++)
        {
            enemyHandSlots[i].StartMove(enemyPlaySlotsPos[i].position);
            enemyPlaySlots.Add(enemyHandSlots[i]);
        }
        enemyHandSlots.Clear();


    }

    //Card show
    private void ShowCards()
    {
        for (int i = enemyHandSlots.Count; i < enemyHandSlotsPos.Length; i++)
        {
            var c = Instantiate(enemyCards[Random.Range(0, enemyCards.Count)]);
            c.transform.position = enemyHandSlotsPos[i].position;
            enemyHandSlots.Add(c.GetComponent<Card>());
        }
    }

    // Randomly shuffle the enemy hand and play them
    public List<Card> ShuffleHand(List<Card> hand)
    {
        // Fisher-Yates shuffle argorithm (source in file)
        for (int i = hand.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            var temp = hand[i];
            hand[i] = hand[j];
            hand[j] = temp;
        }
        return hand;
    }


    private void ChangeCardSprite(Sprite s, Card c)
    {
        c.gameObject.GetComponent<SpriteRenderer>().sprite = s;
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
    private IEnumerator Battles(){
        yield return new WaitForSeconds(4f);
        List<Tuple<bool, bool>> battles = new List<Tuple<bool, bool>>();
        for (int i = 0; i < availablePlaySlots.Count; i++)
        {
            battles.Add(SingleBattle(availablePlaySlots[i], enemyPlaySlots[i]));
        }
        yield return new WaitForSeconds(1);

        List<Card> toBeReturned = new();
        List<Card> returnEnemy = new();

        for (int i = 0; i < battles.Count; i++)
        {
            if (battles[i].Item1 == false)
            {
                Destroy(availablePlaySlots[i].gameObject);
            }
            else
            {
                toBeReturned.Add(availablePlaySlots[i]);
            }

            if (battles[i].Item2 == false)
            {
                Destroy(enemyPlaySlots[i].gameObject);
            }
            else
            {
                returnEnemy.Add(enemyPlaySlots[i]);
            }
            yield return new WaitForSeconds(1);
        }
        foreach (Card c in toBeReturned) {
            ReturnCardToHand(c.gameObject);
            c.ResetPlay();
        }
        foreach (Card c in returnEnemy)
        {
            ReturnToEnemyHand(c.gameObject);
            c.ResetPlay();
        }
        availablePlaySlots.Clear();
        enemyPlaySlots.Clear();

        _defeatedCards = _defeatedCards + 4 - enemyHandSlots.Count; //Edit this to amount cards defeated
        CardsDefeated();

        yield return new WaitForSeconds(2f);
        StartCoroutine(EnemyTurnStart()); // Resets the opponent's hand. check if can be here or needs a different spot in case a battle should end
        _isBattling = false;
    }

    //---- General
    public void CardsDefeated()
    {
        Debug.Log(_defeatedCards);
        cardsBeatenTxt.text = _defeatedCards.ToString() + "/" + GoalDefeatedCards; //Shows how many cards have been defeated in total
    }

    private void EndGame()
    {
        _resultPanel.SetActive(true);
        Debug.Log("Game end");
        if (_defeatedCards >= GoalDefeatedCards)
        {
            _result.text = "Game won!";
            _playedByEnemy.text = "Goal: " + GoalDefeatedCards + " Defeated: " + _defeatedCards;
            Debug.Log("Game won");
        }
        else
        {
            _result.text = "Game over...";
            _playedByEnemy.text = "Goal: " + GoalDefeatedCards + " Defeated: " + _defeatedCards;
            Debug.Log("Game lost");
        }
    }

    //>> ------ Roaming area ------ <<

    //Increase deck
    /*Can't play without 10 cards in deck*/


    //>> ------ UI ------ <<
    public void ContinueGame()
    {
        //Unfreeze the game
    }

    public void PauzeGame()
    {
        //Make sure nothing can be pressed & that the game "freezes"
    }



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
            if (!_GameOver)
            {
                EndGame();
                _GameOver = true;
            }
            //Call cards defeated
        }

        if (availablePlaySlots.Count == 4 && enemyPlaySlots.Count == 4 && !_isBattling)
        {
            Bell.interactable = true;
        }
        else
        {
            Bell.interactable = false;
        }
    }
}