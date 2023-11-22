using UnityEngine;

public class IsolateSelectionScript : MonoBehaviour
{
    public GameObject mainModel;

    private Camera mainCamera;
    private SelectRenderOutline selectOutlineScript;

    public bool isIsolate;
    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = Camera.main;
        selectOutlineScript = mainCamera.GetComponent<SelectRenderOutline>();
    }

    public void IsolateSelection()
    {
        bool isSelectedNull = CheckSelectedNull();
        if (!isSelectedNull)
        {
            isIsolate = !isIsolate;
            //Hide all child objects
            foreach (Transform child in mainModel.transform)
            {
                child.gameObject.SetActive(!isIsolate);
            }
            //Get seleceted objects and set active
            Renderer[] selectedArray = CreateSelectionArray();
            foreach (Renderer selected in selectedArray)
            {
                selected.gameObject.SetActive(true);
            }
        }
    }

    private bool CheckSelectedNull()
    {
        if(selectOutlineScript.SelectRenderObject.Count == 0)
        {
            return true;
        }
        return false;
    }

    private Renderer[] CreateSelectionArray()
    {
        Renderer[] array = new Renderer[selectOutlineScript.SelectRenderObject.Count];
        for (int i = 0; i < selectOutlineScript.SelectRenderObject.Count; i++)
        {
            array[i] = selectOutlineScript.SelectRenderObject[i];
        }
        return array;
    }

}
