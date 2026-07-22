using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    #region Variables: Movement

    [SerializeField] private float speed = 5f;   // horizontal movement speed in units/sec
    private Vector2 _input;                       // latest move input from the player (x = strafe, y = forward/back)
    private CharacterController _controller;      // Unity component used to move the character with collision
    private Animator _animator;                   // animator driving the character's animation state machine
    private Transform _cameraTransform;           // cached main camera transform, used for camera-relative movement

    #endregion

    #region Variables: Gravity

    private float _gravity = -9.81f;              // base downward acceleration (Earth gravity)
    [SerializeField] private float gravityMultiplier = 3.0f; // scales base gravity for snappier, less floaty jumps
    private float _velocity;                      // current vertical (Y) velocity, affected by gravity and jumps
    private bool _isGrounded;                     // whether the character is currently touching the ground

    #endregion

    #region Variables: Jumping

    [SerializeField] private float jumpPower = 10f; // upward velocity applied when jumping

    #endregion

    // True when the player is supplying non-negligible movement input
    public bool IsMoving => _input.sqrMagnitude > 0.001f;

    // Awake runs once before the first frame; cache component references here
    void Awake()
    {
        _cameraTransform = Camera.main.transform;          // grab the tagged Main Camera
        _controller = GetComponent<CharacterController>(); // movement controller on this GameObject
        _animator = GetComponentInChildren<Animator>();    // animator lives on a child model
    }

    // Per-frame update: order matters — ground check, then move, gravity, finally animation
    void Update()
    {
        _isGrounded = IsGrounded();
        ApplyMovement();
        ApplyGravity();
        ApplyAnimation();
    }

    // Called by the Input System when the Move action fires (Vector2 stick/WASD)
    void OnMove(InputValue value)
    {
        _input = value.Get<Vector2>();
    }

    // Called by the Input System when the Jump action fires
    void OnJump(InputValue value)
    {
        if (_isGrounded) {              // only allow jumping while grounded to prevent double-jumps
            _velocity = jumpPower;      // set an immediate upward velocity
            _animator.SetTrigger("Jump"); // tell the animator to play the jump transition
        }
    }

    // Move the character horizontally based on input, and vertically based on current velocity
    void ApplyMovement()
    {

        // Use the character's own facing for input direction so movement is camera-relative
        Vector3 forward = transform.forward;
        forward.y = 0f;                // flatten so movement stays on the horizontal plane
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;                   // flatten the right vector as well
        right.Normalize();

        // Combine strafe (x) and forward (y) input into a single horizontal direction
        Vector3 horizontalMovement = (right * _input.x + forward * _input.y).normalized;
        // Add the vertical velocity so gravity/jumping are applied in the same Move call
        Vector3 movement = horizontalMovement * speed + Vector3.up * _velocity;

        // Time.deltaTime makes movement frame-rate independent
        _controller.Move(movement * Time.deltaTime);

    }

    // Accumulate downward velocity to simulate gravity (with a ground-stick trick)
    void ApplyGravity()
    {
        if (_isGrounded && _velocity < 0f)
        {
            _velocity = -2f;  // common trick to keep the character firmly grounded
            return;            // already on the ground, no need to accumulate gravity
        }

        // Integrate gravity over time, scaled by the multiplier
        _velocity += _gravity * gravityMultiplier * Time.deltaTime;

    }

    // Feed gameplay state into the animator so animations match movement
    void ApplyAnimation()
    {
        float forwardSpeed = 0f;
        float rightSpeed = 0f;
        if (IsMoving)               // only set blend values when there is actual input
        {
            forwardSpeed = _input.y; // forward/back blend
            rightSpeed = _input.x;   // strafe blend
        }

        _animator.SetBool("IsGrounded", _isGrounded);    // grounded flag for transitions
        _animator.SetFloat("ForwardSpeed", forwardSpeed); // drives forward motion blend tree
        _animator.SetFloat("RightSpeed", rightSpeed);     // drives strafe motion blend tree
        _animator.SetFloat("VerticalSpeed", _velocity);   // drives falling/jumping blend tree
    }

    // Checks for ground by sphere casting downward, ignoring the character's own layer
    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f; // start the cast slightly above the feet
        float radius = 0.3f;        // radius of the sphere used for the cast
        float distance = 1.1f;      // how far to cast downward
        int layerMask = ~(1 << gameObject.layer); // ignore the character's own layer to avoid self-hit

        return Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out _,
            distance,
            layerMask
        );
    }

    // Draws the ground-check sphere in the editor so the cast can be visualized
    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * 1.0f;
        float radius = 0.3f;
        float distance = 1.1f;

        Gizmos.color = _isGrounded ? Color.green : Color.red; // green when grounded, red when not
        Gizmos.DrawWireSphere(origin + Vector3.down * distance, radius); // draw sphere at the cast endpoint
    }
}
