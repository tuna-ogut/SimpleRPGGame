using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        private bool isTriggered = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && !isTriggered)
            {
                Debug.Log("sdasfasds");
                GetComponent<PlayableDirector>().Play();
                isTriggered = true;
            }
        }
    }
}

