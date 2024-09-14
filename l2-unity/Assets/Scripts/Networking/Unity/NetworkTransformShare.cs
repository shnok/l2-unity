using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NetworkTransformShare : MonoBehaviour
{
    private CharacterController _characterController;
    private Vector3 _lastPos, _lastRot;
    private float _lastSharedPosTime;

    [SerializeField] public Vector3 _serverPosition;
    [SerializeField] public bool _shouldShareRotation;
    public bool _rotationShareEnabled;

    public bool ShouldShareRotation { get { return _shouldShareRotation; } set { _shouldShareRotation = value; } }

    private static NetworkTransformShare _instance;
    public static NetworkTransformShare Instance { get { return _instance; } }

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

    void Start()
    {
        if (World.Instance.OfflineMode)
        {
            enabled = false;
            return;
        }

        _characterController = GetComponent<CharacterController>();
        _lastPos = transform.position;
        _lastRot = transform.forward;
        _lastSharedPosTime = Time.time;
        _rotationShareEnabled = false; //rotation is now calculated based on movedirection
    }

    void Update()
    {
        if (ShouldSharePosition())
        {
            SharePosition();
        }

        if (ShouldShareRotation && _rotationShareEnabled)
        {
            ShareRotation();
        }
    }

    // Share position every 0.25f and based on delay
    public bool ShouldSharePosition()
    {
        if (Vector3.Distance(transform.position, _lastPos) > .25f || Time.time - _lastSharedPosTime >= 10f)
        {
            return true;
        }

        return false;
    }

    public void SharePosition()
    {
        GameClient.Instance.ClientPacketHandler.UpdatePosition(transform.position);
        _lastSharedPosTime = Time.time;
        _lastPos = transform.position;

        //ClientPacketHandler.Instance.UpdateRotation(transform.eulerAngles.y);
    }

    public void ShareRotation()
    {
        if (Vector3.Angle(_lastRot, transform.forward) >= 10.0f)
        {
            _lastRot = transform.forward;
            GameClient.Instance.ClientPacketHandler.UpdateRotation(transform.eulerAngles.y);
        }
    }

    public void ShareAnimation(byte id, float value)
    {
        GameClient.Instance.ClientPacketHandler.UpdateAnimation(id, value);
    }
}