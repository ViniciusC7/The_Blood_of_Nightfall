// AmmoPickup.cs
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 16; // 2 pentes

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponController weapon = other.GetComponentInChildren<WeaponController>();
            if (weapon != null)
            {
                weapon.AddAmmo(ammoAmount);
                Destroy(gameObject); // Destroi o item após a coleta
            }
        }
    }
}