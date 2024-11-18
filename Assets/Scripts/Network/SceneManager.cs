using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

// Ref : https://github.com/CodeSmile-0000011110110111/UnityNetcodeBiteSizeExamples/blob/main/Assets/Plugins/CodeSmile/Netcode/QuickStart/04_Lobby/Scripts/LobbyGuiController.cs#L52

namespace Network
{
    public class CustSceneManager : NetworkBehaviour
    {
        private const int MaxPlayers = 2;

        [Header("For debugging only")]
        [SerializeField] private bool[] _readyStates;

        private void Awake()
        {
            _readyStates = new bool[MaxPlayers];
        }

        public bool ArePlayersReady()
        {
            var readyCount = 0;
            if (!_readyStates[NetworkManager.Singleton.LocalClientId])
                _readyStates[NetworkManager.Singleton.LocalClientId] = true;
            
            for (var i = 0; i < MaxPlayers; i++)
                if (_readyStates[i])
                    readyCount++;
            
            return readyCount == MaxPlayers;
        }

        public void SetPlayerReadyState(ulong clientId, bool readyState)
        {
            _readyStates[clientId] = readyState;
            
            if(IsServer)
                BroadcastPlayerReadyStates();
        }
        
        public void BroadcastPlayerReadyStates()
        {
            var stateBits = 0;
            for (var i = 0; i < MaxPlayers; i++)
                if (_readyStates[i])
                    stateBits |= 1 << i;

            UpdatePlayerReadyStatesClientRpc((byte)stateBits);
        }

        [ClientRpc]
        private void UpdatePlayerReadyStatesClientRpc(byte stateBits)
        {
            if (IsServer == false)
            {
                for (var i = 0; i < MaxPlayers; i++)
                    _readyStates[i] = (stateBits & 1 << i) != 0;
            }
        }

        public void OnClientReadyStateChanged(bool ready)
        {
            Debug.Log($"Client calling ready state change: {ready}");
            ClientReadyStateChangedServerRpc((byte)(ready ? 1 : 0));
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClientReadyStateChangedServerRpc(byte readyBit, ServerRpcParams rpcParams = default)
        {
            Debug.Log($"Server received ready state change from {rpcParams.Receive.SenderClientId}: {readyBit}");
            SetPlayerReadyState(rpcParams.Receive.SenderClientId, readyBit != 0);
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void LoadSceneSingleOnDemandServerRpc()
        {
            if (IsServer && NetworkManager.IsListening)
            {
                var status = NetworkManager.SceneManager?.LoadScene("SampleScene", LoadSceneMode.Single);
                if (status != SceneEventProgressStatus.Started)
                    Debug.LogError("Scene not loaded");
            }
        }
    }
}