using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomUpdateManager : MonoBehaviour
{
    public static CustomUpdateManager Instance
    {
        get { return instance; }
    }
    private static CustomUpdateManager instance;

    [SerializeField] private List<CustomMonoBehaviour> objectList;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].CustomAwake();
        }
    }
    private void Start()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].CustomStart();
        }
    }

    private void Update()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].CustomUpdate();
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].CustomFixedUpdate();
        }
    }
    private void LateUpdate()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].CustomLateUpdate();
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].CustomOnEnable();
        }
    }
    private void OnDisable()
    {
        for (int i = 0; i < objectList.Count; i++)
        {
            objectList[i].CustomOnDisable();
        }
    }

    public void AddRuntimeObjectToList(CustomMonoBehaviour objectToAdd)
    {
        objectList.Add(objectToAdd);

        switch (objectToAdd)
        {
            default:
            case PowerUp:
                CustomCollisionManager.Instance.AddRuntimePowerUpToCollisionList(objectToAdd.GetComponent<PowerUp>(),
                    objectToAdd.GetComponent<BoxCollider>());
                break;
            case Enemy:
                CustomCollisionManager.Instance.AddRuntimeEnemyToCollisionList(objectToAdd.GetComponent<Enemy>(),
                    objectToAdd.GetComponent<BoxCollider>());
                break;
                case Projectile:
                CustomCollisionManager.Instance.AddRuntimeProjectileToCollisionList(objectToAdd.GetComponent<Projectile>(),
                    objectToAdd.GetComponent<BoxCollider>());
                break;
            case Ball:
                CustomCollisionManager.Instance.AddRuntimeBallToCollisionList(objectToAdd.GetComponent<Ball>(),
                    objectToAdd.GetComponent<SphereCollider>());
                break;
        }

    }
    public void RemoveRuntimeObjectFromList(CustomMonoBehaviour objectToRemove)
    {
        objectList.Remove(objectToRemove);

        switch (objectToRemove)
        {
            default:
            case PowerUp:
                CustomCollisionManager.Instance.RemoveRuntimePowerUpFromCollisionList(objectToRemove.GetComponent<PowerUp>());
                break;
            case Enemy:
                CustomCollisionManager.Instance.RemoveRuntimeEnemyFromCollisionList(objectToRemove.GetComponent<Enemy>());
                break;
            case Projectile:
                CustomCollisionManager.Instance.RemoveRuntimeProjectileFromCollisionList(objectToRemove.GetComponent<Projectile>());
                break;
            case Ball:
                CustomCollisionManager.Instance.RemoveRuntimeBallFromCollisionList(objectToRemove.GetComponent<Ball>());
                break;
        }
    }

    public bool DoesListAlreadyContainObject(CustomMonoBehaviour objectToCheck)
    {
        return objectList.Contains(objectToCheck);
    }
}
