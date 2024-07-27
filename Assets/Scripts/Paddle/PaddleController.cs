using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : CustomMonoBehaviour
{
    public PaddleModel PaddleModel => _paddleModel;
    public Action OnShootInputPressedDelegate;
    public Action OnPauseInputPressedDelegate;

    private PaddleModel _paddleModel;

    private PoolableFactory laserFactory;
    private float laserInputCooldownTimer = 0.0f;
    [SerializeField] private float laserFireRate = 0.5f;
    [SerializeField] private Projectile laserPrefab;
    [SerializeField] private ObjectPool shootableLasersPool;

    public override void CustomAwake()
    {
        _paddleModel = GetComponent<PaddleModel>();
        laserFactory = new PoolableFactory(shootableLasersPool, laserPrefab, shootableLasersPool.PoolMaxSize);
    }

    public override void CustomUpdate()
    {
        if (!GameManager.Instance.HasGameEnded)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                _paddleModel.Move(Input.GetAxis("Horizontal"));
                ClampPaddleMoveToScreen();
            }

            laserInputCooldownTimer += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnShootInputPressedDelegate?.Invoke();

                if (_paddleModel.PaddleCanShootLasers && laserInputCooldownTimer >= laserFireRate)
                {
                    laserFactory.TryCreateObject();
                    laserInputCooldownTimer = 0;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnPauseInputPressedDelegate?.Invoke();
            }

        }
    }

    private void ClampPaddleMoveToScreen()
    {
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp(pos.x, 0.245f, 0.755f); //Valores arbitrarios
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
}
