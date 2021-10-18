using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour
{
    [SerializeField]
    private GameObject _animationRoot;
    [SerializeField]
    private float _zoomRatio = 1f;
    [SerializeField]
    protected float _recoilAmount = 3f;
    [SerializeField]
    protected float _recoilSpeed = 20f;
    [SerializeField]
    protected float _hipfireBloom = 1.5f;
    [SerializeField]
    protected Animator _animator;
    [SerializeField]
    protected float _range = 50f;
    [SerializeField]
    protected float _delayBeforeRayCase = 0.1f;

    private bool _isWeaponActive;

    public float ZoomRatio
    {
        get
        {
            return _zoomRatio;
        }
    }

    public float RecoilAmount
    {
        get
        {
            return _recoilAmount;
        }
    }

    public float HipfireBloom
    {
        get
        {
            return _hipfireBloom;
        }
    }

    public float RecoilSpeed
    {
        get
        {
            return _recoilSpeed;
        }
    }

    public float Range
    {
        get
        {
            return _range;
        }
    }

    public float DelayBeforeRayCast
    {
        get
        {
            return _delayBeforeRayCase;
        }
    }

    public void SetVisibility(bool visible)
    {
        _isWeaponActive = visible;
    }

    public virtual void Update()
    {
        _animationRoot.SetActive(_isWeaponActive);
    }

    //We'll make this virtual so that derived classes can override this if they have differently configured animations
    public virtual void PlayShootAnimation()
    {
        _animator.SetTrigger("Shoot");
    }
}
