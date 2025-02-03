using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool isWalking;
    private UnityEngine.Vector3 lastInteractDir;

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        UnityEngine.Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        UnityEngine.Vector3 moveDir = new UnityEngine.Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != UnityEngine.Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out ClearCounter clearCounter)) {
                // has ClearCounter
                clearCounter.Interact();
            }
        }
    }

    private void HandleMovement() {
        UnityEngine.Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        UnityEngine.Vector3 moveDir = new UnityEngine.Vector3(inputVector.x, 0f, inputVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + UnityEngine.Vector3.up * playerHeight, playerRadius,
            moveDir, moveDistance);
        
        if (!canMove) {
            // Cannot move towards moveDir
            // Attampt only X movement 
            UnityEngine.Vector3 moveDirX = new UnityEngine.Vector3(moveDir.x, 0f, 0f).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + UnityEngine.Vector3.up * playerHeight, playerRadius,
                moveDirX, moveDistance);

            if (canMove) {
                // Can move only on the X
                moveDir = moveDirX;
            } else {
                // Cannot move only on the X
                // Attempt only Z movement
                UnityEngine.Vector3 moveDirZ = new UnityEngine.Vector3(0f, 0f, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + UnityEngine.Vector3.up * playerHeight, playerRadius,
                    moveDirZ, moveDistance);

                if (canMove) {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                } else {
                    // Cannot move on X and Z
                    // Cannot move at all
                    moveDir = UnityEngine.Vector3.zero;
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        } else {
            Debug.Log("Collision Detected: Cannot move!");
        }

        isWalking = moveDir != UnityEngine.Vector3.zero;

        float rotationSpeed = 10f;
        transform.forward = UnityEngine.Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotationSpeed);
    }

}
