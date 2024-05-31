using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HurtResult
{
    public bool succeed;
    public bool isCounter;
    public float shakeFactor;
}

public struct HurtParam
{
    public float damage;
    public int knockForce;
    public Vector3 dir;
}

public abstract class HurtableObject : MonoBehaviour
{

    [SerializeField]
    private Collider _mainCollider;

    public Collider mainCollider { get => _mainCollider; }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(gameObject.layer == LayerMask.NameToLayer("Hurtable"));
        if (_mainCollider == null)
        {
            Debug.LogError("Warning! mainCollider of this hurtable object hasn't been set!");
            _mainCollider = gameObject.GetComponent<Collider>();
            Debug.Assert(mainCollider != null);
        }
    }

    public abstract HurtResult Hurt(HurtParam param);
}
