using System;
using System.Collections;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class CatAiController : MonoBehaviour
{
    public Transform player;
    public Transform playStick;
    public float speed;
    public float fleeSpeed;
    public float angleWithplayStick;
    public float followDelay;
    public GameObject Goal;
    public List<GameObject> monsters; // 몬스터 관리 리스트 따로 빼서 만들어야함. 게임 매니저, 나중에 Layer 사용
    public float catDetectRadius;
    public float catFleeTime;
    private float catFleeTimer;
    private Vector2 lastFleeDir;

    private bool isFollowingplayStick = false;
    private float fleeRecalTimer;

    public enum CatState
    {
        StartMove,
        ToPlayer,
        Idle,
        Wander,
        Flee,
        GoalReached,
        CatchToPlayer,
        AttackByMonsterWithPlayer,
        StickCheck
    };
    // StartMove : 게임 시작시, 0, 시작하고 나서 Idle로 상태 변화
    // ToPlayer : 플레이어에게 가까이 붙엇을시, 1, Flee 가능
    // Idle : 잠깐 멈추어 두리번 거리느것, 2, Flee 가능
    // Wander ; 목적지를 향해 이동, 3, Flee 가능
    // Flee : 몬스터로 부터 도망칠시, 4
    // GoalReached :  목적지 도달시, 5
    // CatchToPlayer : 플레이어에게 잡혔을때, 6, 예외로 플레이어가 공격 받으면 도망가는 조건으로(조건 추가)
    // AttackByMonsterWithPlayer : 플레이어에게 안겼는데 몬스터 공격을 받았을때, 7
    // StickCheck : 고양이가 플레이어의 낚싯대를 구별하는 텀, 8, Flee 가능

    public CatState currentState = CatState.StartMove;

    Rigidbody2D rb;

    public float startMoveDuration;
    public float checkingIsStickDelay;
    public float fleeRecalcInterval;


    private CatState preState;
    private float startMoveTimer = 0f;
    private float checkingStickDelayTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        EnterStartMove();
    }

    void Update()
    {
        Vector2 playerToplayStick = (playStick.position - player.position).normalized;
        Vector2 playerToCat = (transform.position - player.position).normalized;
        float dot = Vector2.Dot(playerToplayStick, playerToCat);
        
        if(statesOfMonterDetect() && DetectMonsters())
        {
            EnterFlee();
        }

        switch (currentState)
        {
            case CatState.StartMove:
                startMoveTimer -= Time.deltaTime;
                if (startMoveTimer <= 0f)
                {
                    EnterIdle();
                }
                break;

            case CatState.Idle:
                // Idle 상태 작성
                TryStickCheck(dot);
                break;

            case CatState.Wander:
                // 플레이어 방향과 반대이면서 목표물을 향해 이동하는 코드(나중에 맵에 따른 변경점 추가)
                TryStickCheck(dot);
                break;

            case CatState.StickCheck:
                if (dot >= angleWithplayStick)
                {
                    checkingStickDelayTimer -= Time.deltaTime;
                    if (checkingStickDelayTimer <= 0f)
                    {
                        isFollowingplayStick = true;
                        currentState = CatState.ToPlayer;
                    }
                }
                else
                {
                    currentState = preState;
                }
                break;

            case CatState.ToPlayer:
                if (dot < angleWithplayStick)
                {
                    isFollowingplayStick = false;
                    currentState = CatState.Wander;
                }
                break;

            case CatState.Flee:
                catFleeTimer -= Time.deltaTime;
                if (DetectMonsters())
                {
                    // 몬스터가 감지되면 최소 유지 시간 보장 ???
                    catFleeTimer = Mathf.Max(catFleeTimer, 0.15f);
                }
                if (catFleeTimer <= 0f && !DetectMonsters())
                {
                    ExitFlee();
                }
                break;

            case CatState.CatchToPlayer:
                // 플레이어한테 잡히면 플레이어 품속을 따라다니는 코드
                break;

            case CatState.AttackByMonsterWithPlayer:
                // 플레이어한테 잡힌 상태로 몬스터 공격을 받았을때 움직임 코드
                break;

            case CatState.GoalReached:
                // 목표 달성시 화면 전화 및 목표 달성 창 띄우는거
                break;
        }

        Debug.Log(currentState);
    }

    void FixedUpdate()
    {
        switch (currentState)
        {
            case CatState.StartMove:
                rb.MovePosition(rb.position + Vector2.up * speed * Time.fixedDeltaTime);
                break;

            case CatState.Idle:
                break;

            case CatState.Wander:
                break;

            case CatState.StickCheck:
                break;

            case CatState.ToPlayer:
                if (isFollowingplayStick)
                {
                    Vector2 next = Vector2.MoveTowards(rb.position, playStick.position, speed * Time.fixedDeltaTime);
                    rb.MovePosition(next);
                }
                break;

            case CatState.Flee:
                fleeRecalTimer -= Time.fixedDeltaTime;
                if (fleeRecalTimer <= 0f)
                {
                    lastFleeDir = FleeDir();
                    fleeRecalTimer = fleeRecalcInterval;
                }
                rb.MovePosition(rb.position + lastFleeDir * fleeSpeed * Time.fixedDeltaTime);
                break;

        }
    }

    private void EnterStartMove()
    {
        currentState = CatState.StartMove;
        startMoveTimer = startMoveDuration;

        isFollowingplayStick = false;
    }

    private void EnterIdle()
    {
        currentState = CatState.Idle;
    }

    private bool TryStickCheck(float dot)
    {
        if (isFollowingplayStick)
        {
            return false;
        }

        if (currentState != CatState.Idle && currentState != CatState.Wander)
        {
            return false;
        }

        if (dot >= angleWithplayStick)
        {
            preState = currentState;
            currentState = CatState.StickCheck;
            checkingStickDelayTimer = checkingIsStickDelay;
            return true;
        }
        return false;
    }

    private bool statesOfMonterDetect() //
    {
        return currentState == CatState.Idle
            || currentState == CatState.Wander
            || currentState == CatState.StickCheck
            || currentState == CatState.ToPlayer;
    }

    private void EnterFlee()
    {
        preState = CatState.Wander; // 임시 Wander, Idle, Wander 둘중 하나 고르게 수정
        currentState = CatState.Flee;
        catFleeTimer = catFleeTime;
        fleeRecalTimer = 0f;
        isFollowingplayStick = false;
    }

    private void ExitFlee()
    {
        currentState = preState;
    }

    private bool DetectMonsters() // 몬스터를 감지했는지 아닌지를 true / false 로
    {
        foreach (GameObject monster in monsters)
        {
            if (monster == null)
            {
                continue;
            }
            Vector2 monsterPos = monster.transform.position;
            float distance = Vector2.Distance(rb.position, monsterPos);
            if (distance < catDetectRadius)
            {
                return true;
            }
        }
        return false;
    }

    private Vector2 FleeDir()
    {
        Vector2 fleeDir = Vector2.zero;

        // 리스트 비어 있을때
        if (monsters == null || monsters.Count == 0)
        {
            return lastFleeDir != Vector2.zero ? lastFleeDir : (Vector2)UnityEngine.Random.insideUnitCircle.normalized;
        }

        foreach (GameObject monster in monsters)
        {
            if(monster == null)
            {
                continue;
            }
            Vector2 monsterPos = monster.transform.position;
            float distance = Vector2.Distance(rb.position, monsterPos); 
            if (distance < catDetectRadius)
            {
                Vector2 away = (rb.position - monsterPos).normalized;
                // 너무 가까우면 가중치(근처일수록 강하게) ???
                float w = Mathf.Clamp01(1f - distance / catDetectRadius);
                fleeDir += away * (0.5f + w);
            }
        }

        // 완전 0이면 마지막 방향 유지(정지 방지)
        if (fleeDir == Vector2.zero)
            return lastFleeDir != Vector2.zero ? lastFleeDir : (Vector2)UnityEngine.Random.insideUnitCircle.normalized;

        lastFleeDir = fleeDir.normalized;
        return lastFleeDir;

    }
}


