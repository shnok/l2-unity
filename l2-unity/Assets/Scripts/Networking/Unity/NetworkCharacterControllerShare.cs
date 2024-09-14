using System;
using UnityEngine;

[RequireComponent(typeof(NetworkTransformShare)), RequireComponent(typeof(CharacterController))]
public class NetworkCharacterControllerShare : MonoBehaviour
{
    private CharacterController _characterController;
    [SerializeField] private int _sharingLoopDelayMs = 100;
    [SerializeField] private Vector3 _lastDirection;
    [SerializeField] private long _lastSharingTimestamp = 0;

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
    }

    private void FixedUpdate()
    {
        Vector3 newDirection = PlayerController.Instance.MoveDirection.normalized;
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (ShouldShareMoveDirection(newDirection, now))
        {
            _lastSharingTimestamp = now;
            _lastDirection = newDirection;

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
        }
    }

    private bool ShouldShareMoveDirection(Vector3 newDirection, long timestamp)
    {
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

    public void ShareMoveDirection(Vector3 moveDirection)
    {
        _lastDirection = moveDirection;
        GameClient.Instance.ClientPacketHandler.UpdateMoveDirection(moveDirection);
    }

    public void ForceShareMoveDirection()
    {
        ShareMoveDirection(PlayerController.Instance.MoveDirection.normalized);
    }
}
