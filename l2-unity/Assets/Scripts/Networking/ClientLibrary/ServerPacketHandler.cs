using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ServerPacketHandler
{
    private AsynchronousClient _client;
    private long _timestamp;
    private CancellationTokenSource _tokenSource;
    private EventProcessor _eventProcessor;

    private static ServerPacketHandler _instance;
    public static ServerPacketHandler Instance { 
        get { 
            if (_instance == null) {
                _instance = new ServerPacketHandler();
            }
            return _instance; 
        } 
    }

    public void SetClient(AsynchronousClient client) {
        _client = client;
        _tokenSource = new CancellationTokenSource();
        _eventProcessor = EventProcessor.Instance;
    }

    public async Task HandlePacketAsync(byte[] data) {
        await Task.Run(() => {
            ServerPacketType packetType = (ServerPacketType)data[0];
            if(DefaultClient.Instance.LogReceivedPackets && packetType != ServerPacketType.Ping) {
                Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] Received packet:" + packetType);
            }
            switch(packetType) {
                case ServerPacketType.Ping:
                    OnPingReceive();
                    break;
                case ServerPacketType.AuthResponse:
                    OnAuthReceive(data);
                    break;
                case ServerPacketType.MessagePacket:
                    OnMessageReceive(data);
                    break;
                case ServerPacketType.SystemMessage:
                    OnSystemMessageReceive(data);
                    break;
                case ServerPacketType.PlayerInfo:
                    OnPlayerInfoReceive(data);
                    break;
                case ServerPacketType.ObjectPosition:
                    OnUpdatePosition(data);
                    break;
                case ServerPacketType.RemoveObject:
                    OnRemoveObject(data);
                    break;
                case ServerPacketType.ObjectRotation:
                    OnUpdateRotation(data);
                    break;
                case ServerPacketType.ObjectAnimation:
                    OnUpdateAnimation(data);
                    break;
                case ServerPacketType.ApplyDamage:
                    OnInflictDamage(data);
                    break;
                case ServerPacketType.NpcInfo:
                    OnNpcInfoReceive(data);
                    break;
                case ServerPacketType.ObjectMoveTo:
                    OnObjectMoveTo(data);
                    break;
                case ServerPacketType.UserInfo:
                    OnUserInfoReceive(data);
                    break;
                case ServerPacketType.ObjectMoveDirection:
                    OnUpdateMoveDirection(data);
                    break;
                case ServerPacketType.GameTime:
                    OnUpdateGameTime(data);
                    break;
                case ServerPacketType.EntitySetTarget:
                    OnEntitySetTarget(data);
                    break;
                case ServerPacketType.AutoAttackStart:
                    OnEntityAutoAttackStart(data);
                    break;
                case ServerPacketType.AutoAttackStop:
                    OnEntityAutoAttackStop(data);
                    break;
                case ServerPacketType.ActionFailed:
                    OnActionFailed(data);
                    break;
            }
        });
    }

    public void CancelTokens() {
        _tokenSource.Cancel();
    }

    private void OnPingReceive() {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int ping = _timestamp != 0 ? (int)(now - _timestamp) : 0;
        //Debug.Log("Ping: " + ping + "ms");
        _client.Ping = ping;

        Task.Delay(1000).ContinueWith(t => {
            if(!_tokenSource.IsCancellationRequested) {
                ClientPacketHandler.Instance.SendPing();
                _timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }

            Task.Delay(DefaultClient.Instance.ConnectionTimeoutMs).ContinueWith(t => {
                if(!_tokenSource.IsCancellationRequested) {
                    long now2 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    if(now2 - _timestamp >= DefaultClient.Instance.ConnectionTimeoutMs) {
                        Debug.Log("Connection timed out");
                        DefaultClient.Instance.Disconnect();
                    }
                }
            }, _tokenSource.Token);
        }, _tokenSource.Token);
    }

    private void OnAuthReceive(byte[] data) {
        AuthResponsePacket packet = new AuthResponsePacket(data);
        AuthResponse response = packet.Response;

        switch(response) {
            case AuthResponse.ALLOW:
                _eventProcessor.QueueEvent(() => DefaultClient.Instance.OnConnectionAllowed());
                break;
            case AuthResponse.ALREADY_CONNECTED:
                Debug.Log("User already connected.");
                _client.Disconnect();
                break;
            case AuthResponse.INVALID_USERNAME:
                Debug.Log("Incorrect user name.");
                _client.Disconnect();
                break;
        }
    }

    private void OnMessageReceive(byte[] data) {
        ReceiveMessagePacket packet = new ReceiveMessagePacket(data);
        String sender = packet.Sender;
        String text = packet.Text;
        ChatMessage message = new ChatMessage(sender, text);
        _eventProcessor.QueueEvent(() => ChatWindow.Instance.ReceiveChatMessage(message));
    }

    private void OnSystemMessageReceive(byte[] data) {
        SystemMessagePacket packet = new SystemMessagePacket(data);
        SystemMessage message = packet.Message;
        _eventProcessor.QueueEvent(() => ChatWindow.Instance.ReceiveChatMessage(message));
    }

    private void OnPlayerInfoReceive(byte[] data) {
        PlayerInfoPacket packet = new PlayerInfoPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.SpawnPlayer(packet.Identity, packet.Status, packet.Stats, packet.Appearance));
    }

    private void OnUserInfoReceive(byte[] data) {
        UserInfoPacket packet = new UserInfoPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.SpawnUser(packet.Identity, packet.Status, packet.Stats, packet.Appearance));
    }

    private void OnUpdatePosition(byte[] data) {
        UpdatePositionPacket packet = new UpdatePositionPacket(data);
        int id = packet.Id;
        Vector3 position = packet.Position;
        _eventProcessor.QueueEvent(() => World.Instance.UpdateObjectPosition(id, position));
    }

    private void OnRemoveObject(byte[] data) {
        RemoveObjectPacket packet = new RemoveObjectPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.RemoveObject(packet.Id));
    }

    private void OnUpdateRotation(byte[] data) {
        UpdateRotationPacket packet = new UpdateRotationPacket(data);
        int id = packet.Id;
        float angle = packet.Angle;
        _eventProcessor.QueueEvent(() => World.Instance.UpdateObjectRotation(id, angle));
    }

    private void OnUpdateAnimation(byte[] data) {
        UpdateAnimationPacket packet = new UpdateAnimationPacket(data);
        int id = packet.Id;
        int animId = packet.AnimId;
        float value = packet.Value;
        _eventProcessor.QueueEvent(() => World.Instance.UpdateObjectAnimation(id, animId, value));
    }

    private void OnInflictDamage(byte[] data) {
        InflictDamagePacket packet = new InflictDamagePacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.InflictDamageTo(packet.SenderId, packet.TargetId, packet.Value, packet.NewHp, packet.CriticalHit)); 
    }

    private void OnNpcInfoReceive(byte[] data) {
        NpcInfoPacket packet = new NpcInfoPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.SpawnNpc(packet.Identity, packet.Status, packet.Stats));
    }

    private void OnObjectMoveTo(byte[] data) {
        ObjectMoveToPacket packet = new ObjectMoveToPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.UpdateObjectDestination(packet.Id, packet.Pos, packet.Speed, packet.Walking));

    }

    private void OnUpdateMoveDirection(byte[] data) {
        UpdateMoveDirectionPacket packet = new UpdateMoveDirectionPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.UpdateObjectMoveDirection(packet.Id, packet.Speed, packet.Direction));
    }

    private void OnUpdateGameTime(byte[] data) {
        GameTimePacket packet = new GameTimePacket(data);
        _eventProcessor.QueueEvent(() => WorldClock.Instance.SynchronizeClock(packet.GameTicks, packet.TickDurationMs, packet.DayDurationMins));
    }

    private void OnEntitySetTarget(byte[] data) {
        EntitySetTargetPacket packet = new EntitySetTargetPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.UpdateEntityTarget(packet.EntityId, packet.TargetId));
    }

    private void OnEntityAutoAttackStart(byte[] data) {
        Debug.Log("OnEntityAutoAttackStart");
        AutoAttackStartPacket packet = new AutoAttackStartPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.EntityStartAutoAttacking(packet.EntityId));
    }

    private void OnEntityAutoAttackStop(byte[] data) {
        Debug.Log("OnEntityAutoAttackStop");
        AutoAttackStopPacket packet = new AutoAttackStopPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.EntityStopAutoAttacking(packet.EntityId));
    }

    private void OnActionFailed(byte[] data) {
        ActionFailedPacket packet = new ActionFailedPacket(data);
        _eventProcessor.QueueEvent(() => PlayerEntity.Instance.OnActionFailed(packet.PlayerAction));
    }
}
