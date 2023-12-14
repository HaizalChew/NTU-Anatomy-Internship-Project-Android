using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideCommand : ICommand
{
    [SerializeField]
    private GameObject[] hideObjects;
    private HideSelectionScript hideSelectionScript;
    
    public HideCommand(GameObject[] hideObjects, HideSelectionScript hideSelectionScript)
    {
        this.hideObjects = hideObjects;
        this.hideSelectionScript = hideSelectionScript;
    }

    public void Undo()
    {
        Debug.Log("Undo Hide");
        hideSelectionScript.UnhideSelection(hideObjects);
    }
}
