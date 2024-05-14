using System.Collections;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;



namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] private WeaponConfig weapon = null;
        [SerializeField] private float respawnTime = 5;
        [SerializeField] private float healthBoost = 0;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        
        }

        private void Pickup(GameObject other)
        {
            if (weapon != null)
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);

            }

            if (healthBoost >= 0)
            {
                other.GetComponent<Health>().Heal(healthBoost);
            }
            StartCoroutine(HideForSeconds(respawnTime));
        }

        IEnumerator HideForSeconds(float seconds)
        {
            HidePickup();
            yield return new WaitForSeconds(seconds);
            ShowPickup();
        }

        private void ShowPickup()
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = true;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }        
        }

        private void HidePickup()
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }

        public bool HandleRaycast(PlayerController caller)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(caller.gameObject);
            }

            return true;
        }

        public Cursors GetCursorType()
        {
            return Cursors.Pickup;
        }
    }
}

