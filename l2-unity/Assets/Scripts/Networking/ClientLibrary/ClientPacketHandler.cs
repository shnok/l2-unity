using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

public class ClientPacketHandler
{
    private static ClientPacketHandler instance;

    public static ClientPacketHandler GetInstance() {
        if(instance == null) {
            instance = new ClientPacketHandler();
        }

        return instance;
    }

    private AsynchronousClient client;
    
    public void SetClient(AsynchronousClient client) {
        this.client = client;
    }

    public void SendPing() {
        PingPacket packet = new PingPacket();
        client.SendPacket(packet);
    }

    public void SendAuth(string username) {
       AuthRequestPacket packet = new AuthRequestPacket(username);
       client.SendPacket(packet);
    }

    public void SendMessage(string message) {
        SendMessagePacket packet = new SendMessagePacket(message);
        client.SendPacket(packet);
    }

    public void UpdatePosition(Vector3 position) {
        RequestMovePacket packet = new RequestMovePacket(position);
        client.SendPacket(packet);
    }

    public void SendLoadWorld() {
        LoadWorldPacket packet = new LoadWorldPacket();
        client.SendPacket(packet);
    }

    public void UpdateRotation(float angle) {
        RequestRotatePacket packet = new RequestRotatePacket(angle);
        client.SendPacket(packet);
    }

    public void UpdateAnimation(byte anim, float value) {
        RequestAnimPacket packet = new RequestAnimPacket(anim, value);
        client.SendPacket(packet);
    }

    public void InflictAttack(int targetId, AttackType type) {
        InflictAttackPacket packet = new InflictAttackPacket(targetId, type);
        client.SendPacket(packet);
    }

    public void UpdateMoveDirection(float speed, Vector3 direction) {
        RequestMoveDirectionPacket packet = new RequestMoveDirectionPacket(speed, direction);
        client.SendPacket(packet);
    }
}
