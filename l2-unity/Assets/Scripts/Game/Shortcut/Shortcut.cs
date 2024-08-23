using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shortcut
{
    public static int TYPE_ITEM = 1;
    public static int TYPE_SKILL = 2;
    public static int TYPE_ACTION = 3;
    public static int TYPE_MACRO = 4;
    public static int TYPE_RECIPE = 5;

    private int _slot;
    private int _page;
    private int _type;
    private int _id;
    private int _level;

    public int Slot { get { return _slot; } }
    public int Page { get { return _page; } }
    public int Type { get { return _type; } }
    public int Id { get { return _id; } }
    public int Level { get { return _level; } }

    public Shortcut(int slotId, int pageId, int shortcutType, int shortcutId, int shortcutLevel)
    {
        _slot = slotId;
        _page = pageId;
        _type = shortcutType;
        _id = shortcutId;
        _level = shortcutLevel;
    }
}
