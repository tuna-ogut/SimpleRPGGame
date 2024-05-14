using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class XPDisplay : MonoBehaviour
    {
        private XP xp;

        private void Awake()
        {
            xp = GameObject.FindWithTag("Player").GetComponent<XP>();
        }
        

        private void Update()
        {
            DisplayText();
        }

        void DisplayText()
        {
            if (xp == null) return;
            GetComponent<TMP_Text>().text =xp.GetPoints().ToString();
        }
    }

}