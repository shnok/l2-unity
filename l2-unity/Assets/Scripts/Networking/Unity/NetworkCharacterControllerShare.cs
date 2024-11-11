using System;
using UnityEngine;

[RequireComponent(typeof(NetworkTransformShare)), RequireComponent(typeof(CharacterController))]
public class NetworkCharacterControllerShare : MonoBehaviour
{
    private CharacterController _characterController;
    [SerializeField] private int _sharingLoopDelayMs = 100;

    [SerializeField] private Vector3 _lastDirection;
    [SerializeField] private Vector3 _lastForcedDirection;
    [SerializeField] private float _lastDirectionAngle;

    [SerializeField] private long _lastSharingTimestamp = 0;
    [SerializeField] private int _heading;

    public int Heading { get { return _heading; } set { _heading = value; } }

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

        _lastForcedDirection = new Vector3(-1, -1, -1);
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
                NetworkTransformShare.Instance.ShouldShareRotation = false;
            }
            else
            {
                NetworkTransformShare.Instance.ShouldShareRotation = true;
            }

            ShareMoveDirection(newDirection);
            _lastDirection = newDirection;
        }
    }

    private bool ShouldShareMoveDirection(Vector3 newDirection, long timestamp)
    {
        if (_lastDirection == newDirection)
        {
            return false;
        }

        if (VectorUtils.IsVectorZero2D(_lastDirection) && !VectorUtils.IsVectorZero2D(newDirection))
        {
            // player just moved
            return true;
        }

        if (!VectorUtils.IsVectorZero2D(_lastDirection) && VectorUtils.IsVectorZero2D(newDirection))
        {
            // player just stopped
            return true;
        }

        // Basic loop delay
        if (timestamp - _lastSharingTimestamp >= _sharingLoopDelayMs && newDirection != _lastDirection)
        {
            return true;
        }

        return false;
    }

    public void ForceShareMoveDirection()
    {
        ShareMoveDirection(PlayerController.Instance.MoveDirection.normalized, true);
    }

    public void ShareMoveDirection(Vector3 moveDirection)
    {
        ShareMoveDirection(moveDirection, false);
    }

    public void ShareMoveDirection(Vector3 moveDirection, bool isForced)
    {
        Vector3 previousDirection = isForced ? _lastForcedDirection : _lastDirection;
        if (previousDirection.x == moveDirection.x && previousDirection.z == moveDirection.z)
        {
            // The direction hasnt changed
            return;
        }

        float directionAngle = VectorUtils.CalculateMoveDirectionAngle(moveDirection.x, moveDirection.z);
        if (!isForced)
        {
            if (Math.Abs(Math.Abs(directionAngle) - Math.Abs(_lastDirectionAngle)) < 2f)
            {
                Debug.Log("The direction change is too small to share");
                // The direction change is too small to share
                return;
            }
        }

        _lastDirectionAngle = directionAngle;

        if (!VectorUtils.IsVectorZero2D(moveDirection))
        {
            Heading = CalculateHeading(directionAngle);
        }

        if (!isForced)
        {
            _lastForcedDirection = Vector3.one;
            _lastDirection = moveDirection;
        }
        else
        {
            _lastForcedDirection = moveDirection;
        }

        GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(moveDirection, Heading);
    }

    private int CalculateHeading(float directionAngle)
    {
        return (int)VectorUtils.ConvertRotToUnreal(directionAngle);
    }
}
