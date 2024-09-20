using UnityEngine;


[ExecuteInEditMode]
public class ParticleDebug : MonoBehaviour
{
    [SerializeField] private float _lastLoop = 0;
    [SerializeField] private float _loopDelay = 1;
    void Start()
    {
        _lastLoop = 0;
    }

    void OnEnable()
    {
        _lastLoop = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _lastLoop + _loopDelay)
        {
            _lastLoop = Time.time;
            ParticleTimerResetGroup resetGroup = GetComponentInChildren<ParticleTimerResetGroup>();
            if (resetGroup == null)
            {
                return;
            }
            resetGroup.enabled = false;
            resetGroup.enabled = true;
        }
    }
}
