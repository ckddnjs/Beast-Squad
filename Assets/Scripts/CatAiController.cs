using System;
using System.Collections;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CatAiController : MonoBehaviour
{
    public Transform player;
    public Transform playStick;
    public float speed;
    public float angleWithplayStick;
    public float followDelay;
    public GameObject Goal;

    private Coroutine followCoroutine = null;
    private bool isFollowingplayStick = false;

    public enum CatState {
        StartMove, 
        ToPlayer, 
        Idle, 
        Wander, 
        Flee, 
        GoalReached, 
        CatchToPlayer,
        AttackByMonsterWithPlayer 
    };
    // Starter : ���� ���۽�, 0
    // ToPlayer : �÷��̾�� ������ �پ�����, 1
    // Idle : ��� ���߾� �θ��� �Ÿ�����, 2
    // Wander ; �÷��̾�������� �������� �̵�, 3
    // Flee : ���ͷ� ���� ����ĥ��, 4
    // GoalReached :  ������ ���޽�, 5
    // CatchToPlayer : �÷��̾�� ��������, 6
    // AttackByMonsterWithPlayer : �÷��̾�� �Ȱ�µ� ���� ������ �޾�����, 7

    public CatState currentState = CatState.StartMove;

    // Ÿ�� 2��
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(StartMovement());
    }

    private void Update()
    {
      
        if (currentState != CatState.StartMove && currentState != CatState.Flee &&
            currentState != CatState.GoalReached && currentState != CatState.CatchToPlayer &&
            currentState != CatState.AttackByMonsterWithPlayer) 
        {
            FollowStick(); //�ൿ 3,4�� ��츸 ���� ����ؼ� ������
        }

        if (currentState == CatState.Idle)
        {
            // Idle �ϴ� �ڵ�
        }

        if (currentState == CatState.Wander) 
        {
            Vector2 playerPosition = player.position;
            Vector2 goalPosition = Goal.transform.position;

            Vector2 playerAndCat = (playerPosition - rb.position).normalized;
            Vector2 catAndGoal = (rb.position - goalPosition).normalized;

            float dot = Vector2.Dot(playerAndCat, catAndGoal);
            Debug.Log("���� : " + dot);

            if(dot >= 0) // ����϶� ��ǥ�� ���� �̵�, ���� ���⵵ ����ؼ� �˰��� ����(����)
            {
                // �ϴ� ������ �̵����� ����, ���ʵ��� �̵����� ���� ����
                rb.MovePosition(rb.position + catAndGoal * speed * Time.deltaTime);

            }
            else  // ���� �϶� �÷��̾� �ݴ�� ����, ���ʵ��� �̵����� �ڴ� ����
            {
                // ���� ����: �÷��̾� �� ����� ������ �������� ���� ȸ��
                Vector2 baseFleeDir = (rb.position - playerPosition).normalized;

                // ȸ�ǰ��� (��45��)
                float randomAngle = UnityEngine.Random.Range(-45f, 45f);
                float rad = randomAngle * Mathf.Deg2Rad;

                // ȸ���� ����
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);
                Vector2 rotatedFleeDir = new Vector2(
                    baseFleeDir.x * cos - baseFleeDir.y * sin,
                    baseFleeDir.x * sin + baseFleeDir.y * cos
                ).normalized;

                rb.MovePosition(rb.position + rotatedFleeDir * speed * Time.deltaTime);
            }
        }

        Debug.Log(currentState);
    }
    IEnumerator StartMovement()
    {
        float startTime = Time.time;
        while(Time.time - startTime < 3)
        {
            rb.MovePosition(rb.position + Vector2.up * speed * Time.deltaTime);
            yield return null;
        }
        currentState = CatState.Idle;
        
    }

    System.Collections.IEnumerator DelayAndFollow()
    {
        yield return new WaitForSeconds(followDelay);
        isFollowingplayStick = true;
        currentState = CatState.ToPlayer; //����� ���� ���� �κ�
        followCoroutine = null;
    }

    void FollowStick()
    {

        Vector2 playerToplayStick = (playStick.position - player.position).normalized;
        Vector2 playerToCat = (transform.position - player.position).normalized;
        float dot = Vector2.Dot(playerToplayStick, playerToCat);


        // �������� angleWithplayStick ���ؼ� ������ ���� ����
        if (dot >= angleWithplayStick)
        {
            if (!isFollowingplayStick && followCoroutine == null)
            {
                // �ڷ�ƾ ������ Coroutine�� ��ü�� �Ҵ�� ���� �Ʒ� DelayAndFollow �ȿ� null �� �ٽ� ����
                followCoroutine = StartCoroutine(DelayAndFollow());
            }
        }

        else // ������ �������� �ƴҶ�
        {
            if (isFollowingplayStick)
            {
                isFollowingplayStick = false;
                currentState = CatState.Wander; // 
            }
            if (followCoroutine != null)
            {
                StopCoroutine(followCoroutine);
                followCoroutine = null;
            }
        }

        if (isFollowingplayStick)
        {
            transform.position = Vector2.MoveTowards(transform.position, playStick.position, speed * Time.deltaTime);
        }
    }

}
