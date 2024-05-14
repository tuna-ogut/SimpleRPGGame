
using TMPro;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health health;

        private void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }
        

        private void Update()
        {
             DisplayText();
        }

        void DisplayText()
        {
            if (health == null) return;
            GetComponent<TMP_Text>().text = health.GetHealthPoints() + "/" + health.GetMaxHealthPoints() ;
        }
    }

}