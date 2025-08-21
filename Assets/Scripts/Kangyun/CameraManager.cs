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
        // 씬 전환 시 가상 카메라 다시 찾기
        vcam = FindFirstObjectByType<CinemachineCamera>();
        AssignFollowTarget();
    }

    private void AssignFollowTarget()
    {
        if (vcam == null) return;

        // 현재 로컬 플레이어 찾기
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