public abstract class NetworkCombat : Combat
{
    protected NetworkEntityReferenceHolder ReferenceHolder { get { return (NetworkEntityReferenceHolder)_referenceHolder; } }
    protected NetworkTransformReceive NetworkTransformReceive { get { return ReferenceHolder.NetworkTransformReceive; } }
    protected NetworkCharacterControllerReceive NetworkCharacterControllerReceive { get { return ReferenceHolder.NetworkCharacterControllerReceive; } }
    protected NetworkIdentity Identity { get { return _referenceHolder.Entity.Identity; } }

    protected override void OnDeath()
    {
        if (AnimationController != null)
        {
            AnimationController.enabled = false;
        }
        if (NetworkTransformReceive != null)
        {
            NetworkTransformReceive.enabled = false;
        }
        if (NetworkCharacterControllerReceive != null)
        {
            NetworkCharacterControllerReceive.enabled = false;
        }
    }

    public void LookAtTarget()
    {
        if (AttackTarget != null && Status.Hp > 0)
        {
            NetworkTransformReceive.LookAt(_attackTarget);
        }
    }

    /* Notify server that entity got attacked */
    public override void InflictAttack(AttackType attackType)
    {
        GameClient.Instance.ClientPacketHandler.InflictAttack(Identity.Id, attackType);
    }
}