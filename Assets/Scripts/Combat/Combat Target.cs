using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public bool HandleRaycast(PlayerController caller)
        {

            if (!caller.GetComponent<Fighter>().CanAttack(gameObject)) return false;
            if(Input.GetMouseButton(0))
            {
                caller.GetComponent<Fighter>().Attack(gameObject);
            }
            return true;
        }

        public Cursors GetCursorType()
        {
            return Cursors.Attack;
        }
    }
}

