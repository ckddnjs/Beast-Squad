using UnityEngine;
[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("각 몬스터 이름")]
    public string monsterName;

    [Header("스탯")]
    public int maxHp;
    public int attackPower;
    public float moveSpeed;
    public float detectionRange;
}
