using System;
using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Random;

public class CatAiController : MonoBehaviour
{
    public enum CatState {Starter, Idle, Wander, ToPlayer, Flee, GoalReached };
    // Starter : 게임 시작시
    // Idle : 잠깐 멈추어 두리번 거리느것
    // Wander ; 플레이어방향제외 무작위로 이동
    // ToPlayer : 플레이어에게 가까이 붙엇을시
    // Flee : 몬스터로 부터 도망칠시
    // GoalReached :  목적지 도달시

    public CatState currentState;

    // 타겟 2개
    public float moveSpeed = 2.0f;

    public Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(StartRoutine());
    }

   IEnumerator StartRoutine()
    {
        while(true)
        {
            switch (currentState)
            {
                case CatState.Starter:
                    yield return Starter();
                    break;
                case CatState.Idle:
                    yield return Idle();
                    break;
                case CatState.Wander:
                    yield return Wander();
                    break;
                case CatState.ToPlayer:
                    yield return ToPlayer();
                    break;
                case CatState.Flee:
                    yield return Flee();
                    break;
                case CatState.GoalReached:
                    yield return GoalReached();
                    break;
            }
            yield return null;
        }
    }

    IEnumerator Idle()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2.0f));
        currentState = CatState.Idle;
    }

    IEnumerator Wander90
    {
        
    }
    
}
