using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    bool hasBeenPlayed;
    public int cardIndex;
    private GameManager Gm;

    private bool move = false;
    private Vector3 movePoint = Vector3.zero;

    public int hp = 0;
    public int dmg = 0;

    public Sprite _cardFront;

    //Visual
    [SerializeField] private TextMeshPro _hpTxt;
    [SerializeField] private TextMeshPro _dmgTxt;

    private void Start()
    {
        _cardFront = gameObject.GetComponent<SpriteRenderer>().sprite;
        Gm = FindAnyObjectByType<GameManager>();
        //_hpTxt.text = hp.ToString();
        //_dmgTxt.text = dmg.ToString();
    }

    private void Update()
    {
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint, 15f);
            if (transform.position == movePoint)
            {
                move = false;
            }
        }
    }

    public void StartMove(Vector3 moveP)
    {
        movePoint = moveP;
        move = true;
    }

    private void OnMouseDown()
    {
        if (hasBeenPlayed == false) 
        {
            //Move to play area
            Debug.Log("Played");
            hasBeenPlayed = Gm.PlayCard(this.gameObject); 
            Debug.Log(hasBeenPlayed);
        }
        else if (hasBeenPlayed == true)
        {
            //Move back to hand
            Debug.Log("returning");
            Gm.ReturnCardToHand(this.gameObject);
            hasBeenPlayed = false;
        }
    }

    public void ResetPlay()
    {
        hasBeenPlayed = false;
    }


}