using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class BytesUsedCounter : MonoBehaviour {

    public TMP_Text counterSession;
    public TMP_Text counterMonth;

    public static long bytesUsedThisSession;
    public static long bytesUsedThisMonth;

    private static string SaveFolder => Application.persistentDataPath + "/BytesUsed/";

    private void Awake() {
        BytesUsed bytesUsed = LoadBytes();

        if (bytesUsed != null) {
            bytesUsedThisMonth = bytesUsed.bytesUsedThisMonth;
        }
    }

    private void OnDestroy() {
        SaveBytes(new BytesUsed() {
            dataVersion = 1,
            bytesUsedThisMonth = bytesUsedThisMonth,
        });
    }

    private void Update() {
        counterSession.text = bytesUsedThisSession.ToString();
        counterMonth.text = bytesUsedThisMonth.ToString();

        if (bytesUsedThisMonth > 800_000_000) { // 800 MB
            Debug.LogError("Relay budget exceeded, STOP PLAYING.");
        }
    }

    public static void AddBytesUsed(int bytes) {
        bytes += 40; //~20–40 bytes overhead per message (rough estimate) //I want to avoid paying so I add the big number to get scared sooner
        bytesUsedThisSession += bytes;
        bytesUsedThisMonth += bytes;
    }

    public static void SaveBytes(BytesUsed data) {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveFolder + "BytesUsed" + ".json", json);
    }

    public static BytesUsed LoadBytes() {
        string path = SaveFolder + "BytesUsed" + ".json";
        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        return JsonUtility.FromJson<BytesUsed>(json);
    }
}

[System.Serializable]
public class BytesUsed {
    public int dataVersion = 1;

    public long bytesUsedThisMonth;
}
