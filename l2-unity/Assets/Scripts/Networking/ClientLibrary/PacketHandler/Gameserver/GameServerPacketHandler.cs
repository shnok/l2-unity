using UnityEngine;
using System;
using System.Threading;

public class GameServerPacketHandler : ServerPacketHandler
{
    public override void HandlePacket(byte[] data)
    {
        GameServerPacketType packetType = (GameServerPacketType)data[0];
        if (GameClient.Instance.LogReceivedPackets)
        {
            Debug.Log("[" + Thread.CurrentThread.ManagedThreadId + "] [GameServer] Received packet:" + packetType);
        }

        switch (packetType)
        {
            case GameServerPacketType.VersionCheck:
                OnKeyReceive(data);
                break;
            case GameServerPacketType.LoginFail:
                OnLoginFail(data);
                break;
            case GameServerPacketType.CharSelectionInfo:
                OnCharSelectionInfoReceive(data);
                break;
            case GameServerPacketType.CreatureSay:
                OnMessageReceive(data);
                break;
            case GameServerPacketType.SystemMessage:
                OnSystemMessageReceive(data);
                break;
            case GameServerPacketType.CharSelected:
                OnCharSelected(data);
                break;
            case GameServerPacketType.ObjectPosition:
                OnUpdatePosition(data);
                break;
            case GameServerPacketType.RemoveObject:
                OnRemoveObject(data);
                break;
            case GameServerPacketType.Attack:
                OnEntityAttack(data);
                break;
            case GameServerPacketType.NpcInfo:
                OnNpcInfoReceive(data);
                break;
            case GameServerPacketType.ObjectMoveTo:
                OnObjectMoveTo(data);
                break;
            case GameServerPacketType.PlayerInfo:
                OnPlayerInfoReceive(data);
                break;
            case GameServerPacketType.ObjectMoveDirection:
                OnUpdateMoveDirection(data);
                break;
            case GameServerPacketType.EntityTargetSet:
                OnEntityTargetSet(data);
                break;
            case GameServerPacketType.TargetUnselected:
                OnEntityTargetUnset(data);
                break;
            case GameServerPacketType.MyTargetSet:
                OnMyTargetSet(data);
                break;
            case GameServerPacketType.ActionFailed:
                OnActionFailed(data);
                break;
            case GameServerPacketType.ServerClose:
                OnServerClose();
                break;
            case GameServerPacketType.StatusUpdate:
                OnStatusUpdate(data);
                break;
            case GameServerPacketType.ActionAllowed:
                OnActionAllowed(data);
                break;
            case GameServerPacketType.InventoryItemList:
                OnInventoryItemList(data);
                break;
            case GameServerPacketType.InventoryUpdate:
                OnInventoryUpdate(data);
                break;
            case GameServerPacketType.LeaveWorld:
                OnLeaveWorld(data);
                break;
            case GameServerPacketType.RestartReponse:
                OnRestartResponse(data);
                break;
            case GameServerPacketType.ShortcutInit:
                OnShortcutInit(data);
                break;
            case GameServerPacketType.ShortcutRegister:
                OnShortcutRegister(data);
                break;
            case GameServerPacketType.ShortcutDelete:
                OnShortcutDelete(data);
                break;
            case GameServerPacketType.ChangeWaitType:
                OnChangeWaitType(data);
                break;
            case GameServerPacketType.ChangeMoveType:
                OnChangeMoveType(data);
                break;
            case GameServerPacketType.CharCreateOk:
                OnCharCreateOk(data);
                break;
            case GameServerPacketType.CharCreateFail:
                OnCharCreateFail(data);
                break;
            case GameServerPacketType.ValidateLocation:
                OnValidateLocation(data);
                break;
            case GameServerPacketType.DoDie:
                OnEntityDie(data);
                break;
            case GameServerPacketType.Revive:
                OnEntityRevive(data);
                break;
            case GameServerPacketType.TeleportToLocation:
                OnTeleportToLocation(data);
                break;
            case GameServerPacketType.StopMove:
                OnEntityStopMove(data);
                break;
            case GameServerPacketType.NpcHtml:
                OnNpcHtmlReceive(data);
                break;
            case GameServerPacketType.ExAutoSoulshot:
                OnExAutoSoulshot(data);
                break;
            case GameServerPacketType.MagicSkillUse:
                OnMagicSkillUse(data);
                break;
            case GameServerPacketType.UserInfo:
                OnUserInfoReceived(data);
                break;
            case GameServerPacketType.SocialAction:
                OnSocialActionReceived(data);
                break;
            case GameServerPacketType.SetupGauge:
                OnSetupGauge(data);
                break;
            case GameServerPacketType.RelationChanged:
                OnRelationChanged(data);
                break;
            case GameServerPacketType.CharDeleteOk:
                OnCharDeleteOk(data);
                break;
            case GameServerPacketType.CharDeleteFail:
                OnCharDeleteFail(data);
                break;
            case GameServerPacketType.SkillList:
                OnSkillList(data);
                break;
            case GameServerPacketType.SkillCoolTime:
                OnSkillCoolTime(data);
                break;
            case GameServerPacketType.AcquireSkillLearnList:
                OnAcquireSkillLearnList(data);
                break;
            case GameServerPacketType.AcquireSkillLearnInfo:
                OnAcquireSkillLearnInfo(data);
                break;
            case GameServerPacketType.AcquireSkillLearnDone:
                OnAcquireSkillDone(data);
                break;
            case GameServerPacketType.BuyList:
                OnBuyListReceived(data);
                break;
            case GameServerPacketType.SellList:
                OnSellListReceived(data);
                break;
            case GameServerPacketType.MagicSkillLaunched:
                OnSkillLaunched(data);
                break;
            case GameServerPacketType.ShortBuffStatusUpdate:
                OnShortBuffStatusUpdate(data);
                break;
            case GameServerPacketType.AbnormalStatusUpdate:
                OnAbnormalStatusUpdate(data);
                break;
            case GameServerPacketType.PartyEffect:
                OnPartyEffect(data);
                break;
            case GameServerPacketType.EtcStatusUpdate:
                OnEtcStatusUpdate(data);
                break;
            case GameServerPacketType.FightStanceStart:
                OnFightStanceStart(data);
                break;
            case GameServerPacketType.FightStanceStop:
                OnFightStanceStop(data);
                break;
            case GameServerPacketType.MagicSkillCanceled:
                OnMagicSkillCanceled(data);
                break;
            default:
                Debug.LogWarning($"Received unhandled packet with OPCode [{packetType}].");
                break;
        }
    }

