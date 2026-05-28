using UnityEngine;

public class CharacterBuilder : MonoBehaviour
{
    private static CharacterBuilder _instance;
    public static CharacterBuilder Instance { get { return _instance; } }

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

    // Load player animations, face and hair
    public GameObject BuildCharacterBase(CharacterModelType raceId, PlayerAppearance appearance, EntityType entityType)
    {

        //Debug.Log($"Building character: Race:{raceId} Sex:{appearance.Sex} Face:{appearance.Face} Hair:{appearance.HairStyle} HairC:{appearance.HairColor}");

        Entity entity;
        Instantiate(ModelTable.Instance.GetContainer(raceId, entityType)).TryGetComponent<Entity>(out entity);
        if (entity == null)
        {
            Debug.LogError($"Character base {raceId} missing entity class.");
        }
        else
        {
            entity.Appearance.CollisionHeight = appearance.CollisionHeight;
            entity.Appearance.CollisionRadius = appearance.CollisionRadius;
        }

        return entity.gameObject;
    }
}
