using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandInvoker
{
    private static Stack<ICommand> commandList = new Stack<ICommand>();

    public static void ExecuteSave(ICommand command)
    {
        //Save move data to stack
        commandList.Push(command);
        Debug.Log("push");
    }

    public static void ExecuteUndo()
    {
        if(commandList.Count > 0)
        {
            ICommand command = commandList.Pop();
            command.Undo();
        }
    }
}
