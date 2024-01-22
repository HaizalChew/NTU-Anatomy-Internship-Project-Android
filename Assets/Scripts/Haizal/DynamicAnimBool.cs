using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicAnimBool : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] string boolID = "IsPressed";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimatorBool(bool animBool)
    {
        animator.SetBool(boolID, animBool);
    }
}
