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
       rank = gameObject.GetComponent<CardBehavior>().rank;
       Debug.Log(rank);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
       
       
       if (isDragging)
       {
           transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
       }
   }

   private void OnCollisionEnter2D(Collision2D col)
   {

       if (!col.gameObject.CompareTag("Card"))
       {
           Debug.Log("Enter" + col.collider.gameObject.name);
           isOverDropZone = true;
           dropZone = col.gameObject;
       }
   }

   private void OnCollisionExit2D(Collision2D other)
   {
       if (!other.gameObject.CompareTag("Card"))
       {
           Debug.Log("Exit" + other.collider.gameObject.name);
           isOverDropZone = false;
           dropZone = null;
       }
   }

   public void StartDragging()
   {
       if (isDraggable)
       {
           Debug.Log("startDragging");
           startPosition = transform.position;
           isDragging = true;
       }
       
   }

   public void EndDragging()
   {
        
        isDragging = false;
        
        CardBehavior cardB = gameObject.GetComponent<CardBehavior>();
        
        
        if (isOverDropZone && rank == dropZone.layer)
        {
            DropCard(cardB);
            
            //transform.SetLocalPositionAndRotation(new Vector3(0,transform.localPosition.y,transform.localPosition.z), transform.rotation);
        }
        else
        {
            if (isOverDropZone && cardB.ability == 14)
            {
                cardB.rank = 14;

                if (cardB.rank + 1 == dropZone.layer ||  cardB.rank + 2 == dropZone.layer)
                {
                    DropCard(cardB);
                }
            }
            else
            {
                gameObject.transform.position = startPosition;
            }
            
        }
   }

   private void DropCard(CardBehavior cardB)
   {
       int nbCardsInRow = BoardManager.wholeBoard[rank - 7].Count;
       float offset = 844.0f / (nbCardsInRow + 1);
       Debug.Log("drop " + dropZone.name);
       foreach (var card in BoardManager.board)
       {
           if (card.GetComponent<CardBehavior>().rank == cardB.rank && !card.GetComponent<DragDrop>().isDraggable)
           {
               card.transform.position = new Vector2(dropZone.transform.position.x - 844.0f/2 + offset * card.GetComponent<CardBehavior>().indice, dropZone.transform.position.y);
           }
       }

       if (cardB.indice >= 0)
       {
           gameManager.RemoveCardInHands(GetComponent<CardBehavior>().indice);
       }
       transform.position = new Vector2(dropZone.transform.position.x - 844.0f/2 + offset *  nbCardsInRow, dropZone.transform.position.y);
       isDraggable = false;
       cardB.OnDrop();
       cardB.indice = nbCardsInRow;
       BoardManager.cardsInHand.Remove(cardB);
       gameManager.physicalCards.Remove(gameObject);    
       BoardManager.wholeBoard[rank - 7].Add(cardB.gameObject);
       BoardManager.board.Add(gameObject);
       
   }
   
}