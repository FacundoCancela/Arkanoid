using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PowerUp : CustomMonoBehaviour
{
    public static Action<PowerUpType, float> OnPowerUpConsume;

    [SerializeField] private Vector2 powerUpColorLeft;
    [SerializeField] private Vector2 powerUpColorRight;
    private MeshRenderer _mesh;
    private MaterialPropertyBlock _materialPropertyBlock;

    [SerializeField] private float powerUpDuration = 0;
    [SerializeField] private float speed;
    public enum PowerUpType
    {
        HPPlus,
        LargerPaddle,
        MultiBall,
        ReduceBallSpeed,
        Laser
    }
    [SerializeField] private PowerUpType type;

    private void Start()
    {
        _mesh = GetComponent<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();

        _materialPropertyBlock.SetVector("_UV_Low", powerUpColorLeft);
        _materialPropertyBlock.SetVector("_UV_Top", powerUpColorRight);
        _mesh.SetPropertyBlock(_materialPropertyBlock);
    }
    private void OnBecameInvisible()
    {
        CustomUpdateManager.Instance.RemoveRuntimeObjectFromList(this);
        Destroy(this.gameObject);
    }
    public override void CustomFixedUpdate()
    {
        CustomPhysics.UniformLineMovement(this.gameObject, speed, Vector3.left);
    }

    public virtual void Consume()
    {
        OnPowerUpConsume?.Invoke(type, powerUpDuration);

        CustomUpdateManager.Instance.RemoveRuntimeObjectFromList(this);
        Destroy(this.gameObject);
    }
}
