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
   
   
   private void Update()
   {
       if (isDragging)
       {
           transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
       }
   }

   private void OnCollisionEnter2D(Collision2D col)
   {
       Debug.Log("Enter" + col.collider.gameObject.name);
       isOverDropZone = true;
       dropZone = col.gameObject;
   }

   private void OnCollisionExit2D(Collision2D other)
   {
       Debug.Log("Exit" + other.collider.gameObject.name);
       isOverDropZone = false;
       dropZone = null;
   }

   public void StartDragging()
   {
       startPosition = transform.position;
       isDragging = true;
   }

   public void EndDragging()
   {
        isDragging = false;

        if (isOverDropZone)
        { 
            Debug.Log("drop " + dropZone.name);
            transform.SetParent(dropZone.transform, false);
          
            transform.SetLocalPositionAndRotation(new Vector3(0,0,transform.position.z), transform.rotation);
        }
        else
        {
            transform.position = startPosition;
        }
   }
}