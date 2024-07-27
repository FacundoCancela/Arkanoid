using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Ball : CustomMonoBehaviour, IPoolable
{
    public static Action<int> OnLastBallOutOfBoundsDelegate;
    public GameObject GameObject => gameObject;

    private Transform ballAnchorPoint;
    private bool isAttached;
    private Vector2 currentDir;

    private static float originalBallSpeed;
    [SerializeField] private static float speed = 20;

    private static Vector3 sharedBallPosition;

    private Vector2 colorUV = new Vector2(0.41f, 1.0f);
    private MeshRenderer _mesh;
    private MaterialPropertyBlock _materialPropertyBlock;

    [SerializeField] private AudioClip onBlockHitSFX;
    [SerializeField] private AudioClip onLaunchBallSFX;
    [SerializeField] private AudioClip onPaddleHitSFX;
    [SerializeField] private AudioClip onLastBallOutOfBoundsSFX;

    private void Awake()
    {
        speed = 20;
        originalBallSpeed = speed;
        isAttached = true;
        ballAnchorPoint = GameObject.FindGameObjectWithTag("BallAnchorPoint").GetComponent<Transform>();

        _mesh = GetComponent<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();

        _materialPropertyBlock.SetVector("_UV_Low", colorUV);
        _materialPropertyBlock.SetVector("_UV_Top", colorUV);
        _mesh.SetPropertyBlock(_materialPropertyBlock);
    }

    public override void CustomUpdate()
    {
        if (isAttached)
        {
            gameObject.transform.position = ballAnchorPoint.position;
        }

        sharedBallPosition = this.transform.position;
        CheckBallOutOfBoundaries();
    }

    public override void CustomFixedUpdate()
    {
        CustomPhysics.UniformLineMovement(this.gameObject, speed, currentDir.normalized);
    }
    private void ReduceBallSpeed(bool bReduceBallSpeed)
    {
        if (bReduceBallSpeed)
        {
            speed *= 0.5f;
        }
        else speed = originalBallSpeed;
    }

    public void OnPoolableObjectEnable()
    {
        if (CustomUpdateManager.Instance.DoesListAlreadyContainObject(this) == false)
        {
            CustomUpdateManager.Instance.AddRuntimeObjectToList(this);
            GameManager.Instance.ActivePaddleController.OnShootInputPressedDelegate += LaunchBall;
            GameManager.Instance.ActivePaddleController.PaddleModel.OnBallSpeedPowerUpStatusChangedDelegate += ReduceBallSpeed;
        }

        //-Ya hay una bola en escena entonces MultiBall
        if (GameManager.Instance.GetNumberOfActiveBallsInScene() > 1)
        {
            gameObject.transform.position = sharedBallPosition;
            ApplyRandomBallImpulseUp();
        }
        else
        {
            isAttached = true;
            gameObject.transform.position = ballAnchorPoint.position;
        }

        gameObject.SetActive(true);
    }

    public void OnPoolableObjectDisable()
    {
        //Si es la ultima bola
        if (GameManager.Instance.GetNumberOfActiveBallsInScene() <= 1)
        {
            OnPoolableObjectEnable();
            OnLastBallOutOfBoundsDelegate?.Invoke(-1);
            AudioManager.Instance.PlayOneShot(onLastBallOutOfBoundsSFX);
        }
        else
        {
            CustomUpdateManager.Instance.RemoveRuntimeObjectFromList(this);
            GameManager.Instance.ActivePaddleController.OnShootInputPressedDelegate -= LaunchBall;
            GameManager.Instance.ActivePaddleController.PaddleModel.OnBallSpeedPowerUpStatusChangedDelegate -= ReduceBallSpeed;
            gameObject.SetActive(false);
        }
    }

    private void LaunchBall()
    {
        //if is mainball
        if (isAttached)
        {
            isAttached = false;
            ApplyRandomBallImpulseUp();
            AudioManager.Instance.PlayOneShot(onLaunchBallSFX);
        }
    }
    private void ApplyRandomBallImpulseUp()
    {
        isAttached = false;
        currentDir.y = 1;
        SetRandomBallDirX();
    }

    private void SetRandomBallDirX()
    {
        char last = gameObject.name[gameObject.name.Length - 1];
        Random.InitState(System.DateTime.Now.Millisecond + int.Parse(last.ToString()));

        float randomNum = Random.Range(0.0f, 1.0f);

        if (randomNum < 0.5f)
        {
            currentDir.x = 0.25f + randomNum;
        }
        else
        {
            currentDir.x = -0.25f - randomNum;
        }
    }

    private void CheckBallOutOfBoundaries()
    {
        if (!isAttached && this.transform.position.y < WorldConstants.KILL_Y)
        {
            if(!GameManager.Instance.HasGameEnded)
            {
                OnPoolableObjectDisable();
            }
        }
    }

    public void CollisionEffectWithMovingObject(Vector2 collisionOriginDirection, Vector2 collidedObjPos, Vector2 collisionHitPoint)
    {
        if (collisionHitPoint.x < collidedObjPos.x)
        {
            currentDir.x = -1;
        }
        else
        {
            currentDir.x = 1;
        }
        
        if (collisionOriginDirection.y > 0)
        {
            currentDir.y = 1;
        }
        else if (collisionOriginDirection.y < 0)
        {
            currentDir.y = -1;
        }

        AudioManager.Instance.PlayOneShot(onPaddleHitSFX);
    }

    public void CollisionEffectWithStaticObject(Vector2 collisionOriginDirection)
    {
        if (collisionOriginDirection.x != 0)
        {
            if (currentDir.x > 0)
                currentDir.x *= collisionOriginDirection.x;
            else if (currentDir.x < 0)
                currentDir.x *= -collisionOriginDirection.x;
            else
            {
                currentDir.x = collisionOriginDirection.x;
            }
        }

        if (collisionOriginDirection.y > 0)
        {
            currentDir.y = 1;
        }
        else if (collisionOriginDirection.y < 0)
        {
            currentDir.y = -1;
        }

        AudioManager.Instance.PlayOneShot(onBlockHitSFX);
    }
}
