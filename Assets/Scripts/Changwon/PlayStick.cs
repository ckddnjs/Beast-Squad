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
        // ���콺 -> ���� ��ǥ
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;
        // �÷��̾� - ���콺 ���� �Ÿ� 
        Vector3 dir = (mouseWorldPos - player.position).normalized;
        // ���� ���˴� ��ġ ���
        transform.position = player.position + dir * distanceFromPlayer;
    }
}
