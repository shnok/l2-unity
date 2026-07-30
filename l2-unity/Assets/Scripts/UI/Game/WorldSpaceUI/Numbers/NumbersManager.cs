using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NumbersManager : MonoBehaviour
{
    private List<PooledNumber> _activeNumbers;
    private Queue<PooledNumber> _numberPool;
    private GameObject _numberContainer;
    private GameObject _numberPoolContainer;

    private GameObject _numbersPrefab;

    [SerializeField] protected Transform playerTransform;

    [SerializeField] protected Camera _mainCamera;
    public Camera MainCamera { get => _mainCamera; }
    protected VisualElement rootElement;
    protected VisualTreeAsset nameplateTemplate;

    public Queue<PooledNumber> NumberPool
    {
        get
        {
            _numberPool ??= new Queue<PooledNumber>();

            return _numberPool;
        }
        private set
        {
            _numberPool = value;
        }
    }

    public List<PooledNumber> ActiveNumbers
    {
        get
        {
            _activeNumbers ??= new List<PooledNumber>();

            return _activeNumbers;
        }
        private set
        {
            _activeNumbers = value;
        }
    }

    public static NumbersManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        _numbersPrefab = Resources.Load<GameObject>("Data/UI/Game/WorldSpaceUI/Number/Number");
    }

    public void Initialize()
    {
        ActiveNumbers.Clear();
        NumberPool.Clear();

        if (_numberPoolContainer != null)
            Destroy(_numberPoolContainer);
        if (_numberContainer != null)
            Destroy(_numberContainer);

        GameObject worldSpaceUI = GameObject.Find("UI");
        _numberPoolContainer = new GameObject("NumbersPool");
        _numberContainer = new GameObject("NumbersActive");
        _numberContainer.transform.SetParent(worldSpaceUI.transform);
        _numberPoolContainer.transform.SetParent(worldSpaceUI.transform);
        _mainCamera = Camera.main;

        PrepareNumberPool();
    }

    private void PrepareNumberPool()
    {
        for (int i = 0; i < 100; i++)
        {
            GameObject numberObject = Instantiate(_numbersPrefab, _numberPoolContainer.transform);
            PooledNumber number = numberObject.GetComponent<PooledNumber>();
            numberObject.SetActive(false);
            NumberPool.Enqueue(number);
        }
    }

    public void PoolEffect(PooledNumber toPool)
    {
        ActiveNumbers.Remove(toPool);
        toPool.transform.SetParent(_numberPoolContainer.transform);
        NumberPool.Enqueue(toPool);
    }

    public void OnEntityDamaged(Entity attacker, Entity target, float value, NumberType numberType)
    {
        if (NumberPool.Count > 0)
        {
            PooledNumber pooledNumber = NumberPool.Dequeue();
            ActiveNumbers.Add(pooledNumber);
            pooledNumber.transform.SetParent(_numberContainer.transform);
            pooledNumber.transform.position = target.transform.position + Vector3.up * target.Appearance.CollisionHeight;
            pooledNumber.gameObject.SetActive(true);

            if (numberType == NumberType.Damaging)
            {
                bool right = IsOnRightOnScreen(attacker.transform, target.transform);
                pooledNumber.Animate(right ? NumberAnimationStyle.BounceRight : NumberAnimationStyle.BounceLeft, numberType, value);
            }
            else
            {
                pooledNumber.Animate(NumberAnimationStyle.ScrollUp, numberType, value);
            }
        }
        else
        {
            Debug.LogWarning("Out of pooled numbers.");
        }
    }

    private void Update()
    {
        if (!IsSystemReady()) return;
    }

    private void FixedUpdate()
    {
        if (!IsSystemReady()) return;
    }

    private void LateUpdate()
    {
        if (!IsSystemReady()) return;
    }

    protected virtual bool IsSystemReady()
    {
        if (GameManager.Instance == null)
        {
            return false;
        }

        if (GameManager.Instance.State != GameState.IN_GAME)
        {
            return false;
        }

        return true;
    }

    public bool IsInView(Transform target)
    {
        Vector3 viewportPos = _mainCamera.WorldToViewportPoint(target.position);

        bool inFront = viewportPos.z > 0;                       // in front of camera
        bool insideX = viewportPos.x >= 0 && viewportPos.x <= 1;
        bool insideY = viewportPos.y >= 0 && viewportPos.y <= 1;

        return inFront && insideX && insideY;
    }

    public bool IsOnRightOnScreen(Transform reference, Transform other)
    {
        Vector3 refScreenPos = _mainCamera.WorldToViewportPoint(reference.position);
        Vector3 otherScreenPos = _mainCamera.WorldToViewportPoint(other.position);

        // We only care about X: 0 = left edge, 1 = right edge
        return otherScreenPos.x > refScreenPos.x;
    }
}
