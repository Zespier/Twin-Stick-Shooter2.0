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
            Ship.instanceOfClient.monedaBarata = characterData.monedaBarata;
            Ship.instanceOfClient.monedaCara = characterData.monedaCara;
            Ship.instanceOfClient.minerals = characterData.minerals;
            Ship.instanceOfClient.lasserAmmo = characterData.lasserAmmo;
            Ship.instanceOfClient.laserTierBeingUsed = characterData.equippedLaserAmmo;
            Ship.instanceOfClient.weaponInventory = characterData.weaponInventory;
        } else {

            CharacterData newCharacterData = new();
            Ship.instanceOfClient.monedaBarata = newCharacterData.monedaBarata;
            Ship.instanceOfClient.monedaCara = newCharacterData.monedaCara;
            Ship.instanceOfClient.minerals = newCharacterData.minerals;
            Ship.instanceOfClient.lasserAmmo = newCharacterData.lasserAmmo;
            Ship.instanceOfClient.laserTierBeingUsed = newCharacterData.equippedLaserAmmo;
            Ship.instanceOfClient.weaponInventory = newCharacterData.weaponInventory;
        }
    }

    public static CharacterData FillCharacterDataWithShip() {

        CharacterData characterData = new();

        characterData.monedaBarata = Ship.instanceOfClient.monedaBarata;
        characterData.monedaCara = Ship.instanceOfClient.monedaCara;
        characterData.minerals = Ship.instanceOfClient.minerals;
        characterData.lasserAmmo = Ship.instanceOfClient.lasserAmmo;
        characterData.equippedLaserAmmo = Ship.instanceOfClient.laserTierBeingUsed;
        characterData.weaponInventory = Ship.instanceOfClient.weaponInventory;

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