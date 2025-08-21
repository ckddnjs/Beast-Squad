using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;


public class AnimationSync : NetworkBehaviour
{

    public SpriteRenderer spriter;   
    public Animator animator;

    private NetworkVariable<Vector2> netMove = new NetworkVariable<Vector2>(
        Vector2.zero,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public override void OnNetworkSpawn()
    {
        netMove.OnValueChanged += OnNetMoveChanged;

        ApplyAnimation(netMove.Value);
    }

    public override void OnNetworkDespawn()
    {
        netMove.OnValueChanged -= OnNetMoveChanged;
    }

    private void OnNetMoveChanged(Vector2 oldVal, Vector2 newVal)
    {
        ApplyAnimation(newVal);
    }

    private void Update()
    {
        if (!IsOwner) return;

        Vector2 inputVec = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        if (netMove.Value != inputVec)
        {
            netMove.Value = inputVec; 
        }

        ApplyAnimation(inputVec);
    }

    private void ApplyAnimation(Vector2 inputVec)
    {
        animator.SetFloat("speed", inputVec.magnitude);

        if (inputVec.x != 0)
        {
            spriter.flipX = inputVec.x < 0 ? true : false;
        }
    }
}
