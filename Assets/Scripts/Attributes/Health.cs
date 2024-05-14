using System;
using GameDevTV.Utils;
using UnityEngine;
using RPG.Saving;
using RPG.Stats;
using RPG.Core;
using UnityEngine.Events;

namespace RPG.Attributes{
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float>  health;
        private bool isAlive = true;
        [SerializeField] private float newHealthPercentage = 70f;
        [SerializeField] private TakeDamageEvent takeDamage;
        [SerializeField] private UnityEvent onDie;
        
        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {
            
        }
        
        public bool IsAlive()
        {
            return isAlive;
        }

        private void Awake()
        {
            health = new LazyValue<float>(GetInitialHealth);
        }

        private void Start()
        {
            health.ForceInit();
        }

        float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }
        
        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelGain += HealthBonus;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelGain -= HealthBonus;
        }

        void HealthBonus()
        {
            float newHealth = GetComponent<BaseStats>().GetStat(Stat.Health) * (newHealthPercentage / 100);
            health.value = Mathf.Max(health.value, newHealth);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            Debug.Log(gameObject.name + "took damage: " + damage);
            health.value = Mathf.Max(health.value - damage, 0);
            
            
            if (health.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
            
        }

        void AwardExperience(GameObject instigator)
        {
            XP xp = instigator.GetComponent<XP>();
            if (xp == null) return;
            xp.GainXP(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        void Die()
        {
            if (!isAlive) return;

            isAlive = false;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionSchedular>().CancelCurrentAction();
        }

        public float GetPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetHealthPoints()
        {
            return health.value;
        }

        public float GetMaxHealthPoints()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public object CaptureState()
        {
            return health.value;
        }

        public void RestoreState(object state)
        {
            health.value = (float)state;

            if (health.value <= 0)
            {
                Die();
            }
        }

        public float GetFraction()
        {
            return  (health.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public void Heal(float healthBoost)
        {
            health.value = Mathf.Min(health.value + healthBoost, GetMaxHealthPoints());
        }
        
    }

}
