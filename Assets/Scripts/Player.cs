using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;

    Rigidbody2D rigid;

    // 처음 한번만 
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        // 위치이동
        rigid.MovePosition(rigid.position + nextVec);
    }
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
}
