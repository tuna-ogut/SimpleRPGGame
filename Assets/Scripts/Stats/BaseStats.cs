using System;
using GameDevTV.Utils;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1,99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] private CharacterClass characterClass;
        [SerializeField] private Progression progression = null;
        [SerializeField] private GameObject levelUpEffect = null;
        [SerializeField] private bool shouldUseModifiers = false;
        LazyValue<int> currentLevel;
        public event Action onLevelGain;

        private XP xp;
        private void Awake()
        {
            xp = GetComponent<XP>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (xp != null)
            {
                xp.onExperienceGained += UpdateLevel;
            }   
        }

        private void OnDisable()
        {
            if (xp != null)
            {
                xp.onExperienceGained -= UpdateLevel;
            }
        }

        void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelGain();
            }
        }

        private void LevelUpEffect()
        {
            if (levelUpEffect != null)
            {
                Instantiate(levelUpEffect, transform);
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetInitialStat(stat) + GetModifier(stat)) * 1 + GetPercentageModifier(stat)/100;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;

            float sum = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifier(stat))
                {
                    sum += modifier;
                }
            }
            return sum; 
        }

        float GetInitialStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        private float GetModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float sum = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetModifier(stat))
                {
                    sum += modifier;
                }
            }
            return sum;
        }

        public int GetLevel()
        {
            if (currentLevel.value < 1)
            {
                currentLevel.value = CalculateLevel();
            }
            return currentLevel.value;
        }
        
        public int CalculateLevel()
        {
            XP xp = GetComponent<XP>();
            if (xp == null) return startingLevel;
            
            float currentXP = xp.GetPoints();
            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int i = 1; i <= penultimateLevel; i++)
            {
                float XPToNextLevel = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, i);
                if (currentXP < XPToNextLevel)
                {
                    return i;//level
                }
            }
            return penultimateLevel + 1;
        }
    }
}