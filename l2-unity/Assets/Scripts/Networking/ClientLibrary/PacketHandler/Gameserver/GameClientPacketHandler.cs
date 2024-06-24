using System.Threading;
using UnityEngine;

public class GameClientPacketHandler : ClientPacketHandler
{
    public void SendPing() {
        PingPacket packet = new PingPacket();
        SendPacket(packet);
    }

    public void SendAuth(string username) {
       AuthRequestPacket packet = new AuthRequestPacket(username);
       SendPacket(packet);
    }

    public void SendMessage(string message) {
        SendMessagePacket packet = new SendMessagePacket(message);
        SendPacket(packet);
    }

    public void UpdatePosition(Vector3 position) {
        RequestMovePacket packet = new RequestMovePacket(position);
        SendPacket(packet);
    }

    public void SendLoadWorld() {
        LoadWorldPacket packet = new LoadWorldPacket();
        SendPacket(packet);
    }

    public void UpdateRotation(float angle) {
        RequestRotatePacket packet = new RequestRotatePacket(angle);
        SendPacket(packet);
    }

    public void UpdateAnimation(byte anim, float value) {
        RequestAnimPacket packet = new RequestAnimPacket(anim, value);
        SendPacket(packet);
    }

    public void InflictAttack(int targetId, AttackType type) {
        RequestAttackPacket packet = new RequestAttackPacket(targetId, type);
        SendPacket(packet);
    }

    public void UpdateMoveDirection(Vector3 direction) {
        RequestMoveDirectionPacket packet = new RequestMoveDirectionPacket(direction);
        SendPacket(packet);
    }

    public void SendRequestSetTarget(int targetId) {
        RequestSetTargetPacket packet = new RequestSetTargetPacket(targetId);
        SendPacket(packet);
    }

    public void SendRequestAutoAttack() {
        RequestAutoAttackPacket packet = new RequestAutoAttackPacket();
        SendPacket(packet);
    }

    public override void SendPacket(ClientPacket packet) {
        if (DefaultClient.Instance.LogSentPackets) {
            GameClientPacketType packetType = (GameClientPacketType)packet.GetPacketType();
            if (packetType != GameClientPacketType.Ping && packetType != GameClientPacketType.RequestRotate) {
                Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [GameServer] Sending packet:" + packetType);
            }
        }

        _client.SendPacket(packet);
    }
}

