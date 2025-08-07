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
    // Starter : 게임 시작시, 0
    // ToPlayer : 플레이어에게 가까이 붙엇을시, 1
    // Idle : 잠깐 멈추어 두리번 거리느것, 2
    // Wander ; 플레이어방향제외 무작위로 이동, 3
    // Flee : 몬스터로 부터 도망칠시, 4
    // GoalReached :  목적지 도달시, 5
    // CatchToPlayer : 플레이어에게 잡혔을때, 6
    // AttackByMonsterWithPlayer : 플레이어에게 안겼는데 몬스터 공격을 받았을때, 7

    public CatState currentState = CatState.StartMove;

    // 타겟 2개
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
            FollowStick(); //행동 3,4일 경우만 내적 계산해서 움직임
        }

        if (currentState == CatState.Idle)
        {
            // Idle 하는 코드
        }

        if (currentState == CatState.Wander) 
        {
            Vector2 playerPosition = player.position;
            Vector2 goalPosition = Goal.transform.position;

            Vector2 playerAndCat = (playerPosition - rb.position).normalized;
            Vector2 catAndGoal = (rb.position - goalPosition).normalized;

            float dot = Vector2.Dot(playerAndCat, catAndGoal);
            Debug.Log("각도 : " + dot);

            if(dot >= 0) // 양수일때 목표를 향해 이동, 맵의 복잡도 사용해서 알고리즘 구현(나중)
            {
                // 일단 일직선 이동으로 구현, 몇초동안 이동할지 랜덤 변수
                rb.MovePosition(rb.position + catAndGoal * speed * Time.deltaTime);

            }
            else  // 음수 일때 플레이어 반대로 도망, 몇초동안 이동할지 핸덤 변수
            {
                // 도망 방향: 플레이어 → 고양이 방향을 기준으로 랜덤 회전
                Vector2 baseFleeDir = (rb.position - playerPosition).normalized;

                // 회피각도 (±45도)
                float randomAngle = UnityEngine.Random.Range(-45f, 45f);
                float rad = randomAngle * Mathf.Deg2Rad;

                // 회전된 방향
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
        currentState = CatState.ToPlayer; //고양이 상태 변경 부분
        followCoroutine = null;
    }

    void FollowStick()
    {

        Vector2 playerToplayStick = (playStick.position - player.position).normalized;
        Vector2 playerToCat = (transform.position - player.position).normalized;
        float dot = Vector2.Dot(playerToplayStick, playerToCat);


        // 내적으로 angleWithplayStick 비교해서 일직선 범위 지정
        if (dot >= angleWithplayStick)
        {
            if (!isFollowingplayStick && followCoroutine == null)
            {
                // 코루틴 실행이 Coroutine의 객체가 할당됨 이후 아래 DelayAndFollow 안에 null 이 다시 대입
                followCoroutine = StartCoroutine(DelayAndFollow());
            }
        }

        else // 내적이 일직석이 아닐때
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
