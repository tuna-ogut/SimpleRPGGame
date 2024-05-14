
using RPG.Attributes;
using TMPro;
using UnityEngine;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter fighter;
        private Health target = null;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }
        

        private void Update()
        {
            DisplayText();
        }

        void DisplayText()
        {
            target = fighter.GetTarget();

            if (target == null)
            {
                GetComponent<TMP_Text>().text = " N/A ";
                return;
            }
            GetComponent<TMP_Text>().text = target.GetHealthPoints() + "/" + target.GetMaxHealthPoints() ;
        }
    }

}