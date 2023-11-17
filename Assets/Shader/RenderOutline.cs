using UnityEngine;
using UnityEngine.Rendering;

public class RenderOutline : MonoBehaviour
{
    public Renderer[] RenderObject;

    public Material WriteObject;

    public Material SelectOutline;
    void Update()
    {

    }

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
        if (RenderObject != null)
        {
            foreach (Renderer renderObj in RenderObject)
            {
                commands.DrawRenderer(renderObj, WriteObject);
            }
        }
        //apply everything and clean up in commandbuffer
        commands.Blit(source, destination, SelectOutline);
        commands.ReleaseTemporaryRT(selectionBuffer);

        //execute and clean up commandbuffer itself
        Graphics.ExecuteCommandBuffer(commands);
        commands.Dispose();
        Graphics.SetRenderTarget(destination);
    }


}