using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NpcAnimationController : MonoBehaviour
{
    public Animator animator;
    public string npcTemplateName;

    private void Awake() {
        if(animator == null) {
            animator = GetComponent<Animator>();
        }

        if(animator.runtimeAnimatorController == null) {
            if(npcTemplateName.Length == 0) {
                npcTemplateName = transform.name;
            }
            animator.runtimeAnimatorController = Resources.Load<AnimatorOverrideController>("Data/Animations/Animator/" + npcTemplateName);
        }
    }

    void Start() {
      
    }

    void FixedUpdate() {

    }
}
