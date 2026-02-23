using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class CharacterSaveManager {
    private static string SaveFolder => Application.persistentDataPath + "/Characters/";

    public static void SaveCharacter(CharacterData data) {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveFolder + data.characterId + ".json", json);
    }

    public static CharacterData LoadCharacter(string id) {
        string path = SaveFolder + id + ".json";
        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<CharacterData>(json);
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

    public int level;
    public int experience;

    public int materials;
    public List<string> ownedWeapons;
    public List<string> ownedShips;

    public string equippedShip;
    public string equippedWeapon;

    public long lastSaveTime;
}