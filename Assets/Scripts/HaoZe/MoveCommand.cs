using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class MoveCommand : ICommand
{
    [SerializeField]

    //Data to Undo

    //Data to execute and Undo
    private Vector3 originPos;
    private Vector3 currentPos;
    private GameObject[] movedObjects;
    private Transform hit;
    private MovePart movePart;

    public MoveCommand(Vector3 originPos, Vector3 currentPos, GameObject[] movedObjects, Transform hit, MovePart movePart)
    {
        this.originPos = originPos;
        this.currentPos = currentPos;
        this.movedObjects = movedObjects;
        this.hit = hit;
        this.movePart = movePart;
    }

    public void Undo()
    {
        foreach(var obj in movedObjects)
        {
            if(obj.transform != hit)
            {
                obj.transform.parent = hit.transform;
            }
        }

        hit.transform.position = originPos;
        movePart.UndoMove(movedObjects,hit,originPos);
    }

    public void Toggle()
    {
        movePart.ToggleMove(hit, movedObjects, currentPos, originPos);
    }
}
