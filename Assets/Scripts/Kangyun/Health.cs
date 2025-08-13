
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

    // �������� ȣ���Ͽ� ����� �Ա�
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        if (!IsServer) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);

        if (currentHealth <= 0)
        {
            // ���� ó�� ���� ȣ��
            OnDeath();
        }
    }

    // �������� ȣ���Ͽ� ȸ���ϱ�
    [ServerRpc(RequireOwnership = false)]
    public void HealServerRpc(float amount)
    {
        if (!IsServer) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    private void OnDeath()
    {

        // ��: ������Ʈ ����
        NetworkObject.Despawn();
    }
}