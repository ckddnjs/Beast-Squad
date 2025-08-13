
using UnityEngine;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    [SerializeField]
    private float maxHealth = 100f;

    private float currentHealth;


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            currentHealth = maxHealth;
        }
    }

    // 서버에서 호출하여 대미지 입기
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if (!IsServer) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth <= 0)
        {
            // 죽음 처리 직접 호출
            OnDeath();
        }
    }

    // 서버에서 호출하여 회복하기
    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(float amount)
    {
        if (!IsServer) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void OnDeath()
    {

        // 예: 오브젝트 삭제
        NetworkObject.Despawn();
    }
}