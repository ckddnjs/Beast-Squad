using UnityEngine;

public class PlayStick : MonoBehaviour
{
    public Transform player;
    public float distanceFromPlayer;

    void Start()
    {
        
    }

    void Update()
    {
        // 마우스 -> 월드 좌표
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        // 플레이어 - 마우스 사이 거리 
        Vector3 dir = (mouseWorldPos - player.position).normalized;
        // 놀이 낚싯대 위치 계산
        transform.position = player.position + dir * distanceFromPlayer;
    }
}
