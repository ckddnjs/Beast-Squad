using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHP;
    public int currentHP { get; private set; } // 외부 : 값 읽기만, 변경 : 내부 메소드에서만

    public event Action<int, int> OnHealthChanged; // event로 += -= 만 가능
    public event Action OnDied;

    void Awake() => currentHP = maxHP;

    public void TakeDamage(int dmg)
    {
        currentHP = Mathf.Max(currentHP - dmg, 0); // 2가지 매개변수중 큰값 반환
        OnHealthChanged?.Invoke(currentHP, maxHP);
        if (currentHP == 0) OnDied?.Invoke();
    }

    public void Heal(int amount)
    {
        currentHP = Math.Min(currentHP + amount, maxHP); // 2가지 매개변수중 작은값 반환
        OnHealthChanged?.Invoke(currentHP, maxHP);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
