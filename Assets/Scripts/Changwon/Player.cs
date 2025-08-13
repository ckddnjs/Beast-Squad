using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
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
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        // ��ġ�̵�
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        ani.SetFloat("speed", inputVec.magnitude);

        if(inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0 ? true : false;
        }
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
}
