using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;

public class GameServerPacketHandler : ServerPacketHandler
{
    public override void HandlePacket(byte[] data) {
        GameServerPacketType packetType = (GameServerPacketType)data[0];
        if (GameClient.Instance.LogReceivedPackets && packetType != GameServerPacketType.Ping) {
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [GameServer] Received packet:" + packetType);
        }

        switch (packetType) {
            case GameServerPacketType.Ping:
                OnPingReceive();
                break;
            case GameServerPacketType.Key:
                OnKeyReceive(data);
                break;
            case GameServerPacketType.LoginFail:
                OnLoginFail(data);
                break;
            case GameServerPacketType.CharSelectionInfo:
                OnCharSelectionInfoReceive(data);
                break;
            case GameServerPacketType.MessagePacket:
                OnMessageReceive(data);
                break;
            case GameServerPacketType.SystemMessage:
                OnSystemMessageReceive(data);
                break;
            case GameServerPacketType.PlayerInfo:
                OnPlayerInfoReceive(data);
                break;
            case GameServerPacketType.ObjectPosition:
                OnUpdatePosition(data);
                break;
            case GameServerPacketType.RemoveObject:
                OnRemoveObject(data);
                break;
            case GameServerPacketType.ObjectRotation:
                OnUpdateRotation(data);
                break;
            case GameServerPacketType.ObjectAnimation:
                OnUpdateAnimation(data);
                break;
            case GameServerPacketType.ApplyDamage:
                OnInflictDamage(data);
                break;
            case GameServerPacketType.NpcInfo:
                OnNpcInfoReceive(data);
                break;
            case GameServerPacketType.ObjectMoveTo:
                OnObjectMoveTo(data);
                break;
            case GameServerPacketType.UserInfo:
                OnUserInfoReceive(data);
                break;
            case GameServerPacketType.ObjectMoveDirection:
                OnUpdateMoveDirection(data);
                break;
            case GameServerPacketType.GameTime:
                OnUpdateGameTime(data);
                break;
            case GameServerPacketType.EntitySetTarget:
                OnEntitySetTarget(data);
                break;
            case GameServerPacketType.AutoAttackStart:
                OnEntityAutoAttackStart(data);
                break;
            case GameServerPacketType.AutoAttackStop:
                OnEntityAutoAttackStop(data);
                break;
            case GameServerPacketType.ActionFailed:
                OnActionFailed(data);
                break;
            case GameServerPacketType.ServerClose:
                OnServerClose();
                break;
        }
    }

    protected override byte[] DecryptPacket(byte[] data) {
        if(GameClient.Instance.LogCryptography) {
            Debug.Log("<---- [GAME] ENCRYPTED: " + StringUtils.ByteArrayToString(data));
        }

        GameClient.Instance.GameCrypt.Decrypt(data);

        if (GameClient.Instance.LogCryptography) {
            Debug.Log("<---- [GAME] DECRYPTED: " + StringUtils.ByteArrayToString(data));
        }

        return data;
    }

    private void OnPingReceive() {
        long now = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        int ping = _timestamp != 0 ? (int)(now - _timestamp) : 0;
        GameClient.Instance.Ping = ping;

        Task.Delay(1000).ContinueWith(t => {
            if (!_tokenSource.IsCancellationRequested) {
                ((GameClientPacketHandler)_clientPacketHandler).SendPing();
                _timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }

            Task.Delay(GameClient.Instance.ConnectionTimeoutMs + 100).ContinueWith(t => {
                if (!_tokenSource.IsCancellationRequested) {
                    long now2 = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                    if (now2 - _timestamp >= GameClient.Instance.ConnectionTimeoutMs) {
                        Debug.LogWarning("Connection timed out");
                        _client.Disconnect();
                    }
                }
            }, _tokenSource.Token);
        }, _tokenSource.Token);
    }

    private void OnKeyReceive(byte[] data) {
        KeyPacket packet = new KeyPacket(data);

        if (!packet.AuthAllowed) {
            Debug.LogWarning("Gameserver connect not allowed.");
            EventProcessor.Instance.QueueEvent(() => GameClient.Instance.Disconnect());
            EventProcessor.Instance.QueueEvent(() => LoginClient.Instance.Disconnect());
            return;
        }

        GameClient.Instance.EnableCrypt(packet.BlowFishKey);

        _eventProcessor.QueueEvent(() => ((GameClientPacketHandler)_clientPacketHandler).SendAuth());

        _eventProcessor.QueueEvent(() => ((GameClientPacketHandler)_clientPacketHandler).SendPing());
    }

    private void OnLoginFail(byte[] data) {
        PlayFailPacket packet = new PlayFailPacket(data);
        EventProcessor.Instance.QueueEvent(() => GameClient.Instance.Disconnect());
        EventProcessor.Instance.QueueEvent(() => LoginClient.Instance.Disconnect());

        Debug.LogWarning($"Gameserver login failed reason: " +
            $"{Enum.GetName(typeof(LoginFailPacket.LoginFailedReason), packet.FailedReason)}");
    }

