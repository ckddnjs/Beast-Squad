using UnityEngine;
[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("�� ���� �̸�")]
    public string monsterName;

    [Header("����")]
    public int maxHp;
    public int attackPower;
    public float moveSpeed;
    public float detectionRange;
}
