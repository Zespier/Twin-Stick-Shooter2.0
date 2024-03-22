using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spore_Zone : Generator {

    public override GameObject SporePrefab { get => Spore_Mind.instance.spore_Small; }

    private void OnEnable() {
        StartCoroutine(C_WaitForSpore_MindInstantiation(1, result => { Spore_Mind.instance.totalSpore_Zones += result; }));
    }

    private void OnDisable() {
        if (Spore_Mind.instance == null) {
            Debug.LogError("The spore died without a Spore mind");
            Spore_Mind.instance.totalSpore_Zones--;
        }
    }

    /// <summary>
    /// Waits for spoew mind to be alive
    /// </summary>
    /// <param name="value"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private IEnumerator C_WaitForSpore_MindInstantiation(int value, Action<int> action) {

        while (Spore_Mind.instance == null) {
            yield return null;
        }

        action?.Invoke(value);
    }

}
