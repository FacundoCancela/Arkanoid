using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Stats/EnemyStats", order = 0)]
public class EnemyStats : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public int ScoreGiven { get; private set; }
    [field: SerializeField] public int DamageToPlayer { get; private set; }
    [field: SerializeField] public Vector2 EnemyColorUVs { get; private set; }
}
