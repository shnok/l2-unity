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
            entity.UpdateAppearance(appearance);
        }

        // GameObject face = ModelTable.Instance.GetFace(raceId, appearance.Face);
        // GameObject hair1 = ModelTable.Instance.GetHair(raceId, appearance.HairStyle, appearance.HairColor, false);
        // GameObject hair2 = ModelTable.Instance.GetHair(raceId, appearance.HairStyle, appearance.HairColor, true);

        // Transform container = entity.transform.GetChild(0).GetChild(1);

        // face?.transform.SetParent(container.transform, false);
        // hair1?.transform.SetParent(container.transform, false);

        // UserGear gear = (UserGear)entity.GetComponent<Entity>().Gear;

        // Transform headBone = gear.HeadBone?.transform;

        // if (headBone == null)
        // {
        //     gear.HeadBone = entity.transform
        //                     .GetChild(0)
        //                     .GetChild(0)
        //                     .GetChild(0)
        //                     .GetChild(0)
        //                     .GetChild(0)
        //                     .GetChild(0)
        //                     .GetChild(2)
        //                     .GetChild(0)
        //                     .GetChild(0)
        //                     .GetChild(0).gameObject;
        // }

        // if (hair2 != null)
        // {
        //     if (hair2.tag == "Hair") // this hair doesnt have an armature and needs to be placed manually under the head bone
        //     {

        //         Vector3 origin = hair2.transform.localPosition;
        //         Vector3 originEuler = hair2.transform.eulerAngles;

        //         hair2.transform.SetParent(headBone);

        //         hair2.transform.localPosition = origin;
        //         hair2.transform.localEulerAngles = originEuler;
        //     }
        //     else
        //     {
        //         hair2.transform.SetParent(container.transform, false);
        //     }
        // }

        // gear.Hairs[(int)HairType.AH] = hair1?.gameObject;
        // gear.Hairs[(int)HairType.BH] = hair2?.gameObject;
        // gear.Face = face;

        return entity.gameObject;
    }
}
