using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    private static ProjectileManager _instance;
    public static ProjectileManager Instance { get { return _instance; } }

    private List<PooledEffect> _activeProjectiles;

    public List<PooledEffect> ActiveProjectiles
    {
        get
        {
            if (_activeProjectiles == null)
            {
                _activeProjectiles = new List<PooledEffect>();
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
            for (int i = ActiveProjectiles.Count - 1; i >= 0; i--)
            {
                PooledEffect effect = ActiveProjectiles[i];

                if (effect.Caster == null || effect.Target == null)
                {
                    Debug.LogWarning("Projectile doesn't have a caster or target.");
                    ActiveProjectiles.RemoveAt(i);
                    continue;
                }

                Vector3 targetPosition = effect.Target.transform.position; // shoot at the ground if missed

                if (effect.HitSuccess)
                {
                    targetPosition += Vector3.up * effect.Target.Appearance.CollisionHeight * 1.25f;
                }

                float lerpRatio = (Time.time - effect.StartTime) / (effect.HitTime - effect.StartTime);
                lerpRatio = Mathf.Min(1, lerpRatio);

                effect.GameObject.transform.position = Vector3.Lerp(effect.StartingPosition, targetPosition, lerpRatio);

                if (lerpRatio >= 1)
                {
                    ActiveProjectiles.RemoveAt(i);

                    if (effect.HitSuccess)
                    {
                        // Pierce target
                        effect.GameObject.transform.parent = effect.Target.transform.GetChild(0).GetChild(0); //rootbone
                    }
                    else
                    {
                        // Pierce the ground
                        effect.GameObject.transform.position += Vector3.up * 0.2f;
                        effect.GameObject.transform.eulerAngles = new Vector3(effect.GameObject.transform.eulerAngles.x, effect.GameObject.transform.eulerAngles.y, effect.GameObject.transform.eulerAngles.z + 50);
                    }
                }
            }
        }
    }
    #endregion

    public void AddProjectile(PooledEffect effect)
    {
        ActiveProjectiles.Add(effect);
    }
}
