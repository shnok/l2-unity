using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBuilder : MonoBehaviour
{
    private static CharacterBuilder _instance;
    public static CharacterBuilder Instance { get { return _instance; } }

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    // Load player animations, face and hair
    public GameObject BuildCharacterBase(CharacterRaceAnimation raceId, PlayerAppearance appearance, bool isPlayer) {
        Debug.Log($"Building character: {raceId} {appearance.Sex} {appearance.Face} {appearance.HairStyle} {appearance.HairColor}");

        GameObject entity = Instantiate(ModelTable.Instance.GetContainer(raceId, isPlayer));
        GameObject face = Instantiate(ModelTable.Instance.GetFace(raceId, appearance.Face));
        GameObject hair1 = Instantiate(ModelTable.Instance.GetHair(raceId, appearance.HairStyle, appearance.HairColor, false));
        GameObject hair2 = Instantiate(ModelTable.Instance.GetHair(raceId, appearance.HairStyle, appearance.HairColor, true));

        Transform container = entity.transform;
        if (!isPlayer) {
            container = entity.transform.GetChild(0);
        }

        face.transform.SetParent(container.transform, false);
        hair1.transform.SetParent(container.transform, false);
        hair2.transform.SetParent(container.transform, false);

        return entity;
    }
}
