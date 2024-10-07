using UnityEngine;

public class SelectableCharacterEntity : Entity
{
    private HumanoidAnimationController _AnimationController { get { return (HumanoidAnimationController)_referenceHolder.AnimationController; } }

    [SerializeField] private CharacterController _characterController;

    [Header("Attributes")]
    [SerializeField] private Vector3 _destination;
    [SerializeField] private Vector3 _destEulerAngles;
    [SerializeField] private CharSelectionInfoPackage _characterInfo;
    [SerializeField] private bool _walking = false;

    private float _walkSpeed = 1.5f;

    public CharSelectionInfoPackage CharacterInfo { get { return _characterInfo; } set { _characterInfo = value; } }


    private void Awake()
    {
        if (_characterController == null)
        {
            _characterController = GetComponent<CharacterController>();
            Debug.LogWarning($"[{transform.name}] CharacterController was not assigned, please pre-assign it to avoid unecessary load.");
        }

        if (_referenceHolder == null)
        {
            _referenceHolder = GetComponent<EntityReferenceHolder>();
            Debug.LogWarning($"[{transform.name}] EntityReferenceHolder was not assigned, please pre-assign it to avoid unecessary load.");
        }

        _destination = transform.position;
    }

    private void Update()
    {
        float ditanceToDestination = Vector3.Distance(_destination, transform.position);
        if (ditanceToDestination > 0.05f)
        {
            if (!_walking)
            {
                StartWalking();
            }

            transform.LookAt(new Vector3(_destination.x, transform.position.y, _destination.z));
            _characterController.Move(transform.forward.normalized * Time.deltaTime * _walkSpeed);
        }
        else if (_walking)
        {
            StopWalking();
        }
    }

    private void StartWalking()
    {
        _walking = true;
        _AnimationController.SetBool(HumanoidAnimType.wait, false);
        _AnimationController.SetBool(HumanoidAnimType.walk, true);
    }

    private void StopWalking()
    {
        _walking = false;
        _AnimationController.SetBool(HumanoidAnimType.walk, false);
        _AnimationController.SetBool(HumanoidAnimType.wait, true);
        transform.eulerAngles = _destEulerAngles;
    }

    public void SetDestination(Logongrp destination)
    {
        Vector3 pawnPosition = new Vector3(destination.X, destination.Y, destination.Z);
        _destination = VectorUtils.ConvertPosToUnity(pawnPosition);
        _destEulerAngles = new Vector3(0, 360.00f * destination.Yaw / 65536, 0);
    }
}
