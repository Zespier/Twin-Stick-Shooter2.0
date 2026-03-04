using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CharacterSaveManager {
    private static string SaveFolder => Application.persistentDataPath + "/Characters/";

    public static void SaveCharacter() {
        if (!Directory.Exists(SaveFolder)) {
            Directory.CreateDirectory(SaveFolder);
        }

        CharacterData characterData = FillCharacterDataWithShip();

        string json = JsonUtility.ToJson(characterData, true);
        File.WriteAllText(SaveFolder + characterData.characterId + ".json", json);
    }

    public static void LoadCharacter(string id) {
        string path = SaveFolder + id + ".json";
        if (!File.Exists(path)) { return; }

        string json = File.ReadAllText(path);
        CharacterData characterData = JsonUtility.FromJson<CharacterData>(json);
        FillShipWithCharacterData( characterData);
    }

    public static void FillShipWithCharacterData(CharacterData characterData) {

        if (characterData != null) {
            Ship.instance.monedaBarata = characterData.monedaBarata;
            Ship.instance.monedaCara = characterData.monedaCara;
            Ship.instance.minerals = characterData.minerals;
            Ship.instance.lasserAmmo = characterData.lasserAmmo;
            Ship.instance.laserTierBeingUsed = characterData.equippedLaserAmmo;
            Ship.instance.weaponInventory = characterData.weaponInventory;
        } else {

            CharacterData newCharacterData = new();
            Ship.instance.monedaBarata = newCharacterData.monedaBarata;
            Ship.instance.monedaCara = newCharacterData.monedaCara;
            Ship.instance.minerals = newCharacterData.minerals;
            Ship.instance.lasserAmmo = newCharacterData.lasserAmmo;
            Ship.instance.laserTierBeingUsed = newCharacterData.equippedLaserAmmo;
            Ship.instance.weaponInventory = newCharacterData.weaponInventory;
        }
    }

    public static CharacterData FillCharacterDataWithShip() {

        CharacterData characterData = new();

        characterData.monedaBarata = Ship.instance.monedaBarata;
        characterData.monedaCara = Ship.instance.monedaCara;
        characterData.minerals = Ship.instance.minerals;
        characterData.lasserAmmo = Ship.instance.lasserAmmo;
        characterData.equippedLaserAmmo = Ship.instance.laserTierBeingUsed;
        characterData.weaponInventory = Ship.instance.weaponInventory;

        return characterData;
    }
}

[System.Serializable]
public class CharacterSlotIndex {
    public List<string> characterIds;
}

[System.Serializable]
public class CharacterData {
    public int dataVersion = 1;

    public string characterId;
    public string characterName;

    public int monedaBarata;
    public int monedaCara;
    public int[] minerals;
    public Ammo[] lasserAmmo;
    public List<Weapon> weaponInventory = new();
    public List<ShipTypes> ownedShips = new();

    public ShipTypes equippedShip;
    public int equippedLaserAmmo = 1;

    public long lastSaveTime;
}