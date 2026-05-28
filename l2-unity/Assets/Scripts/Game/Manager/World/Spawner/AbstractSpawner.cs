using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class EntitySpawnStrategy<TAppearance, TStats, TStatus> where TAppearance : Appearance where TStats : Stats where TStatus : Status
{
    protected readonly EventProcessor _eventProcessor;

    protected EntitySpawnStrategy(EventProcessor eventProcessor)
    {
        _eventProcessor = eventProcessor;
    }

    public void OnReceiveEntityInfo(NetworkIdentity identity, TStatus status, TStats stats,
        TAppearance appearance, EntityActionInfo actionInfo)
    {
        if (!WorldSpawner.Instance.IsEntityPresent(identity.Id))
        {
            _eventProcessor.QueueEvent(() => SpawnEntity(identity, status, stats, appearance, actionInfo));
        }
        else
        {
            _eventProcessor.QueueEvent(() => UpdateEntityAsync(identity, status, stats, appearance, actionInfo));
        }
    }

    protected virtual void UpdateEntityAsync(NetworkIdentity identity, TStatus status, TStats stats,
        TAppearance appearance, EntityActionInfo actionInfo)
    {
        // Don't need to block thread
        Task task = new Task(async () =>
        {
            var entity = await WorldSpawner.Instance.GetEntityAsync(identity.Id);
            if (entity != null)
            {
                _eventProcessor.QueueEvent(() => UpdateEntitySync(entity, identity, status, stats, appearance, actionInfo));
            }
        });
        task.Start();
    }

    protected abstract void SpawnEntity(NetworkIdentity identity, TStatus status,
        TStats stats, TAppearance appearance, EntityActionInfo actionInfo);

    protected abstract void UpdateEntitySync(Entity entity, NetworkIdentity identity,
        TStatus status, TStats stats, TAppearance appearance, EntityActionInfo actionInfo);

    protected void UpdateAction(Entity entity, EntityActionInfo actionInfo)
    {
        entity.UpdateMoveType(actionInfo.Running);

        if (actionInfo.InCombat)
        {
            entity.ReferenceHolder.Combat.RefreshCombatTimestamp();
        }

        if (actionInfo.AlikeDead)
        {
            entity.Combat.OnDeath();
        }

        entity.UpdateMoveType(actionInfo.Running);

        if (actionInfo.Sitting)
        {
            entity.UpdateWaitType(ChangeWaitTypePacket.WaitType.WT_SITTING);
        }
    }
}