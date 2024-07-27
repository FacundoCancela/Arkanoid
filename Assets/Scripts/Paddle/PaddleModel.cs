using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Serialization;
using static PowerUp;

public class PaddleModel : CustomMonoBehaviour
{
    public Action<int> OnLifeChangedDelegate;
    public Action<bool> OnBallSpeedPowerUpStatusChangedDelegate;

    [SerializeField] private float speed;

    private Vector3 originalSize;
    private Vector3 paddleSizeWhenLarge;

    [SerializeField] private int startHealth = 3;
    private int currentHealth = 0;
    private const int MAX_HEALTH = 5;

    public ObjectPool BallPool => ballPool;
    private PoolableFactory ballFactory;
    [SerializeField] private Ball paddleBallPrefab;
    [SerializeField] private ObjectPool ballPool;

    public bool PaddleCanShootLasers => bCanShootLasers;
    private bool bCanShootLasers = false;

    private float enlargeRemainingTime = 0;
    private float slowerBallRemainingTime = 0;
    private float canShootLasersRemainingTime = 0;

    private Vector2 colorUV = new Vector2(0.5f, 1.0f);
    private MeshRenderer _mesh;
    private MaterialPropertyBlock _materialPropertyBlock;
    [SerializeField] private AudioClip onPowerUpPickupSFX;

    public override void CustomStart()
    {
        _mesh = GetComponent<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();

        _materialPropertyBlock.SetVector("_UV_Low", colorUV);
        _materialPropertyBlock.SetVector("_UV_Top", colorUV);
        _mesh.SetPropertyBlock(_materialPropertyBlock);

        ballFactory = new PoolableFactory(ballPool, paddleBallPrefab, ballPool.PoolMaxSize);
        ballFactory.TryCreateObject();

        originalSize = transform.localScale;
        paddleSizeWhenLarge = new Vector3(originalSize.x * 1.5f, originalSize.y, originalSize.z);
        UpdateHealth(startHealth);
    }

    public override void CustomUpdate()
    {
        TrackPowerUpReversal();
    }
    public override void CustomOnEnable()
    {
        PowerUp.OnPowerUpConsume += PowerUpEffect;
        Ball.OnLastBallOutOfBoundsDelegate += UpdateHealth;
        Enemy.OnCollisionWithPlayerPaddleDelegate += UpdateHealth;
    }
    public override void CustomOnDisable()
    {
        PowerUp.OnPowerUpConsume -= PowerUpEffect;
        Ball.OnLastBallOutOfBoundsDelegate -= UpdateHealth;
        Enemy.OnCollisionWithPlayerPaddleDelegate -= UpdateHealth;
    }

    public void Move(float dirX)
    {
        float movement = dirX * speed * Time.deltaTime;
        transform.Translate(movement, 0,0);
    }
    public void UpdateHealth(int newLife)
    {
        if (currentHealth < MAX_HEALTH)
        {
            currentHealth += newLife;
            OnLifeChangedDelegate?.Invoke(currentHealth);
        }
    }

    private void PowerUpEffect(PowerUp.PowerUpType powerUpType, float powerUpDuration)
    {
        switch (powerUpType)
        {
            default:
            case PowerUpType.HPPlus:
                UpdateHealth(1);
                break;
            case PowerUpType.LargerPaddle:
                enlargeRemainingTime = powerUpDuration;
                ChangePaddleSize(paddleSizeWhenLarge);
                break;
            case PowerUpType.MultiBall:
                for (int i = 0; i < ballPool.PoolMaxSize; i++)
                {
                    ballFactory.TryCreateObject();
                }
                break;
            case PowerUpType.ReduceBallSpeed:
                slowerBallRemainingTime = powerUpDuration;
                OnBallSpeedPowerUpStatusChangedDelegate?.Invoke(true);
                break;
            case PowerUpType.Laser:
                canShootLasersRemainingTime = powerUpDuration;
                bCanShootLasers = true;
                break;
        }

        AudioManager.Instance.PlayOneShot(onPowerUpPickupSFX);
    }
    private void ChangePaddleSize(Vector3 paddleSize)
    {
        transform.localScale = paddleSize;
    }

    private void TrackPowerUpReversal()
    {
        if (enlargeRemainingTime > 0)
        {
            enlargeRemainingTime -= Time.deltaTime;

            if (enlargeRemainingTime <= 0)
            {
                ChangePaddleSize(originalSize);
            }
        }
        if (slowerBallRemainingTime > 0)
        {
            slowerBallRemainingTime -= Time.deltaTime;

            if (slowerBallRemainingTime <= 0)
            {
                OnBallSpeedPowerUpStatusChangedDelegate?.Invoke(false);
            }
        }
        if (canShootLasersRemainingTime > 0)
        {
            canShootLasersRemainingTime -= Time.deltaTime;

            if (canShootLasersRemainingTime <= 0)
            {
                bCanShootLasers = false;
            }
        }
    }

}
