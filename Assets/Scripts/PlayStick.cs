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
        
        // ���콺 -> ���� ��ǥ
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        mouseWorldPos.z = 0f;
        // �÷��̾� - ���콺 ���� �Ÿ� 
        Vector3 dir = (mouseWorldPos - player.position).normalized;
        // ���� ���˴� ��ġ ���
        transform.position = player.position + dir * distanceFromPlayer;
    }
}
