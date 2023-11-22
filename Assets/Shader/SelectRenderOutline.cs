using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SelectRenderOutline : MonoBehaviour
{
    public List<Renderer> SelectRenderObject = new List<Renderer>();

    public Material WriteObject;

    public Material SelectOutline;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //setup stuff
        var commands = new CommandBuffer();
        commands.name = "Test";
        int selectionBuffer = Shader.PropertyToID("_SelectionBuffer");
        commands.GetTemporaryRT(selectionBuffer, source.descriptor);
        //render selection buffer
        commands.SetRenderTarget(selectionBuffer);
        commands.ClearRenderTarget(true, true, Color.clear);
        if (SelectRenderObject != null)
        {
            foreach (Renderer renderObj in SelectRenderObject)
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