    private void OnCharSelectionInfoReceive(byte[] data) {
        CharSelectionInfoPacket packet = new CharSelectionInfoPacket(data);

        Debug.Log($"Received {packet.Characters.Count} character(s) from server.");

        EventProcessor.Instance.QueueEvent(() => {
            CharSelectWindow.Instance.SetCharacterList(packet.Characters);
            CharSelectWindow.Instance.SelectSlot(packet.SelectedSlotId);
            CharacterSelector.Instance.SetCharacterList(packet.Characters);
            CharacterSelector.Instance.SelectCharacter(packet.SelectedSlotId);
            LoginClient.Instance.Disconnect();
            GameClient.Instance.OnAuthAllowed();
        });
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
        _eventProcessor.QueueEvent(() => {
            GameClient.Instance.PlayerInfo = packet.PacketPlayerInfo;

            GameManager.Instance.OnCharacterSelect();
        });
    }

    private void OnUserInfoReceive(byte[] data) {
        UserInfoPacket packet = new UserInfoPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.SpawnUser(packet.Identity, packet.Status, packet.Stats, packet.Appearance));
    }

    private void OnUpdatePosition(byte[] data) {
        UpdatePositionPacket packet = new UpdatePositionPacket(data);
        int id = packet.Id;
        Vector3 position = packet.Position;
        World.Instance.UpdateObjectPosition(id, position);
    }

    private void OnRemoveObject(byte[] data) {
        RemoveObjectPacket packet = new RemoveObjectPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.RemoveObject(packet.Id));
    }

    private void OnUpdateRotation(byte[] data) {
        UpdateRotationPacket packet = new UpdateRotationPacket(data);
        int id = packet.Id;
        float angle = packet.Angle;
        World.Instance.UpdateObjectRotation(id, angle);
    }

    private void OnUpdateAnimation(byte[] data) {
        UpdateAnimationPacket packet = new UpdateAnimationPacket(data);
        int id = packet.Id;
        int animId = packet.AnimId;
        float value = packet.Value;

        World.Instance.UpdateObjectAnimation(id, animId, value);
    }

    private void OnInflictDamage(byte[] data) {
        InflictDamagePacket packet = new InflictDamagePacket(data);
        World.Instance.InflictDamageTo(packet.SenderId, packet.TargetId, packet.Value, packet.NewHp, packet.CriticalHit); 
    }

    private void OnNpcInfoReceive(byte[] data) {
        NpcInfoPacket packet = new NpcInfoPacket(data);
        _eventProcessor.QueueEvent(() => World.Instance.SpawnNpc(packet.Identity, packet.Status, packet.Stats));
    }

    private void OnObjectMoveTo(byte[] data) {
        ObjectMoveToPacket packet = new ObjectMoveToPacket(data);
        World.Instance.UpdateObjectDestination(packet.Id, packet.Pos, packet.Speed, packet.Walking);

    }

    private void OnUpdateMoveDirection(byte[] data) {
        UpdateMoveDirectionPacket packet = new UpdateMoveDirectionPacket(data);
        World.Instance.UpdateObjectMoveDirection(packet.Id, packet.Speed, packet.Direction);
    }

    private void OnUpdateGameTime(byte[] data) {
        GameTimePacket packet = new GameTimePacket(data);
        WorldClock.Instance.SynchronizeClock(packet.GameTicks, packet.TickDurationMs, packet.DayDurationMins);
    }

    private void OnEntitySetTarget(byte[] data) {
        EntitySetTargetPacket packet = new EntitySetTargetPacket(data);
        World.Instance.UpdateEntityTarget(packet.EntityId, packet.TargetId);
    }

    private void OnEntityAutoAttackStart(byte[] data) {
        Debug.LogWarning("OnEntityAutoAttackStart");
        AutoAttackStartPacket packet = new AutoAttackStartPacket(data);
        World.Instance.EntityStartAutoAttacking(packet.EntityId);
    }

    private void OnEntityAutoAttackStop(byte[] data) {
        Debug.LogWarning("OnEntityAutoAttackStop");
        AutoAttackStopPacket packet = new AutoAttackStopPacket(data);
        World.Instance.EntityStopAutoAttacking(packet.EntityId);
    }

    private void OnActionFailed(byte[] data) {
        ActionFailedPacket packet = new ActionFailedPacket(data);
        Debug.LogWarning($"Action failed");
        _eventProcessor.QueueEvent(() => PlayerEntity.Instance.OnActionFailed(packet.PlayerAction));
    }

    private void OnServerClose() {
        Debug.Log("ServerClose received from Gameserver");
        _client.Disconnect();
    }
}
