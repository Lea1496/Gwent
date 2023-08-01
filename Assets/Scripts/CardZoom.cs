using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using UnityEngine;

public class CardZoom : MonoBehaviour
{
    public GameObject Canvas;

    public GameObject zoomCard;
    // Start is called before the first frame update
    void Awake()
    {
        Canvas = GameObject.Find("Canvas");
    }

    
    public void OnEnter()
    {
        zoomCard = Instantiate(gameObject, new Vector2(Input.mousePosition.x, Input.mousePosition.y + 20),
            Quaternion.identity);
        zoomCard.transform.SetParent(Canvas.transform, false);
        zoomCard.transform.SetLocalPositionAndRotation(new Vector3(0,transform.localPosition.y,transform.localPosition.z), transform.rotation);
        zoomCard.layer = 10;
        RectTransform rect = zoomCard.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(8, 6);
    }

    public void OnExit()
    {
        Destroy(zoomCard);
    }
    // Update is called once per frame
    void Update()
    {
        /*if (Input.touches.Length > 0)
        {
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                OnEnter();
            }
            else if (Input.touches[0].phase == TouchPhase.Ended)
            {
                OnExit();
            }
        }*/
    }
}

