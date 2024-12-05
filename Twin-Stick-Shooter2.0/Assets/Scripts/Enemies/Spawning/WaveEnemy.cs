
using UnityEngine;

[CreateAssetMenu(fileName = "WaveEnemy", menuName = "Waves/WaveEnemy")]
public class WaveEnemy : ScriptableObject {
    public EnemyType type;
    public bool spawned;
    public float healthRemaining = 100;

    [ContextMenu("Reset Variables")]
    public void ResetVariables() {
        spawned = false;
        healthRemaining = 100;
    }
}
