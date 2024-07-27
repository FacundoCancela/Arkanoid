using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomCollisionManager : CustomMonoBehaviour
{
    public static CustomCollisionManager Instance
    {
        get { return instance; }
    }
    private static CustomCollisionManager instance;

    [SerializeField] private BoxCollider paddle;
    [SerializeField] private List<BoxCollider> objectList;

    private Dictionary<BoxCollider, Block> staticBlocksDictionary;
    private Dictionary<PowerUp, BoxCollider> runtimePowerUpsDictionary;
    private Dictionary<Projectile, BoxCollider> runtimeProjectilesDictionary;
    private Dictionary<Enemy, BoxCollider> runtimeEnemiesDictionary;
    private Dictionary<Ball, SphereCollider> runtimeBallsDictionary;

    public override void CustomAwake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        staticBlocksDictionary = new Dictionary<BoxCollider, Block>();
        runtimePowerUpsDictionary = new Dictionary<PowerUp, BoxCollider>();
        runtimeProjectilesDictionary = new Dictionary<Projectile, BoxCollider>();
        runtimeEnemiesDictionary = new Dictionary<Enemy, BoxCollider>();
        runtimeBallsDictionary = new Dictionary<Ball, SphereCollider>();
        
        for (int i = 0; i < objectList.Count; i++)
        {
            staticBlocksDictionary.Add(objectList[i], objectList[i].GetComponent<Block>());
        }
    }

    public override void CustomFixedUpdate()
    {

        //--Balls--
        foreach (KeyValuePair<Ball, SphereCollider> ball in runtimeBallsDictionary.ToList())
        {
            //--Paddle--
            if (CustomPhysics.CollisionSphereToRectangle(paddle, ball.Value))
            {
                ball.Key.CollisionEffectWithMovingObject(CustomPhysics.GetDistanceVectorDirection(paddle, ball.Value),
                    paddle.transform.position, CustomPhysics.GetCollisionHitPoint(ball.Value.transform, paddle.transform));
            }

            //--Blocks---
            foreach (KeyValuePair<BoxCollider, Block> block in staticBlocksDictionary)
            {
                //El destroy del bloque lo desactiva de la escena - no tiene sentido seguir revisando un bloque que ya no esta en uso
                if (block.Value.gameObject.activeSelf && CustomPhysics.CollisionSphereToRectangle(block.Key, ball.Value))
                {
                    if (block.Value.gameObject.CompareTag("Block"))
                    {
                        ball.Key.CollisionEffectWithStaticObject(CustomPhysics.GetDistanceVectorDirection(block.Key, ball.Value));
                        block.Value.OnHit();
                    }
                    else if (block.Value.gameObject.CompareTag("LevelLimit"))
                    {
                        ball.Key.CollisionEffectWithStaticObject(CustomPhysics.GetDistanceVectorDirection(block.Key, ball.Value));
                    }
                }
            }

            //--PowerUps---
            foreach (KeyValuePair<PowerUp, BoxCollider> powerUp in runtimePowerUpsDictionary.ToList())
            {
                if (CustomPhysics.CollisionRectangleToRectangle(paddle, powerUp.Value))
                {
                    powerUp.Key.Consume();
                }
            }
        }

        //--Projectiles
        foreach (KeyValuePair<Projectile, BoxCollider> projectile in runtimeProjectilesDictionary.ToList())
        {
            //--Blocks---
            foreach (KeyValuePair<BoxCollider, Block> block in staticBlocksDictionary)
            {
                //El destroy del bloque lo desactiva de la escena - no tiene sentido seguir revisando un bloque que ya no esta en uso
                if (block.Value.gameObject.activeSelf && (CustomPhysics.CollisionRectangleToRectangle(projectile.Value, block.Key)))
                {
                    if (block.Value.gameObject.CompareTag("Block"))
                    {
                        block.Value.OnHit();
                        projectile.Key.OnProjectileCollisionWithBlock();
                    }
                }
            }
        }

        //--Enemies
        foreach (KeyValuePair<Enemy, BoxCollider> enemy in runtimeEnemiesDictionary.ToList())
        {
            //--Paddle--
            if (CustomPhysics.CollisionRectangleToRectangle(enemy.Value, paddle))
            {
                enemy.Key.OnCollisionWithPlayerPaddle();
            }

            //--Projectiles
            foreach (KeyValuePair<Projectile, BoxCollider> projectile in runtimeProjectilesDictionary.ToList())
            {
                if (CustomPhysics.CollisionRectangleToRectangle(enemy.Value, projectile.Value))
                {
                    enemy.Key.Die();
                    projectile.Key.OnProjectileCollisionWithEnemy();
                }
            }

            //--Balls--
            foreach (KeyValuePair<Ball, SphereCollider> ball in runtimeBallsDictionary.ToList())
            {
                if (CustomPhysics.CollisionSphereToRectangle(enemy.Value, ball.Value))
                {
                    enemy.Key.Die();
                }
            }
        }
    }

    public void AddRuntimeBallToCollisionList(Ball ball, SphereCollider collider)
    {
        if (runtimeBallsDictionary.ContainsKey(ball) == false)
            runtimeBallsDictionary.Add(ball, collider);
    }
    public void RemoveRuntimeBallFromCollisionList(Ball ball)
    {
        runtimeBallsDictionary.Remove(ball);
    }

    public void AddRuntimePowerUpToCollisionList(PowerUp powerUp, BoxCollider collider)
    {
        if (runtimePowerUpsDictionary.ContainsKey(powerUp) == false)
            runtimePowerUpsDictionary.Add(powerUp, collider);
    }
    public void RemoveRuntimePowerUpFromCollisionList(PowerUp powerUp)
    {
        runtimePowerUpsDictionary.Remove(powerUp);
    }
    public void AddRuntimeEnemyToCollisionList(Enemy enemy, BoxCollider collider)
    {
        if (runtimeEnemiesDictionary.ContainsKey(enemy) == false)
            runtimeEnemiesDictionary.Add(enemy, collider);
    }
    public void RemoveRuntimeEnemyFromCollisionList(Enemy enemy)
    {
        runtimeEnemiesDictionary.Remove(enemy);
    }

    public void AddRuntimeProjectileToCollisionList(Projectile projectile, BoxCollider collider)
    {
        if (runtimeProjectilesDictionary.ContainsKey(projectile) == false)
            runtimeProjectilesDictionary.Add(projectile, collider);
    }
    public void RemoveRuntimeProjectileFromCollisionList(Projectile projectile)
    {
        runtimeProjectilesDictionary.Remove(projectile);
    }


}