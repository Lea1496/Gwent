
using Unity.Netcode;
using UnityEngine;


public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<PlayerNetworkData> netState = new(writePerm: NetworkVariableWritePermission.Owner);
    private Vector3 vel;
    private float rotVel;
    [SerializeField] private float cheapInterpolationTime = 0.1f;


    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            netState.Value = new PlayerNetworkData()
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles
            };
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, netState.Value.Position, ref vel,
                cheapInterpolationTime);
            transform.rotation = Quaternion.Euler(
                0,
                Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, netState.Value.Rotation.y, ref rotVel, cheapInterpolationTime),
                0);
        }
    }

    struct PlayerNetworkData
    {
        private float x, z;
        private float yRot;

        internal Vector3 Position
        {
            get => new Vector3(x, 0, z);

            set
            {
                x = value.x;
                z = value.z;
            }
        }

        internal Vector3 Rotation
        {
            get => new Vector3(0, yRot, 0);
            set => yRot = value.y;

        }

        private void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref z);
            
            serializer.SerializeValue(ref yRot);
        }
    }   
}
