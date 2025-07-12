using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameClientPacketHandler : ClientPacketHandler
{
    protected override void EncryptPacket(ClientPacket packet)
    {
        byte[] data = packet.GetData();

        GameClient.Instance.GameCrypt.Encrypt(data);

        if (GameClient.Instance.LogCryptography)
        {
            Debug.Log("----> [GAME] ENCRYPTED: " + StringUtils.ByteArrayToString(data));
        }

        packet.SetData(data);
    }

    public void SendPing()
    {
        PingPacket packet = new PingPacket();
        SendPacket(packet);
    }

    public void SendProtocolVersion()
    {
        ProtocolVersionPacket packet = new ProtocolVersionPacket(GameManager.Instance.ProtocolVersion);
        SendPacket(packet);
    }

    public void SendAuth()
    {
        GameAuthRequestPacket authPacket =
            new GameAuthRequestPacket(LoginClient.Instance.Account, GameClient.Instance.PlayKey1,
                GameClient.Instance.PlayKey2,
                GameClient.Instance.SessionKey1, GameClient.Instance.SessionKey2);


        SendPacket(authPacket);
    }

    public void SendMessage(string message, L2MessageType messageType, string pmTarget)
    {
        SendMessagePacket packet = new SendMessagePacket(message, messageType, pmTarget);
        SendPacket(packet);
    }

    public void ValidatePosition(Vector3 position, int heading)
    {
        ValidatePositionPacket packet = new ValidatePositionPacket(position, heading);
        SendPacket(packet);
    }

    public void SendLoadWorld()
    {
        EnterWorldPacket packet = new EnterWorldPacket();
        SendPacket(packet);
    }

    public void UpdateRotation(float angle)
    {
        RequestRotatePacket packet = new RequestRotatePacket(angle);
        SendPacket(packet);
    }

    public void UpdateAnimation(byte anim, float value)
    {
        RequestAnimPacket packet = new RequestAnimPacket(anim, value);
        SendPacket(packet);
    }

    public void RequestAttackForce(int targetId)
    {
        RequestAttackPacket packet = new RequestAttackPacket(targetId);
        SendPacket(packet);
    }

    public void UpdateMoveDirection(Vector3 direction, int heading, float verticalVelocity, Vector3 position, bool requireReply)
    {
        // Debug.LogWarning("Sharing move direction: " + direction);
        RequestMoveDirectionPacket packet = new RequestMoveDirectionPacket(direction, heading, verticalVelocity, position, requireReply);
        SendPacket(packet);
    }

    public void SendRequestSetTarget(int targetId)
    {
        RequestSetTargetPacket packet = new RequestSetTargetPacket(targetId, false);
        SendPacket(packet);
    }

    public void SendRequestCancel(bool cancelCast)
    {
        RequestCancelPacket packet = new RequestCancelPacket(cancelCast);
        SendPacket(packet);
    }

    public void SendRequestAction(int objectId)
    {
        RequestActionPacket packet = new RequestActionPacket(objectId);
        SendPacket(packet);
    }

    public void SendRequestSelectCharacter(int slot)
    {
        RequestCharSelectPacket packet = new RequestCharSelectPacket(slot);
        SendPacket(packet);
    }

    public void SendRequestOpenInventory()
    {
        RequestInventoryOpenPacket packet = new RequestInventoryOpenPacket();
        SendPacket(packet);
    }

    public override void SendPacket(ClientPacket packet)
    {
        if (GameClient.Instance.LogSentPackets)
        {
            GameClientPacketType packetType = (GameClientPacketType)packet.GetPacketType();
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [GameServer] Sending packet:" + packetType);
        }

        if (GameClient.Instance.LogCryptography)
        {
            Debug.Log("----> [GAME] CLEAR: " + StringUtils.ByteArrayToString(packet.GetData()));
        }

        if (_client.CryptEnabled)
        {
            EncryptPacket(packet);
        }

        _client.SendPacket(packet);
    }

    public void UseItem(int objectId)
    {
        UseItemPacket packet = new UseItemPacket(objectId);
        SendPacket(packet);
    }

    public void UpdateInventoryOrder(List<InventoryOrder> orders)
    {
        RequestInventoryUpdateOrderPacket packet = new RequestInventoryUpdateOrderPacket(orders);
        SendPacket(packet);
    }

    public void DestroyItem(int objectId, int quantity)
    {
        RequestDestroyItemPacket packet = new RequestDestroyItemPacket(objectId, quantity);
        SendPacket(packet);
    }

    public void DropItem(int objectId, int quantity)
    {
        RequestDropItemPacket packet = new RequestDropItemPacket(objectId, quantity);
        SendPacket(packet);
    }

    public void RequestDisconnect()
    {
        DisconnectPacket packet = new DisconnectPacket();
        SendPacket(packet);
    }

    public void RequestRestart()
    {
        RequestRestartPacket packet = new RequestRestartPacket();
        SendPacket(packet);
    }

    public void RequestAddShortcut(int type, int id, int slot)
    {
        RequestShortcutRegPacket packet = new RequestShortcutRegPacket(type, id, slot);
        SendPacket(packet);
    }

    public void RequestRemoveShortcut(int oldSlot)
    {
        RequestShortcutDelPacket packet = new RequestShortcutDelPacket(oldSlot);
        SendPacket(packet);
    }

    public void RequestActionUse(int actionId)
    {
        bool isControlPressed = false;
        bool isShiftPressed = false;
        RequestActionUsePacket packet = new RequestActionUsePacket(actionId, isControlPressed, isShiftPressed);
        SendPacket(packet);
    }

    public void SendRequestCreateCharacter(string name, CharacterRace race, CharacterSex sex, CharacterClass clazz,
        int hairstyle, int haircolor, int face)
    {
        RequestCharCreatePacket packet =
            new RequestCharCreatePacket(name, race, sex, clazz, hairstyle, haircolor, face);
        SendPacket(packet);
    }

    public void SendRequestRestartPoint(int restartPoint)
    {
        RequestRestartPointPacket packet = new RequestRestartPointPacket(restartPoint);
        SendPacket(packet);
    }

    public void NotifyAppearing()
    {
        AppearingPacket packet = new AppearingPacket();
        SendPacket(packet);
    }

    public void RequestBypassToServer(string htmlCommand)
    {
        RequestBypassToServerPacket packet = new RequestBypassToServerPacket(htmlCommand);
        SendPacket(packet);
    }

    public void RequestAutoSoulshot(int id, bool toggled)
    {
        RequestAutoSoulshotPacket packet = new RequestAutoSoulshotPacket(id, !toggled);
        SendPacket(packet);
    }

    public void SendGMCommand(string command)
    {
        GMCommandPacket packet = new GMCommandPacket(command);
        SendPacket(packet);
    }

    public void SendRequestDeleteCharacter(int slot)
    {
        RequestCharDeletePacket packet = new RequestCharDeletePacket(slot);
        SendPacket(packet);
    }

    public void SendRequestRestoreCharacter(int slot)
    {
        RequestCharRestorePacket packet = new RequestCharRestorePacket(slot);
        SendPacket(packet);
    }

    public void SendRequestSkillList()
    {
        RequestSkillListPacket packet = new RequestSkillListPacket();
        SendPacket(packet);
    }

    public void SendRequestAcquireSkill(int skillId, int skillLvl, PacketSkillType skillType)
    {
        RequestAcquireSkillPacket packet = new RequestAcquireSkillPacket(skillId, skillLvl, skillType);
        SendPacket(packet);
    }

    public void SendRequestAcquireSkillInfo(int skillId, int skillLvl, PacketSkillType skillType)
    {
        RequestAcquireSkillInfoPacket packet = new RequestAcquireSkillInfoPacket(skillId, skillLvl, skillType);
        SendPacket(packet);
    }

    public void SendRequestSellItem(int listId, List<Product> products)
    {
        RequestSellItemPacket packet = new RequestSellItemPacket(listId, products);
        SendPacket(packet);
    }

    public void SendRequestBuyItem(int listId, List<Product> products)
    {
        RequestBuyItemPacket packet = new RequestBuyItemPacket(listId, products);
        SendPacket(packet);
    }

    public void RequestMagicSkillUse(int skillId, bool ctrlPressed, bool shiftPressed)
    {
        RequestMagicSkillUsePacket packet = new RequestMagicSkillUsePacket(skillId, ctrlPressed, shiftPressed);
        SendPacket(packet);
    }
}