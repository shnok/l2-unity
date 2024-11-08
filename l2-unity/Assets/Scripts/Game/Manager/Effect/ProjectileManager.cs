using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager _instance;
    public static ProjectileManager Instance { get { return _instance; } }

    private Queue<PooledEffect> _activeProjectiles;

    public Queue<PooledEffect> ActiveProjectiles
    {
        get
        {
            if (_activeProjectiles == null)
            {
                _activeProjectiles = new Queue<PooledEffect>();
            }

            return _activeProjectiles;
        }
        private set
        {
            _activeProjectiles = value;
        }
    }


    #region Initialization
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
    }
    #endregion

    #region Projectile Management tasks
    private void Update()
    {
        ManageProjectiles();
    }

    private void ManageProjectiles()
    {
        if (ActiveProjectiles.Count > 0)
        {
            PooledEffect effect = ActiveProjectiles.Peek();

            if (effect.Caster == null || effect.Target == null)
            {
                Debug.LogWarning("Projectile doesn't have a caster or target.");
                ActiveProjectiles.Dequeue();
                return;
            }

            float particleHeight = effect.Target.Appearance.CollisionHeight * 1.25f;
            Vector3 targetPosition = effect.Target.transform.position + Vector3.up * particleHeight;

            float lerpRatio = (Time.time - effect.StartTime) / (effect.HitTime - effect.StartTime);
            lerpRatio = Mathf.Min(1, lerpRatio);

            effect.GameObject.transform.position = Vector3.Lerp(effect.StartingPosition, targetPosition, lerpRatio);

            if (lerpRatio >= 1)
            {
                ActiveProjectiles.Dequeue();
                effect.GameObject?.SetActive(false);
            }
        }
    }
    #endregion

    public void AddProjectile(PooledEffect effect)
    {
        ActiveProjectiles.Enqueue(effect);
    }
}
