using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PathFindingNew path;
    [SerializeField] float speed;
    [SerializeField] LayerMask blocks;

     private void Update()
     {
         if (Input.GetMouseButtonDown(0))
         {
             RaycastHit hit;
    
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
             if (Physics.Raycast(ray, out hit, blocks))
             {
               StartCoroutine(Moving(path.FindPath(transform.position, hit.transform.position)));
             }
         }
     }

    IEnumerator Moving(List<Vector3> path)
    {
        while(path.Count != 0)
        {
            while(Vector3.Distance(transform.position, path[0]) >= 0.02f)
            {
                transform.position = Vector3.MoveTowards( transform.position,path[0], speed * Time.deltaTime);

                yield return null;
            }

            path.RemoveAt(0);
        }
    }
}
