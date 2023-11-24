using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace rTexImporter
{
    [Serializable]
    public class AtlasData
    {
        public Software software;
        public Atlas atlas;
        public rSprite[] sprites;
    }

    [Serializable]
    public struct Software
    {
        public string name;
        public string url;
    }

    [Serializable]
    public struct Atlas
    {
        public string imagePath;
        public int width;
        public int height;
        public int spriteCount;
        public bool isFont;
        public int fontSize;
    }

    [Serializable]
    public struct rSprite
    {
        public string nameId;
        public rVector2 origin;
        public rVector2 position;
        public rSize sourceSize;
        public int padding;
        public bool trimmed;
        public rRect trimRec;
    }

    [Serializable]
    public struct rVector2
    {
        public int x;
        public int y;
    }

    [Serializable]
    public struct rSize
    {
        public int width;
        public int height;
    }

    [Serializable]
    public struct rRect
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }
}



