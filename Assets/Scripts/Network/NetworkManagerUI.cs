using System;
using GwentEngine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using NetworkManager = Unity.Netcode.NetworkManager;

namespace Network
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField] private Button serveBtn;
        [SerializeField] private Button clientBtn;
        [SerializeField] private Button hostBtn;
        [SerializeField] private GameObject dbm;
        private void Awake()
        {
            serveBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });
            
            clientBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
                OtherPlayerInfo.IsHost = false;
                
                dbm.GetComponent<DeckBuilderManager>().MyAwake();
               // dbm.SetActive(true);
               
               gameObject.SetActive(false);
            });
            
            hostBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
                OtherPlayerInfo.IsHost = true;
                
                dbm.GetComponent<DeckBuilderManager>().MyAwake();
                
                //dbm.SetActive(true);
            });
        }
    }
}