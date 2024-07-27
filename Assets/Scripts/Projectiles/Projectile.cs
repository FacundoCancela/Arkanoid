using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Projectile : CustomMonoBehaviour, IPoolable
{
    public GameObject GameObject => this.gameObject;

    private MeshRenderer _mesh;
    private MaterialPropertyBlock _materialPropertyBlock;

    [SerializeField] private ProjectileStats stats;
    [SerializeField] private float lifeTime = 3.0f;
    private float originalLifeTime = 0.0f;
    private Transform shootStartPoint;

    [SerializeField] private AudioClip onBlockHitSFX;
    [SerializeField] private AudioClip onShotSFX;

    private void Awake()
    {
        originalLifeTime = lifeTime;
        _mesh = GetComponent<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();

        _materialPropertyBlock.SetVector("_UV_Low", stats.ColorUVs);
        _materialPropertyBlock.SetVector("_UV_Top", stats.ColorUVs);
        _mesh.SetPropertyBlock(_materialPropertyBlock);

        shootStartPoint = GameObject.FindGameObjectWithTag("BallAnchorPoint").GetComponent<Transform>();
    }

    public override void CustomUpdate()
    {
        Travel();

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            OnPoolableObjectDisable();
        }
    }
    public void OnProjectileCollisionWithBlock()
    {
        OnPoolableObjectDisable();
        AudioManager.Instance.PlayOneShot(onBlockHitSFX);
    }
    public void OnProjectileCollisionWithEnemy()
    {
        OnPoolableObjectDisable();
        AudioManager.Instance.PlayOneShot(onBlockHitSFX);
    }
    private void Travel()
    {
        transform.position += transform.up * (Time.deltaTime * stats.Speed);
    }

    public void OnPoolableObjectEnable()
    {
        AudioManager.Instance.PlayOneShot(onShotSFX);
        CustomUpdateManager.Instance.AddRuntimeObjectToList(this);
        lifeTime = originalLifeTime;
        gameObject.transform.position = shootStartPoint.position;
        gameObject.SetActive(true);
    }
    public void OnPoolableObjectDisable()
    {
        CustomUpdateManager.Instance.RemoveRuntimeObjectFromList(this);
        gameObject.SetActive(false);
    }
}
