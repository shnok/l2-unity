using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCreator : MonoBehaviour
{
    private static CharacterCreator _instance;
    public static CharacterCreator Instance { get { return _instance; } }

    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null) {
            _instance = this;
        } else if(_instance != this) {
            Destroy(gameObject);
        }
    }


    public void SpawnDefaultPawns() {
        List<Logongrp> pawns = LogongrpTable.Instance.Logongrps;

        pawns.ForEach(pawn => {
            CharacterRaceAnimation raceId = CharacterRaceAnimation.FDarkElf;
            PlayerAppearance appearance = new PlayerAppearance();
            GameObject pawnObject = CharacterBuilder.Instance.BuildCharacterBase(raceId, appearance, EntityType.Pawn);

            Vector3 pawnPosition = new Vector3(pawn.X, pawn.Y, pawn.Z);
            pawnPosition = VectorUtils.ConvertPosToUnity(pawnPosition);    
            pawnObject.transform.position = pawnPosition;
            pawnObject.transform.eulerAngles = new Vector3(0, 360.00f * pawn.Yaw / 65536, 0);

            UserGear gear = pawnObject.GetComponent<UserGear>();
            gear.Initialize(-1, raceId);

            if (appearance.Chest != 0) {
                gear.EquipArmor(appearance.Chest, ItemSlot.chest);
            } else {
                gear.EquipArmor(ItemTable.NAKED_CHEST, ItemSlot.chest);
            }

            if (appearance.Legs != 0) {
                gear.EquipArmor(appearance.Legs, ItemSlot.legs);
            } else {
                gear.EquipArmor(ItemTable.NAKED_LEGS, ItemSlot.legs);
            }

            if (appearance.Gloves != 0) {
                gear.EquipArmor(appearance.Gloves, ItemSlot.gloves);
            } else {
                gear.EquipArmor(ItemTable.NAKED_GLOVES, ItemSlot.gloves);
            }

            if (appearance.Feet != 0) {
                gear.EquipArmor(appearance.Feet, ItemSlot.feet);
            } else {
                gear.EquipArmor(ItemTable.NAKED_BOOTS, ItemSlot.feet);
            }

            pawnObject.SetActive(true);
        });
    }
}
