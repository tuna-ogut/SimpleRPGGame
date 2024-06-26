using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Health healthComponent = null;
        [SerializeField] private RectTransform foreGround = null;
        [SerializeField] private Canvas rootCanvas = null;
        void Update()
        {
            if (Mathf.Approximately(healthComponent.GetFraction(), 0))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            
            foreGround.localScale = new Vector3(healthComponent.GetFraction(), 1, 1);
            
        }
    }
}
