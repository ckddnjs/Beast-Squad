using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class CameraFollowController : MonoBehaviour
{
    public CinemachineCamera virtualCamera;

    void Start()
    {
        SetFollowTarget();
    }

    // 씬 전환 등으로 플레이어가 늦게 생성될 수도 있으니, 필요 시 Update에서 시도할 수도 있음
    void SetFollowTarget()
    {

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (client.PlayerObject != null && client.PlayerObject.IsLocalPlayer)
            {
                virtualCamera.Follow = client.PlayerObject.transform;
                return;
            }
        }

        Debug.LogWarning("Local player not found yet.");
    }
}