using System.Collections;
using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using JetBrains.Annotations;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public int damage;
    public float maxRange = 20f;
    public float fireRate = 0.5f;
    private float _lastFireTime;

    public LayerMask weaponHitLayer;
    private Transform _cameraTransform;

    public ParticleSystem muzzleFlash;
    protected Animator _animator;
    protected NetworkAnimator _networkAnimator;
    [SerializeField] protected AnimatorOverrideController _overrideController;

    private int _fireId = 1;

    protected virtual void Awake()
    {
        _cameraTransform = transform.parent;
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();
        if( _overrideController != null )
            _animator.runtimeAnimatorController = _overrideController;
    }

    public void Fire()
    {

        if (Time.time < _lastFireTime + fireRate)
            return;

        Debug.Log("Weapon Fired");
        _lastFireTime = Time.time;

        // Play weapon animations
        //_networkAnimator.ResetTrigger("Fire");

        _networkAnimator.ResetTrigger("Fire");
        _networkAnimator.SetTrigger("Fire");

        // Play weapon fire effect
        ServerPlayWeaponAnimation();

        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, maxRange))
        {
            Debug.Log("[Weapon.Fire] Nothing Hit");
            return;
        }

        // If weapon hits player
        if (hit.transform.TryGetComponent(out PlayerHealth health))
        {
            Debug.Log("[Weapon.Fire] Hit a player!");
            health.TakeDamage(damage);
        }
    }


    [ServerRpc]
    public virtual void ServerPlayWeaponAnimation() 
    {
        // Play locally on server
        muzzleFlash.Play();
        

        //_networkAnimator.ResetTrigger("Fire");
        //_networkAnimator.SetTrigger("Fire");

        // Then notify observers to run it too
        ObserversPlayAnimation();
    }

    [ObserversRpc]
    private void ObserversPlayAnimation()
    {
        // Display muzzle flash
        muzzleFlash.Play();



        //_animator.SetTrigger("Fire");

        Debug.Log("Playing weapon animation");
        // Play weapon animation
        //_networkAnimator.ResetTrigger("Fire");
        //_networkAnimator.SetTrigger("Fire");
    }
}
