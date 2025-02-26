using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public Camera playerCam;
    public float walkSpeed = 6f;
    public float dashSpeed = 18f;
    public float leapForce = 7f;
    public float doubleJumpForce = 6f;
    public float fallGravity = 10f;
    public float mouseSensitivity = 2f;
    public float maxTiltAngle = 45f;
    public float fullHeight = 2f;
    public float shrinkHeight = 1f;
    public float crawlSpeed = 3f;
    public float dashTime = 0.3f;

    private CharacterController playerBody;
    private Vector3 motionVector;
    private float cameraPitch = 0f;
    private bool isMovable = true;
    private bool canDoubleJump = false;
    private bool isDashing = false;
    private float lastDashPressTime = 0f;
    private float dashCooldown = 0.5f;

    void Awake()
    {
        playerBody = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        ProcessMovement();
        ProcessCamera();
        ProcessCrouch();
    }

    private void ProcessMovement()
    {
        if (!isMovable) return;

        float moveSide = Input.GetAxis("Horizontal");
        float moveForward = Input.GetAxis("Vertical");
        bool dashPressed = Input.GetKeyDown(KeyCode.W);

        if (dashPressed)
        {
            if (Time.time - lastDashPressTime < dashCooldown)
            {
                StartCoroutine(Dash());
            }
            lastDashPressTime = Time.time;
        }

        float movementSpeed = isDashing ? dashSpeed : walkSpeed;
        Vector3 movement = transform.right * moveSide + transform.forward * moveForward;
        playerBody.Move(movement * movementSpeed * Time.deltaTime);

        if (playerBody.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))
            {
                motionVector.y = leapForce;
                canDoubleJump = true;
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && canDoubleJump)
            {
                motionVector.y = doubleJumpForce;
                canDoubleJump = false;
            }
            motionVector.y -= fallGravity * Time.deltaTime;
        }

        playerBody.Move(motionVector * Time.deltaTime);
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
    }

    private void ProcessCamera()
    {
        if (!isMovable) return;

        float lookX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float lookY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraPitch -= lookY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxTiltAngle, maxTiltAngle);

        playerCam.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * lookX);
    }

    private void ProcessCrouch()
    {
        if (Input.GetKey(KeyCode.R))
        {
            playerBody.height = shrinkHeight;
            walkSpeed = crawlSpeed;
            dashSpeed = crawlSpeed;
        }
        else
        {
            playerBody.height = fullHeight;
            walkSpeed = 6f;
            dashSpeed = 18f;
        }
    }
}
