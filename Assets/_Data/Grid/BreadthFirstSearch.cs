using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch : GridAbstract, IPathfinding
{
    [Header("Breadth First Search")]
    public List<Node> queue = new List<Node>();
    public List<Node> path = new List<Node>();
    public Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

    public virtual void FindPath(BlockCtrl startBlock, BlockCtrl targetBlock)
    {
        Debug.Log("FindPath");
        Node startNode = startBlock.blockData.node;
        Node targetNode = targetBlock.blockData.node;

        this.Enqueue(startNode);
        this.cameFrom[startNode] = startNode;

        while (this.queue.Count > 0)
        {
            Node current = this.Dequeue();

            if (current == targetNode)
            {
                ConstructPath(startNode, targetNode);
                break;
            }

            foreach (Node neighbor in current.Neighbors())
            {
                if (neighbor == null) continue;

                if (this.IsValidPosition(neighbor, targetNode) && !cameFrom.ContainsKey(neighbor))
                {
                    this.Enqueue(neighbor);
                    this.cameFrom[neighbor] = current;
                }
            }
        }

        this.ShowCameFrom();
        this.ShowPath();
    }

    protected virtual void ShowCameFrom()
    {
        foreach(var pair in this.cameFrom)
        {
            Node key = pair.Key;
            Vector3 pos = key.nodeObj.transform.position;
            Transform keyObj = this.ctrl.blockSpawner.Spawn(BlockSpawner.SCAN, pos, Quaternion.identity);
            keyObj.gameObject.SetActive(true);
        }
    }

    protected virtual void ConstructPath(Node startNode, Node targetNode)
    {
        Node currentCell = targetNode;

        while (currentCell != startNode)
        {
            path.Add(currentCell);
            currentCell = this.cameFrom[currentCell];
        }

        path.Add(startNode);
        path.Reverse();
    }

    protected virtual void ShowPath()
    {
        Vector3 pos;
        foreach(Node node in this.path)
        {
            pos = node.nodeObj.transform.position;
            Transform linker = this.ctrl.blockSpawner.Spawn(BlockSpawner.LINKER, pos, Quaternion.identity);
            linker.gameObject.SetActive(true);
        }
    }

    protected virtual void Enqueue(Node blockCtrl)
    {
        this.queue.Add(blockCtrl);
    }

    protected virtual Node Dequeue()
    {
        Node node = this.queue[0];
        this.queue.RemoveAt(0);
        return node;
    }

    private bool IsValidPosition(Node node, Node startNode)
    {
        if (node == startNode) return true;

        return !node.occupied;
    }
}
