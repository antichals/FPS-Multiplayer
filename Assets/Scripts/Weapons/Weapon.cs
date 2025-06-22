using System.Collections.Generic;
using FishNet.Component.Animating;
using FishNet.Object;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public int damage;
    public float maxRange = 20f;
    public float fireRate = 0.5f;
    private float _lastFireTime;

    public LayerMask weaponHitLayer;
    private Transform _cameraTransform;

    public ParticleSystem muzzleFlashParticles;
    public ParticleSystem bloodParticles;
    public ParticleSystem terrainHitParticles;
    private Dictionary<HitEffectType, ParticleSystem> particlesMap;

    protected Animator _animator;
    protected NetworkAnimator _networkAnimator;
    [SerializeField] protected AnimatorOverrideController _overrideController;

    public enum HitEffectType
    {
        Terrain, 
        Blood
    }

    protected virtual void Awake()
    {
        _cameraTransform = transform.parent;
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();

        // Change animation depending on weapon
        if( _overrideController != null )
            _animator.runtimeAnimatorController = _overrideController;

        // We need this in order to pass the information to client and server on what we are hitting
        particlesMap = new Dictionary<HitEffectType, ParticleSystem>
        { 
            { HitEffectType.Terrain, terrainHitParticles },
            { HitEffectType.Blood, bloodParticles }
            // Add more as needed
        };

        // TODO dynamic animation speed depending on fireRate
        //_animator.speed = 1 / fireRate;
    }

    public void Fire()
    {
        // Check firerate
        if (Time.time < _lastFireTime + fireRate)
            return;

        Debug.Log("Weapon Fired");
        _lastFireTime = Time.time;

        // Play weapon animations
        _networkAnimator.ResetTrigger("Fire");
        _networkAnimator.SetTrigger("Fire");

        // Play weapon particle effect
        ServerPlayWeaponParticleEffects();

        // If weapon does NOT hit anything do nothing
        if (!Physics.Raycast(_cameraTransform.position, _cameraTransform.forward, out RaycastHit hit, maxRange))
        {
            Debug.Log("[Weapon.Fire] Nothing Hit");
            return;
        }

        // If weapon hits player
        if (hit.transform.TryGetComponent(out PlayerHealth health))
        {
            Debug.Log("[Weapon.Fire] Hit a player!");

            // Play blood particle effect
            ServerPlayHitParticleEffect(HitEffectType.Blood, hit.point, hit.normal);

            // Call local TakeDame (will call server -> local hit player)
            health.TakeDamage(damage, OwnerId);
        }
        else
        {
            Debug.Log("Terrain Hit");
            // terrain hit
            ServerPlayHitParticleEffect(HitEffectType.Terrain, hit.point, hit.normal);
            
        }
    }

    [ServerRpc]
    private void ServerPlayHitParticleEffect(HitEffectType type, Vector3 point, Vector3 normal)
    {
        // Play effect on server
        Instantiate(particlesMap[type], point, Quaternion.LookRotation(normal)).Play();
        
        // Send info to clients
        ObserversPlayHitParticleEffect(type, point, normal);
    }

    [ObserversRpc]
    private void ObserversPlayHitParticleEffect(HitEffectType type, Vector3 point, Vector3 normal)
    {
        // Play effect on clients
        Instantiate(particlesMap[type], point, Quaternion.LookRotation(normal)).Play();
    }

    [ServerRpc]
    private void ServerPlayWeaponParticleEffects() 
    {
        // Play particle effect locally on server
        muzzleFlashParticles.Play();

        // Play effect on clients
        ObserversPlayWeaponParticleEffects();
    }

    [ObserversRpc]
    private void ObserversPlayWeaponParticleEffects()
    {
        // Play particle effect in all clients
        muzzleFlashParticles.Play();
    }
}
