using UnityEngine;

public class WalkerObject
{
    public Vector2 Position;
    public Vector2 Direction;
    // 방향 전환까지 남은 값
    public float ChangeToChange;

    public WalkerObject(Vector2 pos, Vector2 dir, float changeToChange)
    {
        Position = pos;
        Direction = dir;
        ChangeToChange = changeToChange;
    }
}
