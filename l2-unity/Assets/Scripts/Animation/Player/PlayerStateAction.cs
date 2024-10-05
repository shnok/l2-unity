using System;

public class PlayerStateAction : PlayerStateBase
{
    protected bool ShouldSit()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.SITTING)
        {
            SetBool(HumanoidAnimType.sit, true, false); //Do not share sit animation (shared by server with ChangeWaitType)
                                                        // At some point need to get rid of the ShareAnimation packet 
            return true;
        }

        return false;
    }

    protected bool ShouldJump(bool run)
    {
        if (PlayerStateMachine.Instance.State == PlayerState.IDLE || PlayerStateMachine.Instance.State == PlayerState.RUNNING)
        {
            if (InputManager.Instance.Jump)
            {
                CameraController.Instance.StickToBone = true;
                if (run)
                {
                    SetBool(HumanoidAnimType.run_jump, true, true);
                }
                else
                {
                    SetBool(HumanoidAnimType.jump, true, true);
                }
                return true;
            }
        }

        return false;
    }

    protected bool ShouldRun()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.RUNNING)
        {
            SetBool(HumanoidAnimType.run, true, true);
            return true;
        }

        return false;
    }

    protected bool ShouldWalk()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.WALKING)
        {
            SetBool(HumanoidAnimType.walk, true, true);
            return true;
        }

        return false;
    }

    protected bool ShouldIdle()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.IDLE)
        {
            SetBool(HumanoidAnimType.wait, true);
            return true;
        }

        return false;
    }


    protected bool ShouldAttack()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.ATTACKING)
        {
            return true;
        }

        return false;
    }

    protected bool ShouldDie()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.DEAD)
        {
            SetBool(HumanoidAnimType.death, true);
            return true;
        }

        return false;
    }

    //protected bool ShouldAtkWait() {
    //    long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    //    if ((!InputManager.Instance.IsInputPressed(InputType.Move) && !PlayerController.Instance.RunningToDestination || !PlayerController.Instance.CanMove)
    //         && now - _entity.StopAutoAttackTime < 5000) {
    //        if (PlayerEntity.Instance.AttackTarget == null) {
    //            SetBool("atkwait_" + _weaponAnim, true, false);
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    protected bool ShouldStand()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.STANDING)
        {
            SetBool(HumanoidAnimType.stand, true, false);
            return true;
        }

        return false;
    }

    protected bool ShouldAtkWait()
    {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (PlayerStateMachine.Instance.State == PlayerState.IDLE
             && now - PlayerCombat.Instance.StopAutoAttackTime < 5000)
        {
            if (PlayerCombat.Instance.AttackTarget == null)
            {
                SetBool(HumanoidAnimType.atkwait, true, false);
                return true;
            }
        }

        return false;
    }
}
