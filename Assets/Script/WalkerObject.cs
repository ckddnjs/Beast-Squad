using UnityEngine;

public class WalkerObject
{
    public Vector2 Position;
    public Vector2 Direction;
    // ���� ��ȯ���� ���� ��
    public float ChangeToChange;

    public WalkerObject(Vector2 pos, Vector2 dir, float changeToChange)
    {
        Position = pos;
        Direction = dir;
        ChangeToChange = changeToChange;
    }
}
