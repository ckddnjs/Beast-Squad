using UnityEngine;

public class Cat : MonoBehaviour
{
    public Transform player;
    public Transform playStick;
    public float speed;
    public float angleWithplayStick;
    public float followDelay;

    private Coroutine followCoroutine = null;
    private bool isFollowingplayStick = false;

    void Start()
    {
        
    }

    void Update()
    {
        Vector2 playerToplayStick = (playStick.position - player.position).normalized;
        Vector2 playerToCat = (transform.position - player.position).normalized;

        float dot = Vector2.Dot(playerToplayStick, playerToCat);

        // 내적으로 angleWithplayStick 비교해서 일직선 범위 지정
        if (dot >= angleWithplayStick)
        {
            if(!isFollowingplayStick && followCoroutine == null)
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

    System.Collections.IEnumerator DelayAndFollow()
    {
        yield return new WaitForSeconds(followDelay);
        isFollowingplayStick = true;
        
        followCoroutine = null;
    }
}
