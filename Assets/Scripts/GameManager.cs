using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("---- Player & card game data ---")]
    //Card game data
    public List<Card> deck = new List<Card>();
    public Transform[] handDeckSlots;
    public Transform[] playSlots;
    public List<Card> availableHandSlots = new List<Card>();
    public List<Card> availablePlaySlots = new List<Card>();
    private List<Card> ToBeRetured = new List<Card>();

    private bool _GameOver = false;
    private int _defeatedCards = 0;
    [SerializeField] private int GoalDefeatedCards;

    //[SerializeField] private BoxCollider2D ColliderCard1;

    [Header("---- Enemy variables ---")]
    //Enemy variables
    public Transform[] enemyHandSlotsPos;
    public Transform[] enemyPlaySlotsPos;
    public List<GameObject> enemyCards = new List<GameObject>();
    public List<Card> enemyHandSlots = new List<Card>();
    public List<Card> enemyPlaySlots = new List<Card>();
    [SerializeField] private Sprite cardBack;

    [SerializeField] private float showTime = 5;

    [Header("---- UI ---")]
    //UI data
    public TMP_Text deckAmount;
    [SerializeField] Button Bell;
    private bool _isBattling = false;
    public TMP_Text cardsBeatenTxt;
    [SerializeField] private TMP_Text _result;
    [SerializeField] private GameObject _resultPanel;
    [SerializeField] private TMP_Text _showResult;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _warningPanelPause;
    [SerializeField] private GameObject _warningMaxCards;
    [SerializeField] private GameObject _bell;
    [SerializeField] private Sprite _bellActive;
    [SerializeField] private Sprite _bellInactive;

    [SerializeField] private GameObject _speechPanel;
    [SerializeField] private TMP_Text _speechText;

    [SerializeField] private GameObject _bestiary;
    [SerializeField] private GameObject _page1Panel;
    [SerializeField] private GameObject _page2Panel;
    private bool _isPage1 = true;


    [Header("---- Tutorial ---")]
    //Tutorial
    public bool tutorialActive;
    private bool _playerCardsDrawn;
    private bool _bellRang;
    private bool _enemyHasPlacedCards;

    [Header("---- Timer ---")]
    //Timer
    [SerializeField] TextMeshProUGUI TimerText;
    [SerializeField] float remainingTime;

    //Other
    private bool _gamePaused = false;

    private void Start()
    {
        //add if not tutorial then enemy turn start otherwise don't run at all, it will get called
        if (!tutorialActive)
        {
            StartCoroutine(EnemyTurnStart());
        }
        else
        {
            StartCoroutine(Tutorial());
        }
        Bell.interactable = false;
        Debug.Log(tutorialActive);
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
                Debug.Log("Max cards reached");
                if (tutorialActive)
                {
                    _playerCardsDrawn = true;
                }
                else
                {
                    StartCoroutine(MaxCardsReached());
                }
                }
            }
    }

    private IEnumerator MaxCardsReached()
    {
        _speechPanel.SetActive(true);
        _speechText.text = "You can't grab more cards mate";
        yield return new WaitForSeconds(3.5f);
        _speechPanel.SetActive(false);
        yield return null;
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

        for (int i = 0; i < availableHandSlots.Count; i++) //moves the cards to the left if there is space available
        {
            availableHandSlots[i].GetComponent<Card>().StartMove(handDeckSlots[i].position);
        }
        for (int i = 0; i < availablePlaySlots.Count; i++)
        {
            availablePlaySlots[i].GetComponent<Card>().StartMove(playSlots[i].position);
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
    public void OpenBestiary()
    {
        _bestiary.SetActive(true);
        _page2Panel.SetActive(false);
        Time.timeScale = 0;
    }

    public void BestiaryNextpage()
    {
        if (_isPage1 == true)
        {
            _page1Panel.SetActive(false);
            _page2Panel.SetActive(true);
            _isPage1 = false;
        }
        else
        {
            _page1Panel.SetActive(true);
            _page2Panel.SetActive(false);
            _isPage1 = true;
        }
    }

    public void CloseBestiary()
    {
        _bestiary.SetActive(false);
        Time.timeScale = 1;
    }

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
        _bellRang = true;
        DeactivatePlayerCardsCollision();
        //this prevents the coroutine to start too early during the tutorial
        if (!tutorialActive)
        {
            StartCoroutine(Battles());
        }
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
        _enemyHasPlacedCards = true;

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
                ToBeRetured.Add(availablePlaySlots[i]);
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
        ActivatePlayerCardsCollision();

        _defeatedCards = _defeatedCards + 4 - enemyHandSlots.Count; //Edit this to amount cards defeated
        CardsDefeated();

        yield return new WaitForSeconds(2f);
        StartCoroutine(EnemyTurnStart()); // Resets the opponent's hand. check if can be here or needs a different spot in case a battle should end
        _isBattling = false;
    }

    private void DeactivatePlayerCardsCollision()
    {
        Debug.Log("Deactivate cards");
        foreach (Card c in availablePlaySlots) //need to save cards that have been placed when the bell was rang
        {
            c.GetComponent<Card>().DisableCard();
        }
    }

    private void ActivatePlayerCardsCollision()
    {
        foreach (Card c in ToBeRetured)
        {
            c.GetComponent<Card>().EnableCard();
        }
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
            _showResult.text = "Goal: " + GoalDefeatedCards + " Defeated: " + _defeatedCards;
            Debug.Log("Game won");
        }
        else
        {
            _result.text = "Game over...";
            _showResult.text = "Goal: " + GoalDefeatedCards + " Defeated: " + _defeatedCards;
            Debug.Log("Game lost");
        }
    }

    //>> ------ Tutorial ------ <<
    public void TutorialSelected()//just make sure this gets triggered
    {
        tutorialActive = true; 
    }

    private IEnumerator Tutorial() //call at start
    {
        yield return new WaitForSeconds(2f);
        //Pause certain parts/lengthen duration on waiting time
        //Add text to the text below
        Debug.Log("Tutorial level");
        _speechPanel.SetActive(true);
        _speechText.text = "Welcome to the game table";
        yield return new WaitForSeconds(3.5f);
        _speechText.text = "So here’s how it works";
        yield return new WaitForSeconds(3.5f);
        _speechText.text = "You, my friend, are trying to protect your territory from incoming enemies";
        yield return new WaitForSeconds(4.5f);
        _speechText.text = "Your forest friends are there to help you";
        yield return new WaitForSeconds(3.5f);
        _speechText.text = "You can get cards on your right side";
        yield return new WaitForSeconds(3.5f);

        _speechText.text = "‘Left click’ to interact and draw cards";
        yield return new WaitUntil(() => _playerCardsDrawn == true);

        _speechText.text = "I will grab some cards as well";
        StartCoroutine(EnemyTurnStart());
        yield return new WaitUntil( () => _enemyHasPlacedCards == true);

        _speechText.text = "Now choose your cards and place them in front of you.";
        yield return new WaitForSeconds(3.5f);
        _speechText.text = "And when you’re ready, ring the bell";
        yield return new WaitUntil(() => _bellRang == true);


        //condition that if they hit the bell this will continue
        //_speechText.text = "Before battle, you’ll get to see my cards for a moment before I place them";
        //yield return new WaitForSeconds(3.5f);

        /*
        Goal of the battle
        Why the battle
        Draw cards *
        See enemy cards (press 'space' to continue) *
        Enemy cards then move to place
        Pick cards themselves *
        Press bell when ready *
        Battle starts -> pauze after the cards have been turned
        Wait with damaging and explain hp/dmg deal
        Explain that remaining cards keep their stats by the end of battle
        Player may always see which cards 'I' use before battle
        Beat as many cards as needed within the time to protect your lands
         */
    }


    //>> ------ UI ------ <<
    public void ContinueGame()
    {
        _pausePanel.SetActive(false);
        Time.timeScale = 1;
        _gamePaused = false;
    }

    public void PauseGame()
    {
        _pausePanel.SetActive(true);
        _warningPanelPause.SetActive(false);
        Time.timeScale = 0;
    }

    //Update every frame
    private void Update()
    {

        deckAmount.text = deck.Count.ToString(); //cards in deck to UI

        if (!tutorialActive)
        {
            //Timer
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
            }
        }

        if (availablePlaySlots.Count == 4 && enemyPlaySlots.Count == 4 && !_isBattling)
        {
            Bell.interactable = true;
            _bell.GetComponent<Image>().overrideSprite = _bellActive;
        }
        else
        {
            Bell.interactable = false;
            _bell.GetComponent<Image>().overrideSprite = _bellInactive;
        }

        if(_gamePaused == false && Input.GetKeyDown(KeyCode.P))
        {
            _gamePaused = true;
            PauseGame();
        }
    }
}
