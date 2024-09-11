using System;

public class PlayerStateAction : PlayerStateBase
{
    protected bool ShouldSit()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.SITTING)
        {
            SetBool("sit", false, true, false); //Do not share sit animation (shared by server with ChangeWaitType)
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
                    SetBool("run_jump", false, true);
                }
                else
                {
                    SetBool("jump", false, true);
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
            SetBool("run", true, true);
            return true;
        }

        return false;
    }

    protected bool ShouldIdle()
    {
        if (PlayerStateMachine.Instance.State == PlayerState.IDLE)
        {
            SetBool("wait", true, true);
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
            SetBool("death", false, true);
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

    protected bool ShouldAtkWait()
    {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        if (PlayerStateMachine.Instance.State == PlayerState.IDLE
             && now - _entity.StopAutoAttackTime < 5000)
        {
            if (PlayerEntity.Instance.AttackTarget == null)
            {
                SetBool("atkwait", true, true, false);
                return true;
            }
        }

        return false;
    }
}
