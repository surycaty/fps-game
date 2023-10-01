using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RobotPathPatrol : MonoBehaviour
{
    [Tooltip("Enemies that will be assigned to this path on Start")]
    public Transform enemiesToAssign;
    [Tooltip("The Nodes making up the path")]
    public List<Transform> pathNodes = new List<Transform>();

    public float speedMoveEnemy = 2f;

    private Vector3 nextPosition;

    private void Start()
    {
            //enemiesToAssign.patrolPath = this;
            nextPosition = pathNodes[0].position;
    }

    private void FixedUpdate()
    {
        if (enemiesToAssign != null)
        {
            if (enemiesToAssign.position == pathNodes[0].position)
            {
                nextPosition = pathNodes[1].position;
            }

            if (enemiesToAssign.position == pathNodes[1].position)
            {
                nextPosition = pathNodes[0].position;
            }

            enemiesToAssign.position = Vector3.MoveTowards(enemiesToAssign.position, nextPosition, speedMoveEnemy * Time.deltaTime);
        }
    }

    public float GetDistanceToNode(Vector3 origin, int destinationNodeIndex)
    {
        if (destinationNodeIndex < 0 || destinationNodeIndex >= pathNodes.Count || pathNodes[destinationNodeIndex] == null)
        {
            return -1f;
        }

        return (pathNodes[destinationNodeIndex].position - origin).magnitude;
    }

    public Vector3 GetPositionOfPathNode(int NodeIndex)
    {
        if (NodeIndex < 0 || NodeIndex >= pathNodes.Count || pathNodes[NodeIndex] == null)
        {
            return Vector3.zero;
        }

        return pathNodes[NodeIndex].position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < pathNodes.Count; i++)
        {
            int nextIndex = i + 1;
            if (nextIndex >= pathNodes.Count)
            {
                nextIndex -= pathNodes.Count;
            }

            Gizmos.DrawLine(pathNodes[i].position, pathNodes[nextIndex].position);
            Gizmos.DrawSphere(pathNodes[i].position, 0.1f);
        }
    }
}