    protected override byte[] DecryptPacket(byte[] data)
    {
        if (GameClient.Instance.LogCryptography)
        {
            Debug.Log("<---- [GAME] ENCRYPTED: " + StringUtils.ByteArrayToString(data));
        }

        GameClient.Instance.GameCrypt.Decrypt(data);

        if (GameClient.Instance.LogCryptography)
        {
            Debug.Log("<---- [GAME] CLEAR: " + StringUtils.ByteArrayToString(data));
        }

        return data;
    }

    private void OnKeyReceive(byte[] data)
    {
        VersionCheckPacket packet = new VersionCheckPacket(data);

        if (!packet.AuthAllowed)
        {
            Debug.LogWarning("Gameserver connect not allowed.");
            EventProcessor.Instance.QueueEvent(() => GameClient.Instance.Disconnect());
            EventProcessor.Instance.QueueEvent(() => LoginClient.Instance.Disconnect());
            return;
        }

        GameClient.Instance.EnableCrypt(packet.BlowFishKey);

        _eventProcessor.QueueEvent(() => ((GameClientPacketHandler)_clientPacketHandler).SendAuth());

        //_eventProcessor.QueueEvent(() => ((GameClientPacketHandler)_clientPacketHandler).SendPing());
    }

    private void OnLoginFail(byte[] data)
    {
        LoginFailPacket packet = new LoginFailPacket(data);

        EventProcessor.Instance.QueueEvent(() => GameClient.Instance.Disconnect());
        EventProcessor.Instance.QueueEvent(() => LoginClient.Instance.Disconnect());

        Debug.LogWarning($"Gameserver login failed reason: " +
            $"{Enum.GetName(typeof(LoginServerFailPacket.LoginFailedReason), packet.FailedReason)}");
    }

