using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class IOData {

    public static void Delete(string path) {
        path = Application.persistentDataPath + "/" + path;

        File.Delete(path);
    }

    public static object Load(string path) {

        object obj;
        var binf = new BinaryFormatter();
        path = Application.persistentDataPath + "/" + path;

        if (File.Exists(path)) {

            FileStream file = File.Open(path, FileMode.Open);

            if (file.Length <= 0) { return null; } //If empty

            obj = binf.Deserialize(file);
            file.Close();
            return obj;


        } else {
            return null;
        }
    }

    public static void Save(string path, object savingObject) {

        var binf = new BinaryFormatter();


        path = Application.persistentDataPath + "/" + path;

        if (!File.Exists(path)) {

            FileStream file = File.Create(path);
            binf.Serialize(file, savingObject);
            file.Close();

        } else {
            FileStream file = File.Open(path, FileMode.Open);
            binf.Serialize(file, savingObject);
            file.Close();
        }
    }

}

