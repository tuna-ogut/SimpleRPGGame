using System;
using UnityEngine;
using RPG.Saving;
using Unity.VisualScripting;

namespace RPG.Stats
{
    public class XP: MonoBehaviour, ISaveable
    {
        [SerializeField] private float experiencePoints = 0;


        public event Action onExperienceGained;

        public void GainXP(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public object CaptureState()
        {
            return experiencePoints;
        }
        
        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }

        public float GetPoints()
        {
            return experiencePoints;
        }
    }
}