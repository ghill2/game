using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[DefaultExecutionOrder(50)]
public class CameraControl : MonoBehaviour, AxisState.IInputAxisProvider
{
    [SerializeField] private float rotationSpeed = 500f;

    private CinemachineCamera _cinemachineCamera;
    private Transform _character;
    private Transform _brainCamera;

    void Awake()
    {
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        var brain = GetComponentInChildren<CinemachineBrain>();
        if (brain != null) {
            _brainCamera = brain.transform;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _character = player.transform;
            if (_cinemachineCamera != null) {
                _cinemachineCamera.Follow = _character;
            }
        }
    }

    public float GetAxisValue(int axis)
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.rightButton.isPressed) {
            return 0f;
        }

        Vector2 delta = mouse.delta.ReadValue();
        if (axis == 0) {
            return delta.x * rotationSpeed * Time.deltaTime;
        }

        if (axis == 1) {
            return -delta.y * rotationSpeed * Time.deltaTime;
        }
        return 0f;
    }

    void LateUpdate()
    {
        if (_character == null || _brainCamera == null) return;
        Vector3 dir = _brainCamera.forward;
        dir.y = 0f;
        if (dir.sqrMagnitude <= 0.001f) return;
        _character.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
    }
}
