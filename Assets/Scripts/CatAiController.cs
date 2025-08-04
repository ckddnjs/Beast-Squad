using System;
using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Random;

public class CatAiController : MonoBehaviour
{
    public enum CatState {Starter, Idle, Wander, ToPlayer, Flee, GoalReached };
    // Starter : ���� ���۽�
    // Idle : ��� ���߾� �θ��� �Ÿ�����
    // Wander ; �÷��̾�������� �������� �̵�
    // ToPlayer : �÷��̾�� ������ �پ�����
    // Flee : ���ͷ� ���� ����ĥ��
    // GoalReached :  ������ ���޽�

    public CatState currentState;

    // Ÿ�� 2��
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
