using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public Vector2 inputVec;
    public float speed;

    Rigidbody2D rigid;

    // ó�� �ѹ��� 
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        // ��ġ�̵�
        rigid.MovePosition(rigid.position + nextVec);
    }
    void OnMove(InputValue value)
    {
        if (!IsOwner) return;
        inputVec = value.Get<Vector2>();
    }
}
