using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera vcam;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // �� ��ȯ �� ���� ī�޶� �ٽ� ã��
        vcam = FindFirstObjectByType<CinemachineCamera>();
        AssignFollowTarget();
    }

    private void AssignFollowTarget()
    {
        if (vcam == null) return;

        // ���� ���� �÷��̾� ã��
        foreach (var player in FindObjectsOfType<NetworkObject>())
        {
            if (player.IsOwner)
            {
                vcam.Follow = player.transform;
                vcam.LookAt = player.transform;
                break;
            }
        }
    }
}