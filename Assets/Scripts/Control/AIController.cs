using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Control;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;


namespace RPG.Combat
{
    public class AIController : MonoBehaviour
    {
        
        [SerializeField] private float chaseDistance = 5f;
        [SerializeField] private float weaponRange = 2f;
        [SerializeField] private PatrolPath patrolpath;
        [SerializeField] private float waypointTolerance = 1f;
        [SerializeField] private float suspicionTimer = 3f;
        [SerializeField] private float waypointDwellTime = 2f;
        
        [Range(0,1)]
        [SerializeField] private float patrolSpeedFraction = .2f;

        
        Fighter fighter;
        private GameObject player;
        private Health health;
        private Mover mover;
        private float timeSinceLastSawPlayer = Mathf.Infinity;
        private float timeSinceArrivedWaypoint = Mathf.Infinity;
        private int currentWaypointIndex = 0;

        LazyValue<Vector3>  guardPosition;


        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            player = GameObject.FindWithTag("Player");
            mover = GetComponent<Mover>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Update()
        {
            if (!health.IsAlive()) return;
            if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0;
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTimer)
            {
                SuspicionBehaviour();
            }
            
            else
            {
                PatrolBehaviour();
            }
            timeSinceLastSawPlayer+=Time.deltaTime;
            timeSinceArrivedWaypoint += Time.deltaTime;

        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;
            

            if (patrolpath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedWaypoint > waypointDwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);

            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolpath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolpath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionSchedular>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            return Vector3.Distance(player.transform.position, transform.position) < chaseDistance;
        }
        
        void InteractWithCombat(GameObject target)
        {

            if (target == null) return;
            GetComponent<Fighter>().Attack(target.gameObject);
        }

        
        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance); 
        }
    }

}
