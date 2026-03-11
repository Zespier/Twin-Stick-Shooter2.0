using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletContainer : NetworkBehaviour {

    public GhostBullet ghostBullet1Prefab;
    public GhostBullet ghostBullet2Prefab;
    public GhostBullet ghostBullet3Prefab;
    public GhostBullet ghostBullet4Prefab;
    public GhostBullet ghostBullet5Prefab;
    public GhostBullet enemyGhostBulletPrefab;
    public List<GhostBullet> ghostBulletQueue = new();
    public ServerBullet serverBulletPrefab;
    public List<ServerBullet> serverBulletQueue = new(capacity: 256);

    private ushort _lastId;

    //Each client has it's own container instance, it's not Network-shared.
    public static BulletContainer instance;
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    public void CreateBullet(Vector3 position, Vector3 direction, float speed, bool isEnemyBullet, byte bulletTier = 1) {
        _lastId++;
        if (_lastId > ushort.MaxValue) {
            _lastId = ushort.MinValue;
        }

        Vector2 compressedPosition = new Vector2(position.x, position.z);
        Vector2 compressedDirection = new Vector2(direction.x, direction.z);
        ushort compressedSpeed = (ushort)(speed * 100f);

        CreateServerBulletServerRpc(compressedPosition, compressedDirection, compressedSpeed, _lastId, isEnemyBullet, bulletTier);
        CreateGhostBulletOnAllClientRpc(compressedPosition, compressedDirection, compressedSpeed, _lastId, isEnemyBullet, bulletTier);
    }

    [ServerRpc]
    public void CreateServerBulletServerRpc(Vector2 compressedPosition, Vector2 compressedDirection, ushort compressedSpeed, ushort id, bool isEnemyBullet, byte bulletTier) {

        float speed = compressedSpeed / 100f;
        Vector3 position = new Vector3(compressedPosition.x, 0, compressedPosition.y);
        Vector3 direction = new Vector3(compressedDirection.x, 0, compressedDirection.y);

        int queueCount = serverBulletQueue.Count;
        for (int i = 0; i < queueCount; i++) {

            ServerBullet serverBullet = serverBulletQueue[i];
            if (!serverBullet.gameObject.activeSelf && serverBullet.isEnemyBullet == isEnemyBullet && serverBullet.damageMultiplier == bulletTier) {

                serverBullet.gameObject.SetActive(true);
                serverBullet.id = id;
                serverBullet.transform.position = position;
                serverBullet.transform.forward = direction;
                serverBullet.speed = speed;
                serverBullet.isEnemyBullet = isEnemyBullet;
                serverBullet.damage = isEnemyBullet ? 20 : Ship.instanceOfClient.Stats.Atk * bulletTier;
                serverBullet.damageMultiplier = bulletTier;
                return;
            }
        }

        ServerBullet newServerBullet = Instantiate(serverBulletPrefab, transform);
        newServerBullet.id = id;
        newServerBullet.transform.position = position;
        newServerBullet.transform.forward = direction;
        newServerBullet.speed = speed;
        newServerBullet.isEnemyBullet = isEnemyBullet;
        newServerBullet.damage = isEnemyBullet ? 20 : Ship.instanceOfClient.Stats.Atk;
        newServerBullet.damageMultiplier = bulletTier;
        serverBulletQueue.Add(newServerBullet);
    }

    [ClientRpc]
    public void CreateGhostBulletOnAllClientRpc(Vector2 compressedPosition, Vector2 compressedDirection, ushort compressedSpeed, ushort id, bool isEnemyBullet, byte bulletTier) {

        float speed = compressedSpeed / 100f;
        Vector3 position = new Vector3(compressedPosition.x, 0, compressedPosition.y);
        Vector3 direction = new Vector3(compressedDirection.x, 0, compressedDirection.y);

        int queueCount = ghostBulletQueue.Count;
        for (int i = 0; i < queueCount; i++) {

            GhostBullet ghostBullet = ghostBulletQueue[i];
            if (!ghostBullet.gameObject.activeSelf && ghostBullet.isEnemyBullet == isEnemyBullet && ghostBullet.tier == bulletTier) {

                ghostBullet.gameObject.SetActive(true);
                ghostBullet.id = id;
                ghostBullet.transform.position = position;
                ghostBullet.transform.forward = direction;
                ghostBullet.speed = speed;
                ghostBullet.isEnemyBullet = isEnemyBullet;
                ghostBullet.tier = bulletTier;
                return;
            }
        }

        GhostBullet newGhostBullet;
        if (isEnemyBullet) {
            newGhostBullet = Instantiate(enemyGhostBulletPrefab, transform);

        } else {

            switch (bulletTier) {
                case 1:
                    newGhostBullet = Instantiate(ghostBullet1Prefab, transform);
                    break;
                case 2:
                    newGhostBullet = Instantiate(ghostBullet2Prefab, transform);
                    break;
                case 3:
                    newGhostBullet = Instantiate(ghostBullet3Prefab, transform);
                    break;
                case 4:
                    newGhostBullet = Instantiate(ghostBullet4Prefab, transform);
                    break;
                case 5:
                    newGhostBullet = Instantiate(ghostBullet5Prefab, transform);
                    break;
                default:
                    newGhostBullet = Instantiate(ghostBullet1Prefab, transform);
                    break;
            }
        }
        newGhostBullet.id = id;
        newGhostBullet.transform.position = position;
        newGhostBullet.transform.forward = direction;
        newGhostBullet.speed = speed;
        newGhostBullet.isEnemyBullet = isEnemyBullet;
        newGhostBullet.tier = bulletTier;
        ghostBulletQueue.Add(newGhostBullet);
    }

    [ClientRpc]
    public void DeactivateGhostBulletClientRpc(ushort serverBulletId) {
        int count = ghostBulletQueue.Count;
        for (int i = 0; i < count; i++) {
            if (ghostBulletQueue[i].id == serverBulletId) {
                ghostBulletQueue[i].gameObject.SetActive(false);
                return;
            }
        }
    }
}
