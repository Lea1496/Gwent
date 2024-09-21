using System.Linq;
using GwentEngine;
using UnityEngine;

public class DragDrop : MonoBehaviour
{
    private bool _isDragging;
    private Location _dropLocation;
    private bool _canDrop;
    private Vector2 _initialDraggingPosition;
    private bool _isGameManager;
    private CardBehavior _cardBehavior;
    private GameManager _gameManager;


    private GameObject FindManager<T>() where T : class
    {
        var allManagers = GameObject.FindObjectsOfType<MonoBehaviour>(true);

        // Filter those that implement the interface
        return allManagers.FirstOrDefault(m => m.GetComponent(typeof(T)))?.gameObject;
    }
    private void Start()
    {
        _gameManager = FindManager<IManager>().GetComponent<IManager>() as GameManager;
        _isGameManager = _gameManager != null;
        _cardBehavior = gameObject.GetComponent<CardBehavior>();
    }

    private void Update()
    {
        if (_isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (_isGameManager)
        {
            if (!col.gameObject.CompareTag("Card"))
            {
                _dropLocation = _gameManager.GetLocation(_cardBehavior.Card.EffectivePlayer, col.gameObject);
                _canDrop = _gameManager.CanPlay(_cardBehavior.Card.Number, _dropLocation);

                Debug.Log($"Enter : {col.collider.gameObject.name}. CanDrop: {_canDrop}. Drop Location: {_dropLocation}. Possible locations: {_cardBehavior.Card.PossibleLocations}");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Card"))
        {
            Debug.Log("Exit" + other.collider.gameObject.name);
            _dropLocation = Location.None;
        }
    }

    public void StartDragging()
    {
        if (_isGameManager)
        {
            _initialDraggingPosition = gameObject.transform.position;
            _isDragging = _gameManager.IsDraggable(_cardBehavior.Card);
        }
        
    }

    public void EndDragging()
    {
        _isDragging = false;

        if (_isGameManager)
        {
            if (_dropLocation != Location.None && _canDrop)
            {
                Debug.Log($"Playing : {_cardBehavior.Card}. Drop Location: {_dropLocation}.");
                _gameManager.Play(_cardBehavior.Card.Number, _dropLocation);
            }
            else
            {
                gameObject.transform.position = _initialDraggingPosition;
            }
        }
        
    }
}