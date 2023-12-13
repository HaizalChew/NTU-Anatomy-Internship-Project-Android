using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SaveHistory : MonoBehaviour
{

    public delegate void StoreData();
    public StoreData storeData;

    public List<NewUndoData> historyDataList = new List<NewUndoData>();
    public UndoHideData holdHideData;
    public UndoMoveData holdMoveData;

    [System.Serializable]
    public class UndoMoveData
    {
        public GameObject[] movedObjs;
        public Transform hitTransform;
        public Vector3 currentPos;
        public Vector3 prevPos;
    }

    [System.Serializable]
    public class UndoHideData
    {
        public GameObject hideContainer;
    }

    [System.Serializable] 
    public class NewUndoData
    {
        public UndoMoveData undoMoveData;
        public UndoHideData undoHideData;
    }
    public enum UndoDataType
    {
        Hide,
        Move
    }

    public void TransferHideData(GameObject container)
    {
        UndoHideData hideData = new UndoHideData();
        hideData.hideContainer = container;
        holdHideData = hideData;
    }

    public void TransferMoveData(GameObject[] movedObjects, Transform transform, Vector3 pos, Vector3 prevPos)
    {
        UndoMoveData moveData = new UndoMoveData();
        moveData.movedObjs = movedObjects;
        moveData.hitTransform = transform;
        moveData.currentPos = pos;
        moveData.prevPos = prevPos;
        holdMoveData = moveData;
    }


    public void StoreNewData()
    {
        NewUndoData storeNewData = new NewUndoData();
        if (holdHideData != null)
        {
            storeNewData.undoHideData = holdHideData;
            //Clear holdData
            holdHideData = null;
        }
        if(holdMoveData != null)
        {
            storeNewData.undoMoveData = holdMoveData;
            //Clear MoveData
            holdMoveData = null;
        }      
        historyDataList.Add(storeNewData);

    }

    public int DataToUndo()
    {
        for (int i = historyDataList.Count - 1; i >= 0; i--)
        {
            //Check if undo move or hide
            if (historyDataList[i].undoMoveData != null)
            {
                return i;
            }
        }
        return -1;
    }

    public void UndoMove()
    {
        //Check  if list empty
        if(historyDataList.Count > 0)
        {
            int dataIndex = DataToUndo();
            if(dataIndex >= 0)
            {
                var undoMoveData = historyDataList[dataIndex].undoMoveData;
                //Parent to hit transform to undo move 
                foreach (GameObject obj in undoMoveData.movedObjs)
                {
                    if (obj.transform != undoMoveData.hitTransform)
                    {
                        obj.transform.parent = undoMoveData.hitTransform;
                    }
                }

                Debug.Log(undoMoveData.prevPos);
                Debug.Log(undoMoveData.hitTransform.position);
                //Change Pos back to prevPos
                undoMoveData.hitTransform.position = undoMoveData.prevPos;
                //Remove from list
                historyDataList.Remove(historyDataList[dataIndex]);
            }
        }

    }
}
