using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(MonsterAnimationAudioHandler))]
public class MonsterAnimationController : MonoBehaviour
{
    public Animator animator;
    public string npcTemplateName;
    public string animationClipName;
    private MonsterAnimationAudioHandler monsterAnimationAudioHandler;
    private bool animationStarting = false;

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

        monsterAnimationAudioHandler = GetComponent<MonsterAnimationAudioHandler>();
    }

    void Update() {


       /* if(IsCurrentState("spwait")) {
            if(IsAnimationStarting()) {
                monsterAnimationAudioHandler.PlaySound(MonsterSoundEvent.Breathe);
            }

            if(IsAnimationFinished() && !IsNextState("wait")) {
                SetBool("wait", true);
            }
        }

        if(IsCurrentState("wait")) {
            SetBool("wait", false);

    
        } */
    }

    private bool IsAnimationStarting() {
        bool initialState = animationStarting;
        animationStarting = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 0.05f;
        if(initialState == false && animationStarting == true) {
            return true;
        }
        return false;
    }

    private bool IsCurrentState(string state) {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(state);
    }

    private bool IsAnimationFinished() {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f;
    }

    private bool IsNextState(string state) {
        return animator.GetNextAnimatorStateInfo(0).IsName(state);
    }

    void SetBool(string name, bool value) {
        if(animator.GetBool(name) != value) {
            animator.SetBool(name, value);
        }
    }
}
