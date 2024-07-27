using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockStats", menuName = "Stats/BlockStats", order = 0)]
public class BlockStats : ScriptableObject
{
    [field: SerializeField] public int BlockMaxHp { get; private set; }
    [field: SerializeField] public int ScoreGiven { get; private set; }
    [field: SerializeField] public bool IsBreakable { get; private set; }
    [field: SerializeField] public Vector2[] BlockColorLowUVs { get; private set; }
    [field: SerializeField] public Vector2[] BlockColorTopUVs { get; private set; }
}
