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
        TAppearance appearance, bool running)
    {
        if (!WorldSpawner.Instance.IsEntityPresent(identity.Id))
        {
            _eventProcessor.QueueEvent(() => SpawnEntity(identity, status, stats, appearance, running));
        }
        else
        {
            _eventProcessor.QueueEvent(() => FindAndUpdateEntity(identity, status, stats, appearance, running));
        }
    }

    protected virtual void FindAndUpdateEntity(NetworkIdentity identity, TStatus status, TStats stats,
        TAppearance appearance, bool running)
    {
        // Don't need to block thread
        Task task = new Task(async () =>
        {
            var entity = await WorldSpawner.Instance.GetEntityAsync(identity.Id);
            if (entity != null)
            {
                _eventProcessor.QueueEvent(() => UpdateEntity(entity, identity, status, stats, appearance, running));
            }
        });
        task.Start();
    }

    protected abstract void SpawnEntity(NetworkIdentity identity, TStatus status,
        TStats stats, TAppearance appearance, bool running);

    protected abstract void UpdateEntity(Entity entity, NetworkIdentity identity,
        TStatus status, TStats stats, TAppearance appearance, bool running);

    protected abstract void AddEntity(NetworkIdentity identity, Entity entity);
}