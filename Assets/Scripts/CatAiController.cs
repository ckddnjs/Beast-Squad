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
    public List<GameObject> monsters; // ���� ���� ����Ʈ ���� ���� ��������. ���� �Ŵ���, ���߿� Layer ���
    public float catDetectRadius;
    public float catFleeTime;
    private float catFleeTimer;
    private Vector2 lastFleeDir;

    private Coroutine followCoroutine = null;
    private bool isFollowingplayStick = false;
    private float fleeRecalTimer;

    public enum CatState {
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
    // StartMove : ���� ���۽�, 0, �����ϰ� ���� Idle�� ���� ��ȭ
    // ToPlayer : �÷��̾�� ������ �پ�����, 1, Flee ����
    // Idle : ��� ���߾� �θ��� �Ÿ�����, 2, Flee ����
    // Wander ; �÷��̾�������� �������� �̵�, 3, Flee ����
    // Flee : ���ͷ� ���� ����ĥ��, 4
    // GoalReached :  ������ ���޽�, 5
    // CatchToPlayer : �÷��̾�� ��������, 6, ���ܷ� �÷��̾ ���� ������ �������� ��������(���� �߰�)
    // AttackByMonsterWithPlayer : �÷��̾�� �Ȱ�µ� ���� ������ �޾�����, 7
    // StickCheck : ����̰� �÷��̾��� ���˴븦 �����ϴ� ��, 8, Flee ����

    public CatState currentState = CatState.StartMove;

    Rigidbody2D rb;

    public float startMoveDuration;
    public float checkingIsStickDelay;


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

        switch (currentState)
        {
            case CatState.StartMove:
                startMoveTimer -= Time.deltaTime; 
                if(startMoveTimer <= 0f)
                {
                    EnterIdle();
                }
                break;
            
            case CatState.Idle:
                // Idle ���� �ۼ�
                TryStickCheck(dot);
                break;

            case CatState.Wander:
                // Wander ���� �ۼ�
                TryStickCheck(dot);
                break;

            case CatState.StickCheck:
                if(dot >= angleWithplayStick)
                {
                    checkingStickDelayTimer -= Time.deltaTime;
                    if(checkingStickDelayTimer <= 0f)
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
                if(dot < angleWithplayStick)
                {
                    isFollowingplayStick = false;
                    currentState = CatState.Wander;
                }
                break;

        }

        foreach(GameObject monster in monsters)
        {
            float distance = Vector2.Distance(rb.position, monster.transform.position);
            if(distance < catDetectRadius)
            {
                currentState = CatState.Flee;
                catFleeTimer = catFleeTime; 
            }
        }      

        if(currentState == CatState.Flee)
        {
            catFleeTimer -= Time.deltaTime;
            if(catFleeTimer <= 0)
            {
                currentState = CatState.Wander;
            }
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
                if(isFollowingplayStick)
                {
                    Vector2 next = Vector2.MoveTowards(rb.position, playStick.position, speed * Time.fixedDeltaTime);
                    rb.MovePosition(next);
                }
                break;

            case CatState.Flee:
                // Flee ���� �ۼ�
                break;

        }

        if(currentState == CatState.Flee)
        {
            Vector2 fleeDir = CatFleeFromMonsters();

            rb.MovePosition(rb.position + fleeDir * fleeSpeed * Time.fixedDeltaTime);
        }
    }

    private Vector2 CatFleeFromMonsters()
    {
        Vector2 fleeDir = Vector2.zero;
        
        foreach(GameObject monster in monsters)
        {
            if (monster == null) continue;

            Vector2 monsterPosition = monster.transform.position;
            float distance = Vector2.Distance(rb.position, monsterPosition);

            if(distance < catDetectRadius)
            {
                fleeDir += (rb.position - monsterPosition).normalized;
                // ���� A - B �� B -> A ���� ���� ����, �� ���� �ݴ� ����
            }
        }
        return fleeDir.normalized; // ���� ���������� ��� �������� �̵�
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
        if(isFollowingplayStick)
        {
            return false;
        }
        
        if(currentState != CatState.Idle && currentState != CatState.Wander)
        {
            return false;
        }

        if(dot >= angleWithplayStick)
        {
            preState = currentState;
            currentState = CatState.StickCheck;
            checkingStickDelayTimer = checkingIsStickDelay;
            return true;
        }
        return false;
    }

    private bool IsVulnerableState()
    {
        return currentState == CatState.Idle
            || currentState == CatState.Wander
            || currentState == CatState.StickCheck
            || currentState == CatState.ToPlayer;
    }

    private void EnterFlee()
    {
        preState = CatState.Wander; // �ӽ� Wander, Idle, Wander ���� �ϳ� ���� ����
        currentState = CatState.Flee;
        catFleeTimer = catFleeTime;
        fleeRecalTimer = 0f;
        isFollowingplayStick = false;
    }

    private void ExitFlee()
    {
        currentState = preState;
    }

    private bool DetectMonsters()
    {
        foreach(GameObject monster in monsters)
        {
            if(monster == null)
            {
                continue;
            }
            float distance = Vector2.Distance(rb.position, monster.transform.position); // ����
            if(distance < catDetectRadius)
            {
                return true;
            }
        }
        return false;
    }

    private Vector2 FleeDir()
    {
        Vector2 fleeDir = Vector2.zero;
        foreach(GameObject monster in monsters)
        {
            if(monster == null)
            {
                continue;
            }
            Vector2 monsterPos = monster.transform.position;
            float distance = Vector2.Distance(rb.position, monsterPos); // ����
            if(distance < catDetectRadius)
            {
                // ���� �ݴ�������� ����
                Vector2 away = (rb.position - (Vector2)monsterPos).normalized;
                // �ʹ� ������ ����ġ(��ó�ϼ��� ���ϰ�)
                float w = Mathf.Clamp01(1f - distance / catDetectRadius);
                fleeDir += away * (0.5f + w);
            }

            // ���� 0�̸� ������ ���� ����(���� ����)
            if (fleeDir == Vector2.zero)
                return lastFleeDir != Vector2.zero ? lastFleeDir : (Vector2)UnityEngine.Random.insideUnitCircle.normalized;

            lastFleeDir = fleeDir.normalized;
            return lastFleeDir;
        }

        //�ӽ� �߰�
        return new Vector2();
    }
}


