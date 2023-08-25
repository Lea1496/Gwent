using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragDrop : MonoBehaviour
{
    /*// public GameManager GameManager;
     public GameObject Canvas;
     //public PlayerManager PlayerManager;

     private bool isDragging = false;
     private bool isOverDropZone = false;
     private bool isDraggable = true;
     private GameObject dropZone;
     private GameObject startParent;
     private Vector2 startPosition;

     private void Start()
     {
         /*GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
         Canvas = GameObject.Find("Main Canvas");
         NetworkIdentity netWorkIdentity = NetworkClient.connection.identity;
         PlayerManager = netWorkIdentity.GetComponent<PlayerManager>();

         if (!hasAuthority)
         {
             isDraggable = false;
         }
     }
     void Update()
     {
         if (isDragging)
         {
             transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
             transform.SetParent(Canvas.transform, true);
         }        
     }

    /* private void OnCollisionEnter2D(Collision2D collision)
     {
         if (collision.gameObject == PlayerManager.PlayerSockets[PlayerManager.CardsPlayed])
         {
             isOverDropZone = true;
             dropZone = collision.gameObject;
         }
     }

     private void OnCollisionExit2D(Collision2D collision)
     {
         isOverDropZone = false;
         dropZone = null;
     }

     public void StartDrag()
     {
         //if (!isDraggable) return;
         startParent = transform.parent.gameObject;
         startPosition = transform.position;
         isDragging = true;
     }

     public void EndDrag()
     {
         if (!isDraggable) return;
         isDragging = false;

        /* if (isOverDropZone && PlayerManager.IsMyTurn)
         {
             transform.SetParent(dropZone.transform, false);
             isDraggable = false;
             PlayerManager.PlayCard(gameObject);
         }
         else
         {
             transform.position = startPosition;
             transform.SetParent(startParent.transform, false);
         }
     }*/

    private bool isDragging = false;
    private bool isOverDropZone = false;
    private Vector2 startPosition;
    private Vector2 endPosition;
    private GameObject dropZone;
    private Touch touch;
    public bool isDraggable = true;
    private int rank;

    private GameManager gameManager;
    private void Start()
    {
        //rank = gameObject.GetComponent<CardBehavior>().rank;
        //Debug.Log(rank);
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        /*if (Input.touches.Length > 0)
        {
            Debug.Log("touch");
            touch = Input.touches[0];
            if (touch.phase == TouchPhase.Began)
            {

                StartDragging();
            }
            else
            {
                if (touch.phase == TouchPhase.Ended)
                {
                    EndDragging();
                }
            }
        }*/


        //if (isDragging)
        //{
        //    transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //}
    }

    private void OnCollisionEnter2D(Collision2D col)
    {

        //if (!col.gameObject.CompareTag("Card"))
        //{
        //    Debug.Log("Enter" + col.collider.gameObject.name);
        //    isOverDropZone = true;
        //    dropZone = col.gameObject;
        //}
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        //if (!other.gameObject.CompareTag("Card"))
        //{
        //    Debug.Log("Exit" + other.collider.gameObject.name);
        //    isOverDropZone = false;
        //    dropZone = null;
        //}
    }

    public void StartDragging()
    {
        //if (isDraggable)
        //{
        //    Debug.Log("startDragging");
        //    startPosition = transform.position;
        //    isDragging = true;
        //}

    }

    public void EndDragging()
    {

        //isDragging = false;

        //CardBehavior cardB = gameObject.GetComponent<CardBehavior>();


        //if (isOverDropZone && rank == dropZone.layer)
        //{
        //    DropCard(cardB);

        //    //transform.SetLocalPositionAndRotation(new Vector3(0,transform.localPosition.y,transform.localPosition.z), transform.rotation);
        //}
        //else
        //{
        //    if (isOverDropZone && cardB.ability == 14)
        //    {
        //        cardB.rank = 14;

        //        if (cardB.rank + 1 == dropZone.layer || cardB.rank + 2 == dropZone.layer)
        //        {
        //            DropCard(cardB);
        //        }
        //    }
        //    else
        //    {
        //        gameObject.transform.position = startPosition;
        //    }

        //}
    }

    private void DropCard(CardBehavior cardB)
    {
        //Debug.Log("ici");
        //List<GameObject> row;
        //int nbCardsInRow = 1;
        //float offset = 0;
        //float width;

        //bool isNotComHorn = false;
        //if (rank < 13)
        //{
        //    isNotComHorn = true;
        //    row = BoardManager.wholeBoard[rank - 7];
        //    nbCardsInRow = row.Count;
        //    width = 844.0f;
        //    BoardManager.wholeBoard[rank - 7].Add(cardB.gameObject);
        //}
        //else
        //{
        //    if (rank < 20)
        //    {
        //        row = new List<GameObject>();
        //        width = 141f;
        //    }
        //    else
        //    {
        //        isNotComHorn = true;
        //        row = BoardManager.weatherCards;
        //        width = 261f;
        //        BoardManager.weatherCards.Add(cardB.gameObject);
        //    }
        //}

        //if (isNotComHorn)
        //{
        //    if (row.Count + 1 < 6)
        //    {
        //        offset = width / (nbCardsInRow + 1);
        //    }
        //    else
        //    {
        //        offset = width / (nbCardsInRow + 1);
        //    }
        //    Debug.Log("drop " + dropZone.name);

        //    gameManager.AddCard(dropZone, row, offset);
        //}



        //isDraggable = false;

        //BoardManager.cardsInHand.Remove(cardB);
        //Debug.Log($"TEST + {cardB.name}");
        //gameManager.physicalCards.Remove(gameObject);
        //BoardManager.board.Add(gameObject);
        //if (cardB.indice >= 0)
        //{
        //    //gameManager.RemoveCard(cardB.indice, gameManager.zoneCard, gameManager.physicalCards);
        //}


        //transform.position = new Vector2(dropZone.transform.position.x - width / 2 + offset * nbCardsInRow, dropZone.transform.position.y);

        //cardB.OnDrop();
        //cardB.indice = nbCardsInRow;
        //VerifyIfAbilityIsUsedOn();
    }

    private void VerifyIfAbilityIsUsedOn()
    {
        //CardBehavior cardB = gameObject.GetComponent<CardBehavior>();
        //if (rank < 10 && !cardB.isHero)
        //{

        //    if (gameManager.isComUsed[rank - 7])
        //    {
        //        cardB.power *= 2;
        //    }
        //    if (gameManager.isWeatherUsed[rank - 7])
        //    {
        //        cardB.power -= (cardB.ogPower - 1);
        //    }
        //}
    }

}