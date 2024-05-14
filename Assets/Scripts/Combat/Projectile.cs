using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using Unity.VisualScripting;
using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;


namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        private Health target = null;
        [SerializeField] float speed = 1f;
        [SerializeField] private bool isHoming = true;
        private float damage = 0;
        [SerializeField] private GameObject hitEffect = null;
        [SerializeField] private float maxLifeTime = 10f;
        [SerializeField] private GameObject[] destroyOnHit = null;
        [SerializeField] private float lifeAfterImpact = 2f;
        [SerializeField] private UnityEvent onHit;
        private GameObject instigator = null;


        private void Start()
        {
            transform.LookAt(GetAimLocation());

        }

        void Update()
        {
            if (target == null) return;
            if (isHoming && target.IsAlive())
            {
                transform.LookAt(GetAimLocation());

            }
            transform.Translate(Vector3.forward* speed * Time.deltaTime);
            
            
        }

        Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * targetCapsule.height/2;
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            Destroy(gameObject, maxLifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;
            if(!target.IsAlive()) return;
            target.TakeDamage(instigator, damage);
            //speed = 0;
            onHit.Invoke();
            if (hitEffect!= null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)
            {
                Destroy(toDestroy);
            }
            
            Destroy(gameObject, lifeAfterImpact);
        }
    }
}

