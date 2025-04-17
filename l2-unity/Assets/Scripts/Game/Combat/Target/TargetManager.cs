using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private Entity _target = null;
    private Entity _attackTarget = null;
    private Transform _playerTransform;

    [SerializeField] private float _maximumTargetDistance = 15f;
    [SerializeField] private LayerMask _entityMask;
    [SerializeField] private RaycastHit[] _entitiesInRange;
    [SerializeField] private List<Transform> _visibleEntities;
    [SerializeField] private int _nextTargetIndex;

    public Entity Target { get { return _target; } }
    public Entity AttackTarget { get { return _attackTarget; } }

    private static TargetManager _instance;
    public static TargetManager Instance { get { return _instance; } }

    private void Awake()
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

    private void Start()
    {
        _target = null;
        _attackTarget = null;
    }

    void OnDestroy()
    {
        _instance = null;
    }

    public void SetMask(LayerMask mask)
    {
        _entityMask = mask;
    }

    private void FixedUpdate()
    {
        if (_playerTransform == null)
        {
            if (PlayerEntity.Instance != null && PlayerEntity.Instance.transform != null)
            {
                _playerTransform = PlayerEntity.Instance.transform;
            }
            else
            {
                return;
            }
        }

        VerifyNextTargetIndex();

        _entitiesInRange = Physics.SphereCastAll(_playerTransform.position, _maximumTargetDistance, transform.forward, 0, _entityMask);
        _visibleEntities = GetVisibleEntities();
    }

    private List<Transform> GetVisibleEntities()
    {
        List<Transform> visibleEntities = new List<Transform>();

        for (int i = 0; i < _entitiesInRange.Length; i++)
        {
            Transform entityTransform = _entitiesInRange[i].transform;
            Entity entity = entityTransform.GetComponent<Entity>();
            if (IsTransformVisible(entityTransform) && entity.Identity.EntityType == EntityType.Monster && !entity.IsDead)
            {
                visibleEntities.Add(entityTransform);
            }
        }

        visibleEntities = visibleEntities
            .OrderBy(obj => Vector3.Distance(_playerTransform.position, obj.position))
            .ToList();

        return visibleEntities;
    }

    private bool IsTransformVisible(Transform target)
    {
        bool isTooFar = Vector3.Distance(_playerTransform.position, target.position) > _maximumTargetDistance;
        if (isTooFar)
        {
            return false;
        }

        return CameraController.Instance.IsObjectVisible(target);
    }

    private void VerifyNextTargetIndex()
    {
        // Is the next target out of bounds
        if (_nextTargetIndex >= _visibleEntities.Count)
        {
            _nextTargetIndex = 0;
            return;
        }

        // Is our target visible
        if (HasTarget() && !IsTransformVisible(_target.transform))
        {
            _nextTargetIndex = 0;
            return;
        }

        // Did my next target index change?
        if (HasTarget())
        {
            for (int i = 0; i < _visibleEntities.Count; i++)
            {
                if (_visibleEntities[i] == _target.transform)
                {
                    if (i != _nextTargetIndex)
                    {
                        _nextTargetIndex = 0;
                    }
                }
            }

            return;
        }
    }

    private int GetNextTargetIndex()
    {
        int index = _nextTargetIndex;

        if (_visibleEntities.Count == 0)
        {
            return 0;
        }

        if (index >= _visibleEntities.Count)
        {
            index = 0;
        }

        if (HasTarget() && _visibleEntities[index] == _target.transform)
        {
            index++;
        }

        if (index >= _visibleEntities.Count)
        {
            index = 0;
        }

        return index;
    }

    public void NextTarget()
    {
        if (_visibleEntities.Count == 0)
        {
            return;
        }

        _nextTargetIndex = GetNextTargetIndex();

        SetTarget(new ObjectData(_visibleEntities[_nextTargetIndex].gameObject));
    }

    public void SetTarget(ObjectData target)
    {
        if (target == null)
        {
            ClearTarget();
            return;
        }

        SetTarget(target.ObjectTransform.GetComponent<Entity>());
    }

    public void SetTarget(Entity target)
    {
        if (target == null)
        {
            return;
        }

        _target = target;

        PlayerCombat.Instance.TargetId = _target.Identity.Id;
        PlayerCombat.Instance.Target = _target;

        ShopWindow.Instance.HideWindow(false);

        GameClient.Instance.ClientPacketHandler.SendRequestSetTarget(_target.Identity.Id);
    }

    public void ClearAttackTarget()
    {
        _attackTarget = null;
    }

    public bool IsAttackTargetSet()
    {
        return _target != null && _attackTarget != null && _target.Identity.Id == _attackTarget.Identity.Id;
    }

    public bool HasTarget()
    {
        return _target != null && _target.transform != null;
    }

    public bool HasAttackTarget()
    {
        return _attackTarget != null;
    }

    public void ClearTarget()
    {
        ClearTarget(true);
    }

    public void ClearTarget(bool request)
    {
        if (HasTarget())
        {
            if (PlayerCombat.Instance.TargetId != -1)
            {
                if (request)
                {
                    GameClient.Instance.ClientPacketHandler.SendRequestCancel(false);
                }
                PlayerCombat.Instance.TargetId = -1;
                PlayerCombat.Instance.Target = null;
            }

            _target = null;
            _attackTarget = null;
        }
    }

    void Update()
    {
        if (PlayerEntity.Instance == null)
        {
            return;
        }

        if (HasTarget())
        {
            // _target.Distance = Vector3.Distance(
            //     PlayerController.Instance.transform.position,
            //     _target.Data.ObjectTransform.position);
        }
        else
        {
            ClearTarget();
        }
    }

    internal void SetAttackTarget(Entity attackTarget)
    {
        _attackTarget = attackTarget;
    }
}
