using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField]
    Animator animator;

    public void OpenChest()
    {
        animator.SetBool("isOpen", true);
    }
}
