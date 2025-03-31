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
            bool sharePosition = false;
            Vector3 position = Vector3.zero;
            if (ShouldSharePosition(now))
            {
                sharePosition = true;
                position = transform.position;
                _lastSharingPosition = now;
            }
            float verticalVelocity = 0.0f;

            if (ShouldShareJumping())
            {
                //TODO: Maybe we need validate if the player is falling or not
                verticalVelocity = PlayerController.Instance._verticalVelocity;
                _isSharedJumping = true;
            }


            ShareMoveDirection(newDirection, verticalVelocity, sharePosition, position);
            _lastDirection = newDirection;
        }
    }
    private bool ShouldSharePosition(long timestamp)
    {
        if (timestamp - _lastSharingPosition >= _sharingPositionDelayMs)
        {
            Debug.LogWarning("Sharing move direction: Should share position: passed time" + timestamp);
            return true;
        }
        return false;
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

        if (VectorUtils.IsVectorZero2D(newDirection) && VectorUtils.IsVectorZero2D(_lastDirection))
        {
            // player just stopped and is not moving
            return false;
        }
        if (ShouldSharePosition(timestamp))
        {
            return true;
        }
        /*
        Removed these validations because they are not needed anymore
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
        */
        // Basic loop delay
        if (timestamp - _lastSharingTimestamp >= _sharingLoopDelayMs)
        {
            return true;
        }

        return false;
    }

    public void ForceShareMoveDirection()
    {
        ShareMoveDirection(PlayerController.Instance.MoveDirection.normalized, true, 0.0f, false, Vector3.zero);
    }

    public void ShareMoveDirection(Vector3 moveDirection, float verticalVelocity, bool sharePosition, Vector3 position)
    {
        ShareMoveDirection(moveDirection, false, verticalVelocity, sharePosition, position);
    }

    public void ShareMoveDirection(Vector3 moveDirection, bool isForced, float verticalVelocity, bool sharePosition, Vector3 position)
    {
        Vector3 previousDirection = isForced ? _lastForcedDirection : _lastDirection;
        /* if (previousDirection.x == moveDirection.x && previousDirection.z == moveDirection.z)
        {
            // The direction hasnt changed
            return;
        } */

        float directionAngle = VectorUtils.CalculateMoveDirectionAngle(moveDirection.x, moveDirection.z);
        /* if (!isForced)
        {
            if (Math.Abs(Math.Abs(directionAngle) - Math.Abs(_lastDirectionAngle)) < 2f)
            {
                // Debug.Log("The direction change is too small to share");
                // The direction change is too small to share
                return;
            }
        } */

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

        GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(moveDirection, Heading, verticalVelocity, sharePosition, position);
    }

    private int CalculateHeading(float directionAngle)
    {
        return (int)VectorUtils.ConvertRotToUnreal(directionAngle);
    }
}
