using System.Collections.Generic;
using UnityEngine;

public class UndoFunction : MonoBehaviour
{
    public MovePart movePart;
    public List<HistoryData> historyContainer = new List<HistoryData>();

    [System.Serializable]
    public struct HistoryData
    {
        public List<MovePart.movedObjectData> moveContainer;
        public GameObject undoContainer;
    }

    private void OnEnable()
    {
        MovePart.saveMoveHistory += StoreMoveHistory;
    }

    public void StoreMoveHistory(List<MovePart.movedObjectData> movedObjectContainer)
    {
        foreach (MovePart.movedObjectData container in movedObjectContainer)
        {
            Debug.Log(container.movedObject);
            Debug.Log(container.movedObjectOriginalPos);
        }
        HistoryData historyData = new HistoryData();
        historyData.moveContainer = movedObjectContainer;
        historyContainer.Add(historyData);
    }

    public void Undo()
    {
        int lastCount = historyContainer.Count - 1;
        //Check if Move or Undo 
        if (historyContainer[lastCount].moveContainer != null)
        {
            //Get UndoHistory from MovePartScript
        }
    }
}
