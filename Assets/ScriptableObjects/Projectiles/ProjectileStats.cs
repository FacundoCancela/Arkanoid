using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileStats", menuName = "Stats/ProjectileStats", order = 0)]
public class ProjectileStats : ScriptableObject
{
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public Vector2 ColorUVs { get; private set; }
}
