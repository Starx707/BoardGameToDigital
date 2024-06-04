using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    bool hasBeenPlayed;
    public int cardIndex;
    private GameManager Gm;
    private bool move = false;
    private Vector3 movePoint = Vector3.zero;
    public int hp = 0;
    public int dmg = 0;

    private void Start()
    {
        Gm = FindAnyObjectByType<GameManager>();
    }

    private void Update()
    {
        if (move)
        {
            transform.position = Vector3.MoveTowards(transform.position, movePoint, 3f);
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
            Gm.PlayCard(this.gameObject);
            hasBeenPlayed = true;
            //Gm.availableHandSlots[cardIndex] = null;
        }
        else if (hasBeenPlayed == true)
        {
            //Move back to hand
            Gm.ReturnCardToHand(this.gameObject);
            hasBeenPlayed = false;
            //Gm.availablePlaySlots[cardIndex] = this;
        }
    }


}