    private void OnCharSelectionInfoReceive(byte[] data)
    {
        CharSelectionInfoPacket packet = new CharSelectionInfoPacket(data);

        CharacterSelector.Instance.Characters = packet.Characters;
        CharacterSelector.Instance.DefaultSelectedSlot = packet.SelectedSlotId;

        Debug.Log($"Received {packet.Characters.Count} character(s) from server.");

        if (GameManager.Instance.State == GameState.LOGIN_AUTHED)
        {
            EventProcessor.Instance.QueueEvent(() =>
            {
                LoginClient.Instance.Disconnect();
                GameManager.Instance.NotifyEvent(GameEvent.AUTH_ALLOWED);
            });
        }
        else if (GameManager.Instance.State == GameState.CHAR_SELECT || GameManager.Instance.State == GameState.CHAR_CREATION)
        {
            EventProcessor.Instance.QueueEvent(() =>
            {
                GameManager.Instance.NotifyEvent(GameEvent.CHAR_LOADED);
            });
        }
        else
        {
            EventProcessor.Instance.QueueEvent(() =>
            {
                Debug.Log("Return to character selection.");
                GameManager.Instance.NotifyEvent(GameEvent.RESTART_ALLOWED);
            });
        }
    }

    private void OnCharCreateFail(byte[] data)
    {
        CharCreateFailPacket packet = new CharCreateFailPacket(data);
        CharCreateFailPacket.CreateFailReason reason = (CharCreateFailPacket.CreateFailReason)packet.Reason;
        Debug.LogWarning($"Character creation failed: {reason}.");
        int systemMessageId = 128;
        _eventProcessor.QueueEvent(() =>
        {
            switch (reason)
            {
                case CharCreateFailPacket.CreateFailReason.REASON_CREATION_FAILED:
                    systemMessageId = 128;
                    break;
                case CharCreateFailPacket.CreateFailReason.REASON_NAME_ALREADY_EXISTS:
                    systemMessageId = 79;
                    break;
                case CharCreateFailPacket.CreateFailReason.REASON_TOO_MANY_CHARACTERS:
                    systemMessageId = 77;
                    break;
                case CharCreateFailPacket.CreateFailReason.REASON_INCORRECT_NAME:
                    systemMessageId = 205;
                    break;
            }
            L2ConfirmWindow.Instance.ShowWindow(systemMessageId, () =>
            {
            }, null);
        });
    }

    private void OnCharCreateOk(byte[] data)
    {
        CharCreateOkPacket packet = new CharCreateOkPacket(data);
        Debug.Log($"Character creation succeeded.");
        // EventProcessor.Instance.QueueEvent(() => GameClient.Instance.OnCharCreateOk());
    }

    private void OnMessageReceive(byte[] data)
    {
        CreatureSayPacket packet = new CreatureSayPacket(data);
        if (packet.MessageType == L2MessageType.BOAT)
        {
            SystemMessageDat messageData = SystemMessageTable.Instance.GetSystemMessage(packet.SystemMessageId);
            SystemMessage systemMessage = new SystemMessage(null, messageData);
            _eventProcessor.QueueEvent(() => ChatWindow.Instance.ReceiveSystemMessage(systemMessage));
            //TODO: Handle packet SysStringId
            return;
        }

        String sender = packet.Sender;
        String text = packet.Text;

        //TODO: Handle message channel colors
        ChatMessage message;
        switch (packet.MessageType)
        {
            case L2MessageType.TRADE:
                message = new TradeMessage(sender, text);
                break;
            case L2MessageType.SHOUT:
                message = new ShoutMessage(sender, text);
                break;
            case L2MessageType.PARTY:
                message = new PartyMessage(sender, text);
                break;
            case L2MessageType.CLAN:
                message = new ClanMessage(sender, text);
                break;
            case L2MessageType.ALLIANCE:
                message = new AllianceMessage(sender, text);
                break;
            case L2MessageType.HERO_VOICE:
                message = new HeroMessage(sender, text);
                break;
            case L2MessageType.TELL:
                message = new TellMessage(sender, text);
                break;
            case L2MessageType.CRITICAL_ANNOUNCE:
                message = new CriticalAnnounceMesasge(sender, text);
                break;
            case L2MessageType.ANNOUNCEMENT:
                message = new AnnounceMesasge(sender, text);
                break;
            default:
                message = new NormalMessage(sender, text);
                break;
        }

        _eventProcessor.QueueEvent(() => ChatWindow.Instance.ReceiveChatMessage(message));
    }

