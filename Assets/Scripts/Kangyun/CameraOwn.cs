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

    // �� ��ȯ ������ �÷��̾ �ʰ� ������ ���� ������, �ʿ� �� Update���� �õ��� ���� ����
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