/*
void Update()
{
    Vector2 playerToplayStick = (playStick.position - player.position).normalized;
    Vector2 playerToCat = (transform.position - player.position).normalized;
    float dot = Vector2.Dot(playerToplayStick, playerToCat);
if (IsVulnerableState() && DetectThreats())
    {
        EnterFlee();
    }

    switch (currentState)
    {
        case CatState.StartMove:
            startMoveTimer -= Time.deltaTime;
            if (startMoveTimer <= 0f) EnterIdle();
            break;

        case CatState.Idle:
            TryStickCheck(dot);
            break;

        case CatState.Wander:
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
            // Ÿ�̸� ���� �� ���� ������ Update����
            catFleeTimer -= Time.deltaTime;

            // ���� ������ �����ִٸ� Ÿ�̸Ӹ� ��¦ ����(���� ����, ����)
            if (DetectThreats())
                catFleeTimer = Mathf.Max(catFleeTimer, 0.15f);

            if (catFleeTimer <= 0f && !DetectThreats())
            {
                ExitFlee();
            }
            break;
    }

    // (�����)
    // Debug.Log(currentState);
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
            // �ֱ������� ���� ����(�ʹ� ���� �ٲ�� ����)
            fleeRecalcTimer -= Time.fixedDeltaTime;
            if (fleeRecalcTimer <= 0f)
            {
                lastFleeDir = ComputeFleeDir();
                fleeRecalcTimer = fleeRecalcInterval;
            }
            rb.MovePosition(rb.position + lastFleeDir * fleeSpeed * Time.fixedDeltaTime);
            break;
    }
}

*/
