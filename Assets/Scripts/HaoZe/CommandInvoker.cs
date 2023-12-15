using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker
{
    public static Stack<ICommand> commandList = new Stack<ICommand>();

    public static void ExecuteSave(ICommand command)
    {
        //Save move data to stack
        commandList.Push(command);
        Debug.Log("push");
    }

    public static void ExecuteUndo(UiManager uiManager)
    {
        if(commandList.Count > 0)
        {
            ICommand command = commandList.Pop();
            command.Undo();
        }
        uiManager.UpdateHistoryPanel();
    }

    public static void ExecuteToggle(ICommand command)
    {
        command.Toggle();
    }

    public static void ExecuteReset(UiManager uiManager)
    {
        foreach(ICommand command in commandList)
        {
            command.Undo();
        }
        commandList.Clear();
        uiManager.UpdateHistoryPanel();
    }
}