    private void OnSystemMessageReceive(byte[] data)
    {
        SystemMessagePacket packet = new SystemMessagePacket(data);
        SMParam[] smParams = packet.Params;
        int messageId = packet.Id;

        SystemMessageDat messageData = SystemMessageTable.Instance.GetSystemMessage(messageId);
        if (messageData != null)
        {
            SystemMessage systemMessage = new SystemMessage(smParams, messageData);
            _eventProcessor.QueueEvent(() =>
            {
                if (messageData.Id == 113)
                { // unsuitable terms
                    WorldCombat.Instance.OnSkillNotAllowed(smParams[0].GetIntArrayValue()[0]);
                }

                if (messageData.Id == 109)
                {
                    PlayerStateMachine.Instance.OnActionDenied();
                }

                if (messageData.Sound != null)
                {
                    AudioManager.Instance.PlayUISound(messageData.Sound);
                }

                ChatWindow.Instance.ReceiveSystemMessage(systemMessage);
            });
        }
        else
        {
            _eventProcessor.QueueEvent(() => ChatWindow.Instance.ReceiveSystemMessage(new UnhandledMessage()));
        }
    }

    private void OnCharSelected(byte[] data)
    {
        CharSelectedPacket packet = new CharSelectedPacket(data);
        _eventProcessor.QueueEvent(() =>
        {
            GameClient.Instance.PlayerInfo = packet.PacketPlayerInfo;
            GameManager.Instance.NotifyEvent(GameEvent.CHAR_SELECTED);
        });
    }

    private void OnPlayerInfoReceive(byte[] data)
    {
        PlayerInfoPacket packet = new PlayerInfoPacket(data);
        WorldSpawner.Instance.OnReceivePlayerInfo(packet.Identity, packet.Status, packet.Stats, packet.Appearance, packet.EntityActionInfo);

        PlayerInventory.Instance.SetInventorySize(packet.InventorySpace);
    }

    private void OnUserInfoReceived(byte[] data)
    {
        UserInfoPacket packet = new UserInfoPacket(data);

        WorldSpawner.Instance.OnReceiveUserInfo(packet.Identity, packet.Status, packet.Stats, packet.Appearance, packet.EntityActionInfo);
    }

    private void OnUpdatePosition(byte[] data)
    {
        UpdatePositionPacket packet = new UpdatePositionPacket(data);
        int id = packet.Id;
        Vector3 position = packet.Position;
        World.Instance.UpdateObjectPosition(id, position);
    }

    private void OnValidateLocation(byte[] data)
    {
        ValidateLocationPacket packet = new ValidateLocationPacket(data);
        int id = packet.Id;
        Vector3 position = packet.Location;
        int heading = packet.Heading;
        World.Instance.AdjustObjectPositionAndRotation(id, position, heading);
    }

    private void OnRemoveObject(byte[] data)
    {
        RemoveObjectPacket packet = new RemoveObjectPacket(data);
        WorldSpawner.Instance.RemoveObject(packet.Id);
    }

