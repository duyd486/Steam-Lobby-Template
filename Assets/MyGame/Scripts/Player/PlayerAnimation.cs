using Unity.Netcode;
using UnityEngine;

public class PlayerAnimation : NetworkBehaviour
{
    private Animator animator;

    private Vector3 lastPosition;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        bool isWalking;

        if (IsOwner)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            isWalking = Mathf.Abs(h) > 0.01f || Mathf.Abs(v) > 0.01f;
        }
        else
        {
            float speed =
                (transform.position - lastPosition).magnitude / Time.deltaTime;

            isWalking = speed > 0.15f;
        }

        animator.SetBool("IsWalking", isWalking);

        lastPosition = transform.position;
    }
}