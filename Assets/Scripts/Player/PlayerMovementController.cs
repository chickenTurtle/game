using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementController : NetworkBehaviour
{
    public GameObject PlayerModel;
    private Rigidbody rb;

    [Header("Movement")]
    public float moveSpeed;
    public Transform orientation;
    private float horizontalInput, verticalInput;
    private Vector3 moveDirection;
    public float groundDrag;

    public float jumpForce;
    public float jumpCoolDown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Keybind")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    // Start is called before the first frame update
    private void Start()
    {
        PlayerModel.SetActive(false);
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
        {
            if (PlayerModel.activeSelf == false)
            {
                transform.position = new Vector3(0, 3, 0);
                PlayerModel.SetActive(true);
            }
            if (authority)
            {
                grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
                if (Input.GetKey(jumpKey) && readyToJump && grounded)
                {
                    readyToJump = false;
                    Jump();
                    Invoke(nameof(ResetJump), jumpCoolDown);
                }

                if (grounded)
                    rb.drag = groundDrag;
                else
                    rb.drag = 0;

                horizontalInput = Input.GetAxis("Horizontal");
                verticalInput = Input.GetAxis("Vertical");
            }
        }
    }

    private void FixedUpdate()
    {
        if (authority)
        {
            MovePlayer();
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

}
