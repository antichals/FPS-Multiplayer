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

    protected Animator _animator;
    protected NetworkAnimator _networkAnimator;
    [SerializeField] protected AnimatorOverrideController _overrideController;


    protected virtual void Awake()
    {
        _cameraTransform = transform.parent;
        _animator = GetComponent<Animator>();
        _networkAnimator = GetComponent<NetworkAnimator>();

        // Change animation depending on weapon
        if( _overrideController != null )
            _animator.runtimeAnimatorController = _overrideController;
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
            Instantiate(bloodParticles, hit.point, Quaternion.LookRotation(hit.normal)).Play();

            // Call local TakeDame (will call server -> local hit player)
            health.TakeDamage(damage);
        }
        else
        {
            Debug.Log("Terrain Hit");
            // terrain hit
            Instantiate(terrainHitParticles, hit.point, Quaternion.LookRotation(hit.normal)).Play() ;
        }
    }

    [ServerRpc]
    public virtual void ServerPlayWeaponParticleEffects() 
    {
        // Play particle effect locally on server
        muzzleFlashParticles.Play();

        // Play effect on clients
        ObserversPlayWeaponParticleEffects();
    }

    [ObserversRpc]
    private void ObserversPlayWeaponParticleEffects()
    {
        Debug.Log("Playing Muzzle Flash");
        // Play particle effect in all clients
        muzzleFlashParticles.Play();
    }
}
