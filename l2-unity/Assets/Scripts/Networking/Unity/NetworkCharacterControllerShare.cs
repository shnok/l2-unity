using System;
using UnityEngine;

[RequireComponent(typeof(NetworkTransformShare)), RequireComponent(typeof(CharacterController))]
public class NetworkCharacterControllerShare : MonoBehaviour
{
    private CharacterController _characterController;
    [SerializeField] private int _sharingLoopDelayMs = 100;

    [SerializeField] private Vector3 _lastDirection;
    [SerializeField] private float _lastDirectionAngle;

    [SerializeField] private long _lastSharingTimestamp = 0;
    [SerializeField] private int _heading;

    public int Heading { get { return _heading; } set { _heading = value; } }


    [SerializeField] private int _sharingPositionDelayMs = 500;
    [SerializeField] private long _lastSharingPosition = 0;

    public bool _isSharedJumping = false;


    private static NetworkCharacterControllerShare _instance;
    public static NetworkCharacterControllerShare Instance { get { return _instance; } }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        _lastDirectionAngle = -99999;
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        if (_characterController == null || World.Instance.OfflineMode)
        {
            this.enabled = false;
            return;
        }

        _lastDirection = new Vector3(-1, -1, -1);
    }

    private void FixedUpdate()
    {
        Vector3 newDirection = Vector3.zero;
        if (PlayerController.Instance.IsMoving())
        {
            newDirection = PlayerController.Instance.MoveDirection.normalized;
        }
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (ShouldShareMoveDirection(newDirection, now))
        {
            _lastSharingTimestamp = now;

            if (VectorUtils.IsVectorZero2D(newDirection))
            {
                NetworkTransformShare.Instance.SharePosition();
            }

            float verticalVelocity = 0.0f;

            if (ShouldShareJumping())
            {
                //TODO: Maybe we need validate if the player is falling or not
                verticalVelocity = PlayerController.Instance._verticalVelocity;
                _isSharedJumping = true;
            }


            ShareMoveDirection(newDirection, verticalVelocity, transform.position);
        }
    }

    private bool ShouldShareJumping()
    {
        if (!PlayerController.Instance.IsJumping() && _isSharedJumping)
        {
            _isSharedJumping = false;
        }

        if (PlayerController.Instance.IsJumping() && !_isSharedJumping)
        {
            Debug.LogWarning("Sharing move direction: Should share jump");
            return true;
        }
        return false;
    }

    private bool ShouldShareMoveDirection(Vector3 newDirection, long timestamp)
    {
        if (ShouldShareJumping())
        {
            return true;
        }

        // Debug.LogWarning($"Check: Last: {_lastDirection} New: {newDirection}");
        if (!VectorUtils.IsVectorZero2D(_lastDirection) && VectorUtils.IsVectorZero2D(newDirection))
        {
            // Debug.LogWarning("Player just stopped");
            // player just stopped
            return true;
        }
        if (VectorUtils.IsVectorZero2D(newDirection) && VectorUtils.IsVectorZero2D(_lastDirection))
        {
            // player just stopped and is not moving
            return false;
        }
        if (!VectorUtils.IsVectorZero2D(newDirection) && VectorUtils.IsVectorZero2D(_lastDirection))
        {
            // player just moved
            // Debug.LogWarning("Player just moved");
            return true;
        }

        // Basic loop delay
        if (timestamp - _lastSharingTimestamp >= _sharingLoopDelayMs)
        {
            return true;
        }

        return false;
    }

    public void ForceShareMoveDirection()
    {
        ShareMoveDirection(PlayerController.Instance.MoveDirection.normalized, true, 0.0f, Vector3.zero);
    }

    public void ShareMoveDirection(Vector3 moveDirection, float verticalVelocity, Vector3 position)
    {
        ShareMoveDirection(moveDirection, false, verticalVelocity, position);
    }

    public void ShareMoveDirection(Vector3 moveDirection, bool isForced, float verticalVelocity, Vector3 position)
    {
        if (position == Vector3.zero)
        {
            position = transform.position;
        }


        float directionAngle = VectorUtils.CalculateMoveDirectionAngle(moveDirection.x, moveDirection.z);

        _lastDirectionAngle = directionAngle;

        if (!VectorUtils.IsVectorZero2D(moveDirection))
        {
            Heading = CalculateHeading(directionAngle);
        }

        _lastDirection = moveDirection;

        // Debug.LogWarning("Sharing: " + moveDirection);
        GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(moveDirection, Heading, verticalVelocity, position, isForced);
    }

    private int CalculateHeading(float directionAngle)
    {
        return (int)VectorUtils.ConvertRotToUnreal(directionAngle);
    }
}
