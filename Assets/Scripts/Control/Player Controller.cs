
using System;
using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Health health;



        [System.Serializable]
        struct CursorMapping
        {
            public Cursors type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] cursors = null;
        [SerializeField] private float maxNavMeshDistance = 1f;
        [SerializeField] private float maxPathLength = 40f;
        private void Awake()
        {
            health = GetComponent<Health>();
        }

        private void Update()
        {
            if (InteractWithUI())
            {
                return;
            }
            if (!health && health.IsAlive())
            {
                SetCursor(Cursors.None);
                return;
            }

            if (InteractWithComponent())
            {
                return;
            }

            if (InteractWithMovement())
            {
                return;
            };
            SetCursor(Cursors.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }

            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(getMouseRay());
            float[] distances = new float[hits.Length];
            
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            
            Array.Sort(distances,hits);
            return hits;
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())//GameObject means UI GAME object in this context
            {
                SetCursor(Cursors.OnUI);
                return true;
            }
            return false;
        }
        
        bool InteractWithMovement()
        {
            //RaycastHit hit;
            //bool hasHit = Physics.Raycast(getMouseRay(), out hit);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    if(GetComponent<Mover>()) GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursor(Cursors.Move);

                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(getMouseRay(), out hit);
            if (!hasHit) return false;
            
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, 
                maxNavMeshDistance,
                NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;
            
            target = navMeshHit.position;
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxPathLength) return false;
            return true;
        }

        private double GetPathLength(NavMeshPath path)
        {
            float sum = 0;
            if (path.corners.Length < 2) return sum;
            for (int i = 0; i < path.corners.Length-1; i++)
            {
                sum  += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return sum;
        }


        private void SetCursor(Cursors cursor)
        {
            CursorMapping map = GetCursorMapping(cursor);
            Cursor.SetCursor(map.texture, map.hotspot, CursorMode.Auto);
        }

        CursorMapping GetCursorMapping(Cursors cursor)
        {
            foreach (CursorMapping map in cursors)
            {
                if (map.type == cursor)
                {
                    return map;
                }
            }

            return cursors[0];
        }



        static Ray getMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}

