// WeaponController.cs
using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Referências")]
    public Camera playerCamera;
    public ParticleSystem muzzleFlash;
    public GameObject hitEffectPrefab; // Efeito de impacto no inimigo

    [Header("Atributos da Arma")]
    public float normalDamage = 2f;
    public float headshotDamage = 20f;
    public float fireRate = 0.8f; // Tempo entre tiros, simulando um revólver
    public float range = 100f;

    [Header("Munição e Recarga")]
    public int maxAmmoInClip = 8;
    public int currentAmmoInClip;
    public int totalAmmo = 24;
    public float reloadTime = 5f;
    private bool isReloading = false;
    private float nextTimeToFire = 0f;

    void Start()
    {
        currentAmmoInClip = maxAmmoInClip;
        HUDManager.Instance.UpdateAmmoUI(currentAmmoInClip, totalAmmo);
    }

    void OnEnable()
    {
        isReloading = false; // Garante que não comece recarregando ao reativar
    }

    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmoInClip <= 0 && totalAmmo > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire && currentAmmoInClip > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        muzzleFlash.Play();
        currentAmmoInClip--;
        HUDManager.Instance.UpdateAmmoUI(currentAmmoInClip, totalAmmo);

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
        {
            EnemyAI enemy = hit.transform.GetComponentInParent<EnemyAI>();
            if (enemy != null)
            {
                float damageToDeal = normalDamage;
                // Verifica se o tiro atingiu a cabeça (usando uma tag "Head")
                if (hit.collider.CompareTag("Head"))
                {
                    damageToDeal = headshotDamage;
                    Debug.Log("Headshot!");
                }

                enemy.TakeDamage(damageToDeal);

                // Efeito de impacto
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Recarregando...");
        HUDManager.Instance.ShowReloadingText(true);

        yield return new WaitForSeconds(reloadTime);

        int ammoToReload = maxAmmoInClip - currentAmmoInClip;
        int ammoToTake = Mathf.Min(ammoToReload, totalAmmo);

        currentAmmoInClip += ammoToTake;
        totalAmmo -= ammoToTake;

        HUDManager.Instance.UpdateAmmoUI(currentAmmoInClip, totalAmmo);
        HUDManager.Instance.ShowReloadingText(false);
        isReloading = false;
    }

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        HUDManager.Instance.UpdateAmmoUI(currentAmmoInClip, totalAmmo);
    }

    // --- Funções para Upgrades ---
    public void IncreaseDamage(float multiplier)
    {
        normalDamage *= multiplier;
        headshotDamage *= multiplier;
    }

    public void IncreaseAmmoCapacity(int amount)
    {
        maxAmmoInClip += amount;
    }
}