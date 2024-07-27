using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Enemy : CustomMonoBehaviour, IPoolable
{
    public GameObject GameObject => this.gameObject;
    public static Action<int> OnCollisionWithPlayerPaddleDelegate;

    [SerializeField] private float lifeTime = 10.0f;
    private float originalLifeTime = 0.0f;
    [SerializeField] private EnemyStats stats;

    private MeshRenderer _mesh;
    private MaterialPropertyBlock _materialPropertyBlock;

    private Vector3 spawnPoint;

    [SerializeField] private AudioClip OnDeathSFX;
    [SerializeField] private AudioClip OnCollisionWithPlayerSFX;


    private void Awake()
    {
        originalLifeTime = lifeTime;
        _mesh = GetComponent<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();

        _materialPropertyBlock.SetVector("_UV_Low", stats.EnemyColorUVs);
        _materialPropertyBlock.SetVector("_UV_Top", stats.EnemyColorUVs);
        _mesh.SetPropertyBlock(_materialPropertyBlock);
    }

    public override void CustomUpdate()
    {
        Move();

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            OnPoolableObjectDisable();
        }

        if (transform.position.y <= WorldConstants.KILL_Y)
        {
            OnPoolableObjectDisable();
        }
    }

    public void Move()
    {
        transform.position += stats.Speed * Time.deltaTime * Vector3.down;
    }
    public void Die()
    {
        GameManager.Instance.UpdateGameScore(stats.ScoreGiven);
        AudioManager.Instance.PlayOneShot(OnDeathSFX);
        OnPoolableObjectDisable();
    }
    public void OnCollisionWithPlayerPaddle()
    {
        AudioManager.Instance.PlayOneShot(OnCollisionWithPlayerSFX);
        OnCollisionWithPlayerPaddleDelegate?.Invoke(-stats.DamageToPlayer);
        OnPoolableObjectDisable();
    }

    public void OnPoolableObjectEnable()
    {
        lifeTime = originalLifeTime;

        float rndX = UnityEngine.Random.Range(-10.0f, 10.0f);
        spawnPoint = new Vector3(rndX, 0.0f, 0.0f);

        gameObject.transform.localPosition = spawnPoint;

        CustomUpdateManager.Instance.AddRuntimeObjectToList(this);
        gameObject.SetActive(true);
    }

    public void OnPoolableObjectDisable()
    {
        CustomUpdateManager.Instance.RemoveRuntimeObjectFromList(this);
        gameObject.SetActive(false);
    }
}
