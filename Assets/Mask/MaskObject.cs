using UnityEngine;

public class MaskObject : MonoBehaviour
{
    public GameObject[] maskObj;
    public Material maskMaterial;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < maskObj.Length; i++)
        {
            MeshRenderer meshRenderer = maskObj[i].GetComponent<MeshRenderer>();

            for (int k = 0; k < meshRenderer.materials.Length; k++)
            {
                meshRenderer.materials[k].renderQueue = 3002;
            }
                
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
