using Unity.VisualScripting;
using UnityEngine;

public class MonsterTemporaily : MonoBehaviour
{

    Rigidbody2D rb;
    public GameObject target;
    public float speed;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        Vector2 next = ((Vector2)target.transform.position - rb.position).normalized;
        rb.MovePosition(rb.position + next * speed * Time.deltaTime);
    }
}
