using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeContainer : MonoBehaviour {

    public Upgrade upgradePrefab;

    private float _timer;
    private List<Upgrade> _upgrades = new List<Upgrade>();

    public Upgrade TempUpgrade { get; set; }

    public static UpgradeContainer instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }


    // Update is called once per frame
    void Update() {
        if (_upgrades.Count >= 30) { return; }

        if (_timer >= 1) {
            _timer = 0;
            GenerateUpgrade();
        }

        _timer += Time.deltaTime;
    }

    private void GenerateUpgrade() {
        _upgrades.Add(Instantiate(upgradePrefab, transform));
    }

    public Upgrade GetUpgrade() {
        if (_upgrades.Count > 0) {
            TempUpgrade = _upgrades[0];
            _upgrades.RemoveAt(0);
            return TempUpgrade;
        }

        return null;
    }
}
