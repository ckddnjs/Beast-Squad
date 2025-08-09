using Unity.Netcode;
using UnityEngine;

public class PlayerMove : NetworkBehaviour
{
    public float speed = 5f;
    private Rigidbody2D rb;
    private Vector3 move;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!IsOwner) return; // 내 캐릭터만 조작 가능

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        move = new Vector3(h, v, 0);
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        rb.MovePosition(transform.position + move * speed * Time.fixedDeltaTime);
    }
}
