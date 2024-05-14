 
using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageText =null;
        
        
        public void  Spawn(float damageAmount)
        {
            DamageText instance =  Instantiate<DamageText>(damageText,transform);
            instance.SetText(damageAmount);
        }
    }
}