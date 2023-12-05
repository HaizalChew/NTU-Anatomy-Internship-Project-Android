using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StrictButtonExtension : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] Animator animator;
    public bool isOn;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (button == null)
        {
            button = GetComponent<Button>();

        }

        button.onClick.AddListener(() => Toggle());
    }

    public void Toggle()
    {
        isOn = !isOn;
    }

    private void Update()
    {
        animator?.SetBool("IsSelected", isOn);
    }

}
