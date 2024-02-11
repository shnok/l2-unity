using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ParticleRenderer : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _particles;
    [SerializeField] private int _aliveCount;

    void Awake() {
        //_particles = transform.GetComponentsInChildren<ParticleSystem>().ToList();
    }

    void Update() {
        _particles = transform.GetComponentsInChildren<ParticleSystem>();
        //_aliveCount = _particles.Select(x => x != null).ToList().Count();

        if (_particles.Length == 0) {
            Destroy(gameObject);
        }
    }
}
