using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using GameDevTV.Utils;
using RPG.Movement;
using RPG.Saving;
using UnityEngine;
using RPG.Attributes;
using RPG.Core;
using RPG.Stats;


namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] private Transform rightHandTransform = null;
        [SerializeField] private Transform leftHandTransform = null;
        [SerializeField] private WeaponConfig defaultWeapon = null;
        private float timeSinceLastAttack = Mathf.Infinity;
        [SerializeField] private string defaultWeaponName = "Unarmed";
        
        Health target;
        WeaponConfig currentWeaponConfig;
         LazyValue<Weapon> currentWeapon;
        private void Awake()
        {
            currentWeaponConfig = defaultWeapon; //new LazyValue<WeaponConfig>(GetDefaultWeapon);
            currentWeapon = new LazyValue<Weapon>(GetDefaultWeapon);
        }
        
        Weapon GetDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }


        private void Start()
        {
            //AttachWeapon(currentWeaponConfig);
            //currentWeaponConfig.ForceInit();
            currentWeapon.ForceInit();
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            if (weapon == null) return;
            currentWeaponConfig = weapon;

            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, GetComponent<Animator>());
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if (target == null) return;
            if (!target.IsAlive()) return;
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);
            }
            
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        void AttackBehaviour()
        {
            transform.LookAt(target.transform);

            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                //This will trigger the Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;

            }
        }

        void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }
        //Animation event: içine yazdığım şeyler tam animasyon süresinde oluyor... Acayip
        void Hit()
        {
            if (target == null) return;
            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            if (currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }
            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);

            }
        }

        void Shoot()
        {
            Hit();
        }

        bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetWeaponRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
                
            }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && targetToTest.IsAlive();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionSchedular>().StartAction(this);
            
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
            target = null;
            GetComponent<Mover>().Cancel();
        }
        // Animation Event

        public object CaptureState()
        {
            if (currentWeapon == null) return "Unarmed" ;
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            if (!Resources.Load<WeaponConfig>(weaponName)) return;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        public Health GetTarget()
        {
            return target;
        }

        public IEnumerable<float> GetModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetWeaponDamage();
            }
        }


        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}