    private void OnEntityAttack(byte[] data)
    {
        InflictDamagePacket packet = new InflictDamagePacket(data);
        Hit[] hits = packet.Hits;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] != null)
            {
                WorldCombat.Instance.EntityAttacks(packet.AttackerPosition, packet.SenderId, hits[i], i);
            }
        }
    }

    private void OnNpcInfoReceive(byte[] data)
    {
        NpcInfoPacket packet = new NpcInfoPacket(data);
        _eventProcessor.QueueEvent(() => WorldSpawner.Instance.OnReceiveNpcInfo(packet.Identity, packet.Status, packet.Stats, packet.Appearance, packet.EntityActionInfo));
    }

    private void OnObjectMoveTo(byte[] data)
    {
        ObjectMoveToPacket packet = new ObjectMoveToPacket(data);
        World.Instance.UpdateObjectDestination(packet.Id, packet.CurrentPosition, packet.Destination);
    }

    private void OnUpdateMoveDirection(byte[] data)
    {
        UpdateMoveDirectionPacket packet = new UpdateMoveDirectionPacket(data);
        World.Instance.UpdateObjectMoveDirection(packet.Id, packet.Position, packet.Direction, packet.VerticalVelocity, packet.Timestamp);
    }

    private void OnEntityTargetSet(byte[] data)
    {
        EntityTargetSetPacket packet = new EntityTargetSetPacket(data);
        WorldCombat.Instance.UpdateEntityTarget(packet.EntityId, packet.TargetId, packet.EntityPosition);
    }

    private void OnEntityTargetUnset(byte[] data)
    {
        EntityTargetUnsetPacket packet = new EntityTargetUnsetPacket(data);
        WorldCombat.Instance.UnsetEntityTarget(packet.EntityId);
    }

    private void OnMyTargetSet(byte[] data)
    {
        MyTargetSetPacket packet = new MyTargetSetPacket(data);
        WorldCombat.Instance.UpdateMyTarget(GameClient.Instance.CurrentPlayerId, packet.TargetId);
    }

    private void OnActionFailed(byte[] data)
    {
        ActionFailedPacket packet = new ActionFailedPacket(data);
        Debug.Log($"Action failed");
        _eventProcessor.QueueEvent(() => PlayerEntity.Instance.OnActionFailed());
    }

    private void OnActionAllowed(byte[] data)
    {
        ActionAllowedPacket packet = new ActionAllowedPacket(data);
        _eventProcessor.QueueEvent(() => PlayerEntity.Instance.OnActionAllowed());
    }

    private void OnServerClose()
    {
        Debug.Log("ServerClose received from Gameserver");
        _client.Disconnect();
    }

    private void OnStatusUpdate(byte[] data)
    {
        StatusUpdatePacket packet = new StatusUpdatePacket(data);
        WorldCombat.Instance.StatusUpdate(packet.ObjectId, packet.Attributes);
    }

    private void OnInventoryItemList(byte[] data)
    {
        InventoryItemListPacket packet = new InventoryItemListPacket(data);
        _eventProcessor.QueueEvent(() => PlayerInventory.Instance.SetInventory(packet.Items, packet.OpenWindow));
    }

    private void OnInventoryUpdate(byte[] data)
    {
        InventoryUpdatePacket packet = new InventoryUpdatePacket(data);
        Debug.Log("Updated items: " + packet.Items.Length);
        _eventProcessor.QueueEvent(() => PlayerInventory.Instance.UpdateInventory(packet.Items));
    }

    private void OnLeaveWorld(byte[] data)
    {
#if UNITY_EDITOR
        _client.Disconnect();
#else
        _eventProcessor.QueueEvent(() =>
        {
            Application.Quit();
        });
#endif
    }

    private void OnRestartResponse(byte[] data)
    {
        // RestartResponsePacket packet = new RestartResponsePacket(data);
        // if (packet.Allowed)
        // {
        //     // Do nothing, handle upcoming charselect packet instead
        //     GameManager.Instance.GameState = GameState.RESTARTING;
        // }
    }

    private void OnShortcutInit(byte[] data)
    {
        ShortcutInitPacket packet = new ShortcutInitPacket(data);
        _eventProcessor.QueueEvent(() => PlayerShortcuts.Instance.SetShortcutList(packet.Shortcuts));
    }

    private void OnShortcutRegister(byte[] data)
    {
        ShortcutRegisterPacket packet = new ShortcutRegisterPacket(data);
        _eventProcessor.QueueEvent(() => PlayerShortcuts.Instance.RegisterShortcut(packet.NewShortcut));
    }

    private void OnShortcutDelete(byte[] data)
    {
        ShortcutDeletePacket packet = new ShortcutDeletePacket(data);
        _eventProcessor.QueueEvent(() => PlayerShortcuts.Instance.RemoveShotcutLocally(packet.Slot));
    }

    private void OnChangeWaitType(byte[] data)
    {
        ChangeWaitTypePacket packet = new ChangeWaitTypePacket(data);
        // Debug.Log("ChangeWaitType: " + packet.Owner + " " + packet.MoveType);
        World.Instance.ChangeWaitType(packet.Owner, packet.MoveType, packet.EntityPosition);
    }

    private void OnChangeMoveType(byte[] data)
    {
        ChangeMoveTypePacket packet = new ChangeMoveTypePacket(data);
        // Debug.Log("ChangeMoveType: " + packet.Owner + " running? " + packet.Running);
        World.Instance.ChangeMoveType(packet.Owner, packet.Running);
    }

    private void OnEntityDie(byte[] data)
    {
        DoDiePacket packet = new DoDiePacket(data);
        WorldCombat.Instance.EntityDied(packet.EntityId, packet.ToVillageAllowed, packet.ToClanHallAllowed, packet.ToCastleAllowed, packet.ToSiegeHQAllowed, packet.Sweepable, packet.FixedResAllowed);
    }

    private void OnEntityRevive(byte[] data)
    {
        RevivePacket packet = new RevivePacket(data);
        WorldCombat.Instance.EntityRevived(packet.EntityId);
    }

    private void OnTeleportToLocation(byte[] data)
    {
        TeleportToLocationPacket packet = new TeleportToLocationPacket(data);
        Debug.LogWarning("Teleport screen!");
        World.Instance.EntityTeleported(packet.EntityId, packet.TeleportTo, packet.LoadingScreen);
    }

    private void OnEntityStopMove(byte[] data)
    {
        ObjectStopMovePacket packet = new ObjectStopMovePacket(data);
        World.Instance.ObjectStoppedMove(packet.Id, packet.CurrentPosition, packet.Heading);
    }

    private void OnNpcHtmlReceive(byte[] data)
    {
        NpcHtmlPacket packet = new NpcHtmlPacket(data);
        World.Instance.NpcHtmlReceived(packet.ObjectId, packet.Html, packet.ItemId);
    }

    private void OnExAutoSoulshot(byte[] data)
    {
        // TODO: manage different 0xfe packets if we use them
        ExAutoSoulshotPacket packet = new ExAutoSoulshotPacket(data);
        WorldCombat.Instance.ExAutoSoulshotReceived(packet.ItemId, packet.Enable);
    }

    private void OnSocialActionReceived(byte[] data)
    {
        SocialActionPacket packet = new SocialActionPacket(data);
        World.Instance.SocialActionReceived(packet.ObjectId, packet.Action);
    }

    private void OnSetupGauge(byte[] data)
    {
        SetupGaugePacket packet = new SetupGaugePacket(data);
        WorldCombat.Instance.SetupGauge(packet.Color, packet.Time, packet.MaxTime);
    }

    private void OnRelationChanged(byte[] data)
    {
        RelationChangedPacket packet = new RelationChangedPacket(data);
        WorldCombat.Instance.RelationChanged(packet.Owner, packet.Karma, packet.PvpFlag);
    }

    private void OnCharDeleteOk(byte[] data)
    {
    }

    private void OnCharDeleteFail(byte[] data)
    {
        CharDeleteFailPacket packet = new CharDeleteFailPacket(data);
        Debug.LogWarning("Char Delete Failed: " + packet.Reason);
        int systemMessageId = 128;
        _eventProcessor.QueueEvent(() =>
        {
            switch ((CharDeleteFailPacket.CharDeleteFailReason)packet.Reason)
            {
                case CharDeleteFailPacket.CharDeleteFailReason.REASON_DELETION_FAILED:
                    systemMessageId = 306;
                    break;
                case CharDeleteFailPacket.CharDeleteFailReason.REASON_YOU_MAY_NOT_DELETE_CLAN_MEMBER:
                    systemMessageId = 541;
                    break;
                case CharDeleteFailPacket.CharDeleteFailReason.REASON_CLAN_LEADERS_MAY_NOT_BE_DELETED:
                    systemMessageId = 540;
                    break;
            }
            L2ConfirmWindow.Instance.ShowWindow(systemMessageId, () =>
            {
            }, null);
        });
    }

    private void OnSkillList(byte[] data)
    {
        SkillListPacket packet = new SkillListPacket(data);
        _eventProcessor.QueueEvent(() => PlayerSkill.Instance.SetSkills(packet.Skills));
    }

    private void OnSkillCoolTime(byte[] data)
    {
        SkillCoolTimePacket packet = new SkillCoolTimePacket(data);
        _eventProcessor.QueueEvent(() => PlayerSkill.Instance.UpdateSkillCoolTimes(packet.Cooltimes));
    }

    private void OnAcquireSkillLearnList(byte[] data)
    {
        AcquireSkillLearnListPacket packet = new AcquireSkillLearnListPacket(data);
        _eventProcessor.QueueEvent(() => SkillLearnWindow.Instance.InitSkillsList(packet.Skills));
    }

    private void OnAcquireSkillLearnInfo(byte[] data)
    {
        AcquireSkillLearnInfoPacket packet = new AcquireSkillLearnInfoPacket(data);
        _eventProcessor.QueueEvent(() => SkillLearnWindow.Instance.ShowSkillDetail(packet.Requirements));
    }

    private void OnAcquireSkillDone(byte[] data)
    {
        // AcquireSkillDonePacket _ = new AcquireSkillDonePacket(data);
        // do nothing
    }

    private void OnBuyListReceived(byte[] data)
    {
        BuyListPacket packet = new BuyListPacket(data);
        _eventProcessor.QueueEvent(() =>
        {
            NpcHtmlWindow.Instance.HideWindow(false);
            ShopWindow.Instance.ShowWindow();
            ShopWindow.Instance.RefreshProductList(packet.ListId, packet.Adena, packet.Products, ShopTab.ShopTabType.BUY, packet.OpenTab);
        });
    }

    private void OnSellListReceived(byte[] data)
    {
        SellListPacket packet = new SellListPacket(data);
        _eventProcessor.QueueEvent(() =>
        {
            NpcHtmlWindow.Instance.HideWindow(false);
            ShopWindow.Instance.ShowWindow();
            ShopWindow.Instance.RefreshProductList(-1, packet.Adena, packet.Products, ShopTab.ShopTabType.SELL, packet.OpenTab);
        });
    }

    private void OnMagicSkillUse(byte[] data)
    {
        MagicSkillUsePacket packet = new MagicSkillUsePacket(data);
        WorldCombat.Instance.OnMagicSkillUse(packet);
    }

    private void OnMagicSkillCanceled(byte[] data)
    {
        MagicSkillCanceledPacket packet = new MagicSkillCanceledPacket(data);
        WorldCombat.Instance.OnMagicSkillCanceled(packet);
    }

    private void OnSkillLaunched(byte[] data)
    {
        MagicSkillLaunchedPacked packet = new MagicSkillLaunchedPacked(data);
        WorldCombat.Instance.OnMagicSkillLaunched(packet);
    }

    private void OnFightStanceStart(byte[] data)
    {
        FightStanceStartPacket packet = new FightStanceStartPacket(data);
        WorldCombat.Instance.OnFightStanceStart(packet.EntityId);
    }

    private void OnFightStanceStop(byte[] data)
    {
        FightStanceStopPacket packet = new FightStanceStopPacket(data);
        WorldCombat.Instance.OnFightStanceStop(packet.EntityId);
    }

    private void OnShortBuffStatusUpdate(byte[] data)
    {
        ShortBuffStatusUpdatePacket packet = new ShortBuffStatusUpdatePacket(data);
        BuffWindow.Instance.AddEffect(packet.SkillId, packet.SkillLvl, packet.Duration, BuffType.Special);
    }

    private void OnAbnormalStatusUpdate(byte[] data)
    {
        AbnormalStatusUpdatePacket packet = new AbnormalStatusUpdatePacket(data);
        _eventProcessor.QueueEvent(() => BuffWindow.Instance.SetEffects(packet.Effects));
        _eventProcessor.QueueEvent(() => DebuffWindow.Instance.SetEffects(packet.Effects));
    }

    private void OnPartyEffect(byte[] data)
    {
        PartyEffectPacket packet = new PartyEffectPacket(data);
        Debug.LogWarning("Part effects not yet handled!");
        // _eventProcessor.QueueEvent(() => BuffWindow.Instance.UpsertBuffs(packet.ObjectId, packet.Type, packet.Effects));
    }

    private void OnEtcStatusUpdate(byte[] data)
    {
        EtcStatusUpdatePacket packet = new EtcStatusUpdatePacket(data);
        _eventProcessor.QueueEvent(() => BuffWindow.Instance.SetEtcEffects(
            new PlayerBuffStatus(packet.Charges, packet.WeightPenalty, packet.IsBlockingAllPlayers,
                packet.IsInsideDangerZone, packet.HasPenalty, packet.HasCharmOfCourage, packet.DeathPenaltyLvl)));
        _eventProcessor.QueueEvent(() => DebuffWindow.Instance.SetEtcEffects(
            new PlayerBuffStatus(packet.Charges, packet.WeightPenalty, packet.IsBlockingAllPlayers,
                packet.IsInsideDangerZone, packet.HasPenalty, packet.HasCharmOfCourage, packet.DeathPenaltyLvl)));
    }
}
