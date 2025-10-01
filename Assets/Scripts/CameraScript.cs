using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float sprintSpeedMultiplier = 2f;
    [SerializeField] private float lookSensitivity;
    private float lookSenseMem;

    [SerializeField] private float rotationX;
    [SerializeField] private float rotationY;

    // Start is called before the first frame update
    void Start()
    {
        lookSenseMem = lookSensitivity;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Camera Rotation (Mouse Look)
        rotationX += Input.GetAxis("Mouse X") * lookSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * lookSensitivity;
        rotationY = Mathf.Clamp(rotationY, -90f, 90f); // Clamp vertical rotation

        transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);

        // Camera Movement (WASD, Control/Space)
        float currentMovementSpeed = movementSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentMovementSpeed *= sprintSpeedMultiplier;
        }

        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveDirection -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            moveDirection -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += transform.right;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            moveDirection += transform.up;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            moveDirection -= transform.up;
        }

        transform.position += moveDirection.normalized * currentMovementSpeed * Time.deltaTime;

        // Unlock cursor with Escape key (optional)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (lookSensitivity != 0)
            {
                lookSensitivity = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                lookSensitivity = lookSenseMem;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
