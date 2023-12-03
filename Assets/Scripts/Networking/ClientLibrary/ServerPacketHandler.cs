using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

public class ServerPacketHandler
{
    private AsynchronousClient client;
    private long timestamp;
    private CancellationTokenSource tokenSource;
    private static ServerPacketHandler instance;
    private EventProcessor eventProcessor;

    public static ServerPacketHandler GetInstance() {
        if(instance == null) {
            instance = new ServerPacketHandler();
        }

        return instance;
    }
    public void SetClient(AsynchronousClient client) {
        this.client = client;
        tokenSource = new CancellationTokenSource();
        eventProcessor = EventProcessor.GetInstance();
    }

    public AsynchronousClient GetClient() {
        return client;
    }

    public void HandlePacket(byte[] data) {
        ServerPacketType packetType = (ServerPacketType) data[0];
        //Debug.Log("Received packet:" + packetType);
        switch(packetType)
        {
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
                OnRemoveObject(data) ;
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
        }
    }

    public void CancelTokens() {
        tokenSource.Cancel();
    }

    private void OnPingReceive() {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int ping = timestamp != 0 ? (int)(now - timestamp) : 0;
        //Debug.Log("Ping: " + ping + "ms");
        client.SetPing(ping);

        Task.Delay(1000).ContinueWith(t => {
            if(!tokenSource.IsCancellationRequested) {
                ClientPacketHandler.GetInstance().SendPing();
                timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }

            Task.Delay(DefaultClient.GetInstance().connectionTimeoutMs).ContinueWith(t => {
                if(!tokenSource.IsCancellationRequested) {
                    long now2 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    if(now2 - timestamp >= DefaultClient.GetInstance().connectionTimeoutMs) {
                        Debug.Log("Connection timed out");
                        DefaultClient.GetInstance().Disconnect();
                    }
                }
            }, tokenSource.Token);
        }, tokenSource.Token);
    }

    private void OnAuthReceive(byte[] data) {
        AuthResponsePacket packet = new AuthResponsePacket(data);
        AuthResponse response = packet.GetResponse();

        switch(response) {
            case AuthResponse.ALLOW:
                eventProcessor.QueueEvent(() => DefaultClient.GetInstance().OnConnectionAllowed());
                break;
            case AuthResponse.ALREADY_CONNECTED:
                Debug.Log("User already connected.");
                client.Disconnect();
                break;
            case AuthResponse.INVALID_USERNAME:
                Debug.Log("Incorrect user name.");
                client.Disconnect();
                break;
        }
    }

    private void OnMessageReceive(byte[] data) {
        ReceiveMessagePacket packet = new ReceiveMessagePacket(data);
        String sender = packet.GetSender();
        String text = packet.GetText();
        ChatMessage message = new ChatMessage(sender, text);
        eventProcessor.QueueEvent(() => ChatWindow.GetInstance().ReceiveChatMessage(message));
    }
    private void OnSystemMessageReceive(byte[] data) {
        SystemMessagePacket packet = new SystemMessagePacket(data);
        SystemMessage message = packet.GetMessage();
        eventProcessor.QueueEvent(() => ChatWindow.GetInstance().ReceiveChatMessage(message));
    }
    private void OnPlayerInfoReceive(byte[] data) {
        PlayerInfoPacket packet = new PlayerInfoPacket(data);
        NetworkIdentity identity = packet.GetIdentity();
        identity.EntityType = EntityType.Player;
        PlayerStatus status = packet.GetStatus();
        eventProcessor.QueueEvent(() => World.GetInstance().SpawnPlayer(identity, status));
    }
    private void OnUserInfoReceive(byte[] data) {
        UserInfoPacket packet = new UserInfoPacket(data);
        NetworkIdentity identity = packet.GetIdentity();
        identity.EntityType = EntityType.User;
        PlayerStatus status = packet.GetStatus();
        eventProcessor.QueueEvent(() => World.GetInstance().SpawnUser(identity, status));
    }

    private void OnUpdatePosition(byte[] data) {
        UpdatePositionPacket packet = new UpdatePositionPacket(data);
        int id = packet.getId();
        Vector3 position = packet.getPosition();
        eventProcessor.QueueEvent(() => World.GetInstance().UpdateObjectPosition(id, position));
    }
    private void OnRemoveObject(byte[] data) {
        RemoveObjectPacket packet = new RemoveObjectPacket(data);
        eventProcessor.QueueEvent(() => World.GetInstance().RemoveObject(packet.getId()));
    }
    private void OnUpdateRotation(byte[] data) {
        UpdateRotationPacket packet = new UpdateRotationPacket(data);
        int id = packet.getId();
        float angle = packet.getAngle();
        eventProcessor.QueueEvent(() => World.GetInstance().UpdateObjectRotation(id, angle));
    }
    private void OnUpdateAnimation(byte[] data) {
        UpdateAnimationPacket packet = new UpdateAnimationPacket(data);
        int id = packet.getId();
        int animId = packet.getAnimId();
        float value = packet.getValue();
        eventProcessor.QueueEvent(() => World.GetInstance().UpdateObjectAnimation(id, animId, value));
    }
    private void OnInflictDamage(byte[] data) {
        InflictDamagePacket packet = new InflictDamagePacket(data);
        eventProcessor.QueueEvent(() => World.GetInstance().InflictDamageTo(packet.SenderId, packet.TargetId, packet.AttackId, packet.Value)); 
    }
    private void OnNpcInfoReceive(byte[] data) {
        NpcInfoPacket packet = new NpcInfoPacket(data);
        NetworkIdentity identity = packet.GetIdentity();
        identity.EntityType = EntityType.NPC;
        NpcStatus status = packet.GetStatus();

        eventProcessor.QueueEvent(() => World.GetInstance().SpawnNpc(identity, status));
    }
    private void OnObjectMoveTo(byte[] data) {
        ObjectMoveToPacket packet = new ObjectMoveToPacket(data);
        int id = packet.getId();
        Vector3 position = packet.getPosition();
        eventProcessor.QueueEvent(() => World.GetInstance().UpdateObjectDestination(id, position));
        eventProcessor.QueueEvent(() => World.GetInstance().UpdateObjectAnimation(id, 0, 1));
    }
    private void OnUpdateMoveDirection(byte[] data) {
        UpdateMoveDirectionPacket packet = new UpdateMoveDirectionPacket(data);
        eventProcessor.QueueEvent(() => World.GetInstance().UpdateObjectMoveDirection(packet.getId(), packet.getSpeed(), packet.getDirection()));
    }
}
