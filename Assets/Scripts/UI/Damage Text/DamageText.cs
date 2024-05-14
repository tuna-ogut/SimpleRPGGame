using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private Text damageText = null;        


        public void SetText(float amount)
        {
            damageText.text = amount.ToString();
        }

        public void DestroyText()
        {
            Destroy(gameObject);
        }
    }

}