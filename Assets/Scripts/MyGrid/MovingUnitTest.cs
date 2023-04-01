using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingUnitTest : MonoBehaviour
{

    private const float speed = 40f;

    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
    }

    private void StopMoving()
	{
        pathVectorList = null;
	}

    public Vector3 GetPosition()
	{
        return transform.position;
	}
    private void HandleMovement()
	{
        if (pathVectorList != null)
		{
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            targetPosition.y -= Pathfinding.Instance.GetGrid().GetCellSize() / 2;
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
			{
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                //animatedWalker.SetMoveVector(moveDir);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
			}
			else
			{
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
				{
                    StopMoving();
                    //animatedWalker.SetMoveVector(Vector3.zero);
				}
			}
		}
        else
        {
            //animatedWalker.SetMoveVector(Vector3.zero);
        }
    }
    public void SetTargetPosition(Vector3 targetPosition)
	{
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1)
		{
            pathVectorList.RemoveAt(0);

        }
	}
}
