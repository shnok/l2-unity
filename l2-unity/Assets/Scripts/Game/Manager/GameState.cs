using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState : byte { 
    LOGIN_SCREEN = 0, 
    LOGIN_CONNECTED = 1, 
    READING_LICENSE = 2, 
    SERVER_LIST = 3,
    READY_TO_CONNECT = 4,
    CONNECTING_TO_GAMESERVER = 5,
    GAMESERVER_CONNECTED = 6,
    GAMESERVER_AUTHED = 7,
    CHAR_SELECT = 8, 
    CHAR_CREATION = 9, 
    IN_GAME = 10 
}