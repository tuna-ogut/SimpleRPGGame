using TMPro;
using UnityEngine;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats baseStats;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            DisplayText();
        }

        void DisplayText()
        {
            if (baseStats == null) return;
            GetComponent<TMP_Text>().text =baseStats.GetLevel().ToString();
        }
    }

}