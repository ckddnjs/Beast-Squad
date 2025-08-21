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

    // 처음 한번만 
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
        // 위치이동
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if (!IsOwner) return;
        ani.SetFloat("speed", inputVec.magnitude);

        if(inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0 ? true : false;
        }
    }
    void OnMove(InputValue value)
    {
        if (!IsOwner) return;
        inputVec = value.Get<Vector2>();
    }
}
