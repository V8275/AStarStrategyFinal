using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField]
    private string IdleStateName = "Idle";
    [SerializeField]
    private string RunStateName = "Run";
    [SerializeField]
    private string ShootStateName = "Shoot";
    [SerializeField]
    private string LookOutStateName = "LookOut";

    [SerializeField]
    private List<Animator> AnimatorControllers = new List<Animator>();

    public void AnimationIdle(bool val)
    {
        foreach(var anim in AnimatorControllers)
        {
            anim.SetBool(IdleStateName, val);   
        }
    }

    public void AnimationRun(bool val)
    {
        foreach (var anim in AnimatorControllers)
        {
            anim.SetBool(RunStateName, val);
        }
    }

    public void AnimationShoot()
    {
        foreach (var anim in AnimatorControllers)
        {
            anim.SetTrigger(ShootStateName);
        }
    }

    public void AnimationLookOut()
    {
        foreach (var anim in AnimatorControllers)
        {
            anim.SetTrigger(LookOutStateName);
        }
    }
}
