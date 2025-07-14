using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHP;
    public int currentHP { get; private set; } // �ܺ� : �� �б⸸, ���� : ���� �޼ҵ忡����

    public event Action<int, int> OnHealthChanged; // event�� += -= �� ����
    public event Action OnDied;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(int dmg)
    {
        currentHP = Mathf.Max(currentHP - dmg, 0); // 2���� �Ű������� ū�� ��ȯ
        OnHealthChanged?.Invoke(currentHP, maxHP);
        if (currentHP == 0) OnDied?.Invoke();
    }

    public void Heal(int amount)
    {
        currentHP = Math.Min(currentHP + amount, maxHP); // 2���� �Ű������� ������ ��ȯ
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
