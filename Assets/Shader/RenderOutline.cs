using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RenderOutline : MonoBehaviour
{
    public List<Renderer> RenderObject = new List<Renderer>();

    public Material WriteObject;

    public Material SelectOutline;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //setup stuff
        var commands = new CommandBuffer();
        commands.name = "Test2";
        int selectionBuffer = Shader.PropertyToID("_SelectionBuffer");
        commands.GetTemporaryRT(selectionBuffer, source.descriptor);
        //render selection buffer
        commands.SetRenderTarget(selectionBuffer);
        commands.ClearRenderTarget(true, true, Color.clear);
        if (RenderObject != null)
        {
            foreach (Renderer renderObj in RenderObject)
            {
                commands.DrawRenderer(renderObj, WriteObject);  
            }
        }
        //apply everything and clean up in command buffer
        commands.Blit(source, destination, SelectOutline);
        commands.ReleaseTemporaryRT(selectionBuffer);

        //execute and clean up command buffer itself
        Graphics.ExecuteCommandBuffer(commands);
        commands.Dispose();
        Graphics.SetRenderTarget(destination);
    }


}