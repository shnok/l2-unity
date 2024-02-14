using UnityEngine;

public class ClientPacketHandler
{
    private AsynchronousClient _client;

    private static ClientPacketHandler _instance;
    public static ClientPacketHandler Instance {
        get {
            if(_instance == null) {
                _instance = new ClientPacketHandler();
            }
            return _instance;
        }
    }
    
    public void SetClient(AsynchronousClient client) {
        _client = client;
    }

    public void SendPing() {
        PingPacket packet = new PingPacket();
        _client.SendPacket(packet);
    }

    public void SendAuth(string username) {
       AuthRequestPacket packet = new AuthRequestPacket(username);
       _client.SendPacket(packet);
    }

    public void SendMessage(string message) {
        SendMessagePacket packet = new SendMessagePacket(message);
        _client.SendPacket(packet);
    }

    public void UpdatePosition(Vector3 position) {
        RequestMovePacket packet = new RequestMovePacket(position);
        _client.SendPacket(packet);
    }

    public void SendLoadWorld() {
        LoadWorldPacket packet = new LoadWorldPacket();
        _client.SendPacket(packet);
    }

    public void UpdateRotation(float angle) {
        RequestRotatePacket packet = new RequestRotatePacket(angle);
        _client.SendPacket(packet);
    }

    public void UpdateAnimation(byte anim, float value) {
        RequestAnimPacket packet = new RequestAnimPacket(anim, value);
        _client.SendPacket(packet);
    }

    public void InflictAttack(int targetId, AttackType type) {
        RequestAttackPacket packet = new RequestAttackPacket(targetId, type);
        _client.SendPacket(packet);
    }

    public void UpdateMoveDirection(Vector3 direction) {
        RequestMoveDirectionPacket packet = new RequestMoveDirectionPacket(direction);
        _client.SendPacket(packet);
    }

    public void SendRequestSetTarget(int targetId) {
        RequestSetTargetPacket packet = new RequestSetTargetPacket(targetId);
        _client.SendPacket(packet);
    }
}
