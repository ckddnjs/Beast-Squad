using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayStick : NetworkBehaviour
{
    public Transform player;
    public float distanceFromPlayer;
    public Camera cam;
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        FindCamera();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCamera();
    }
    private void FindCamera()
    {
        var brain = FindFirstObjectByType<CinemachineBrain>();
        if (brain != null)
        {
            cam = brain.GetComponent<Camera>();
        }
        else
        {
            cam = Camera.main; // fallback
        }
    }
    void Update()
    {
        if (!player.TryGetComponent<NetworkObject>(out var netObj) || !netObj.IsOwner)
            return;
        
        // 마우스 -> 월드 좌표
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        mouseWorldPos.z = 0f;
        // 플레이어 - 마우스 사이 거리 
        Vector3 dir = (mouseWorldPos - player.position).normalized;
        // 놀이 낚싯대 위치 계산
        transform.position = player.position + dir * distanceFromPlayer;
    }
}
