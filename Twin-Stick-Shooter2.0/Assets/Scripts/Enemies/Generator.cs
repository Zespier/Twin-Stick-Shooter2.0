using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public abstract class Generator : MonoBehaviour {

    [SerializeField] private Spore_ZoneSize size;
    public Transform body;
    public CircleCollider2D circleCollider2D;

    private float _defaultBodySize;
    private float _defaultCollider2DRadius;

    protected float _spawnTimer;
    private float _lifetimeTimer;
    private float _sizeRatio = 1f;

    public Spore_ZoneSize Size { get => size; set { size = value; SetSize(); } } //Makes sense that everytime we change the size, it ACTUALLY changes the size
    public abstract GameObject SporePrefab { get; }

    public virtual int NextAmountToGenerate { get => 1; }

    private void Awake() {
        _defaultBodySize = body.localScale.x;
        _defaultCollider2DRadius = circleCollider2D.radius;
    }

    protected virtual void Update() {

        if (_spawnTimer >= (Spore_Mind.instance.spore_Zone_SpawningTime - ReduceSpawnTimeBasedOnSpore_Zones()) / _sizeRatio) {
            Spawn();
        }

        if (_lifetimeTimer >= Spore_Mind.instance.spore_Zone_lifetime) {
            Destroy(gameObject);
        }

        _spawnTimer += Time.deltaTime;
        _lifetimeTimer += Time.deltaTime;
    }

    protected void Spawn() {
        if (Spore_Mind.instance.TotalSpores >= 100) { return; }

        _spawnTimer = 0;
        for (int i = 0; i < NextAmountToGenerate; i++) {

            Spore_Mind.instance.SpawnSpore(SporePrefab, transform.position);
        }
    }

    public float ReduceSpawnTimeBasedOnSpore_Zones() {

        int totalSpawnZones = Spore_Mind.instance.totalSpore_Zones > 30 ? 30 : Spore_Mind.instance.totalSpore_Zones;
        return Mathf.Log(totalSpawnZones, Mathf.Exp(1) + 1);
    }

    public void SetSize() {

        switch (size) {

            case Spore_ZoneSize.small:
                //TODO: Change size
                break;

            case Spore_ZoneSize.medium:
                ProportionalSize(Spore_Mind.instance.spore_Zone_MediumSize);
                break;

            case Spore_ZoneSize.big:
                ProportionalSize(Spore_Mind.instance.spore_Zone_BigSize);
                break;

            default:
                break;
        }
    }

    public void ProportionalSize(float ratio) {
        _sizeRatio = ratio;
        body.localScale = Vector3.one * _defaultBodySize * ratio;
        circleCollider2D.radius = _defaultCollider2DRadius * ratio;
    }
}

public enum Spore_ZoneSize {
    small,
    medium,
    big,
}
