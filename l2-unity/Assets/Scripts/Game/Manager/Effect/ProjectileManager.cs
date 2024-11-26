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

                if (effect == null || effect.Caster == null || effect.Target == null || effect.GameObject == null)
                {
                    Debug.LogWarning("Projectile doesn't have a caster or target.");
                    ActiveProjectiles.RemoveAt(i);
                    continue;
                }

                UpdateEffectTargetPosition(effect);

                float lerpRatio = (Time.time - effect.StartTime) / (effect.HitTime - effect.StartTime);
                lerpRatio = Mathf.Min(1, lerpRatio);

                effect.GameObject.transform.position = Vector3.Lerp(effect.StartingPosition, effect.TargetPosition, lerpRatio);

                if (lerpRatio >= 1)
                {
                    float randomPosX = Random.Range(-1f, 1f);
                    float randomPosY = Random.Range(-1f, 1f);

                    ActiveProjectiles.RemoveAt(i);

                    if (effect.HitSuccess)
                    {
                        effect.GameObject.transform.position += new Vector3(randomPosX, randomPosY, randomPosY) * 0.075f;

                        // Pierce target
                        if (effect.Target.AnimationController.RootBone != null)
                        {
                            effect.GameObject.transform.parent = effect.Target.AnimationController.RootBone; //rootbone
                        }
                        else
                        {
                            effect.GameObject.transform.parent = effect.Target.transform;
                        }
                    }
                    else
                    {
                        // Pierce the ground
                        effect.GameObject.transform.position += Vector3.up * 0.2f + new Vector3(randomPosX, randomPosY * 0.1f, randomPosY) * 0.2f;
                        effect.GameObject.transform.eulerAngles = new Vector3(effect.GameObject.transform.eulerAngles.x + randomPosX * 5f, effect.GameObject.transform.eulerAngles.y + randomPosX * 5f, effect.GameObject.transform.eulerAngles.z + 50 + randomPosX * 15f);
                    }
                }
            }
        }
    }
    #endregion

    private void UpdateEffectTargetPosition(PooledEffect effect)
    {
        Vector3 targetPosition = effect.Target.transform.position; // shoot at the ground if missed

        if (effect.HitSuccess)
        {
            targetPosition += Vector3.up * effect.Target.Appearance.CollisionHeight * 1.25f;
        }

        effect.TargetPosition = targetPosition;
    }

    public void AddProjectile(PooledEffect effect)
    {
        UpdateEffectTargetPosition(effect);
        effect.GameObject.transform.LookAt(effect.TargetPosition);
        effect.GameObject.transform.eulerAngles = effect.GameObject.transform.eulerAngles + Vector3.up * 90;

        ActiveProjectiles.Add(effect);
    }
}
