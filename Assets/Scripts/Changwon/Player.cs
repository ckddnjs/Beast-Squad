using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public Vector2 inputVec;
    public float speed;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator ani;

    // ó�� �ѹ��� 
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();

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
    /*
    void LateUpdate()
    {
        if (!IsOwner) return;
        ani.SetFloat("speed", inputVec.magnitude);

        if(inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0 ? true : false;
        }
    }*/
    void OnMove(InputValue value)
    {
        if (!IsOwner) return;
        inputVec = value.Get<Vector2>();
    }
}
