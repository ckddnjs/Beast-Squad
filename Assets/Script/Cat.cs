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
                followCoroutine = StartCoroutine(DelayAndFollow());
            }
        }
        else
        {
             if(isFollowingplayStick)
            {
                isFollowingplayStick = false;
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
