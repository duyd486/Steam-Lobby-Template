using Unity.Netcode;
using UnityEngine;

public class PlayerLocomotion : NetworkBehaviour
{
    [SerializeField] private float walkSpeed = 5f;

    private void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 inputVector = new Vector2(h, v);
        Vector3 inputDir = new Vector3(inputVector.x, 0f, inputVector.y);

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        Vector3 moveDir = (cameraForward * inputDir.z + cameraRight * inputDir.x).normalized;

        transform.position += moveDir * walkSpeed * Time.deltaTime;

        Quaternion targetRot = Quaternion.LookRotation(cameraForward);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            10f * Time.deltaTime
        );
    }
}
