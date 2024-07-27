using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class Block : CustomMonoBehaviour
{
    private int _hp;
    private MeshRenderer _mesh;
    private MaterialPropertyBlock _materialPropertyBlock;
    [SerializeField] private BlockStats stats;

    [SerializeField] private PowerUp[] powerUpDrop;
    [Range(0, 100)]
    [SerializeField] private int powerUpDropChance = 0;
    
    public override void CustomAwake()
    {
        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    public override void CustomStart()
    {
        InitializeBlock();
    }

    public void InitializeBlock()
    {
        _mesh = GetComponent<MeshRenderer>();

        if (stats != null)
        {
            _hp = stats.BlockMaxHp;

            _materialPropertyBlock.SetVector("_UV_Low", stats.BlockColorLowUVs[stats.BlockMaxHp - 1]);
            _materialPropertyBlock.SetVector("_UV_Top", stats.BlockColorTopUVs[stats.BlockMaxHp - 1]);
            _mesh.SetPropertyBlock(_materialPropertyBlock);

            //_mesh.material.SetVector("_UV", stats.BlockColorUVs[stats.BlockMaxHp - 1]);
            //_mesh.sharedMaterial.SetVector("_UV", stats.BlockColorUVs[stats.BlockMaxHp - 1]);
        }
    }
    
    public void OnHit()
    {
        if (stats.IsBreakable)
        {
            _hp--;
            if(_hp <= 0)
            {
                DestroyBlock();
            }
            else
            {
                ChangeBlockColor();
            }
        }
    }

    private void ChangeBlockColor()
    {
        _materialPropertyBlock.SetVector("_UV_Low", stats.BlockColorLowUVs[_hp - 1]);
        _materialPropertyBlock.SetVector("_UV_Top", stats.BlockColorTopUVs[_hp - 1]);
        _mesh.SetPropertyBlock(_materialPropertyBlock);

        //_mesh.material.SetVector("_UV", stats.BlockColorUVs[_hp - 1]);
        //_mesh.sharedMaterial.SetVector("_UV", stats.BlockColorUVs[_hp - 1]);
    }
    
    private void DestroyBlock()
    {
        gameObject.SetActive(false);

        GameManager.Instance.UpdateBlocksInSceneCount();
        GameManager.Instance.UpdateGameScore(stats.ScoreGiven);
        SpawnPowerUp();
    }

    private void SpawnPowerUp()
    {
        if (powerUpDrop != null)
        {
            int rnd = Random.Range(0, 100);
            if (rnd <= powerUpDropChance)
            {
                int puSelected = Random.Range(0, powerUpDrop.Length);
                PowerUp spawnedPowerUp = Instantiate(powerUpDrop[puSelected], transform.position, powerUpDrop[puSelected].transform.rotation);
                CustomUpdateManager.Instance.AddRuntimeObjectToList(spawnedPowerUp);
            }
        }
    }
}
