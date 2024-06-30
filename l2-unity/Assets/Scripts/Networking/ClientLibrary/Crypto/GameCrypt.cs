using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace L2_login {
    /// <summary>
    /// Summary description for Crypt.
    /// </summary>
    public class GameCrypt {
        private byte[] _key = new byte[16];//8];

        public GameCrypt() {
        }

        public void setKey(byte[] key) {
            _key[0] = key[0];
            _key[1] = key[1];
            _key[2] = key[2];
            _key[3] = key[3];
            _key[4] = key[4];
            _key[5] = key[5];
            _key[6] = key[6];
            _key[7] = key[7];
            _key[8] = key[8];
            _key[9] = key[9];
            _key[10] = key[10];
            _key[11] = key[11];
            _key[12] = key[12];
            _key[13] = key[13];
            _key[14] = key[14];
            _key[15] = key[15];
        }

        public void decrypt(byte[] raw) {
            uint temp = 0;
            for (int i = 0; i < raw.Length; i++) {
                uint temp2 = raw[i] & (uint)0xff;
                raw[i] = (byte)(temp2 ^ _key[i & 15] ^ temp);
                temp = temp2;
            }

            uint old = _key[8] & (uint)0xff;//0
            old |= ((uint)_key[9]) << 8 & 0xff00;//1
            old |= ((uint)_key[10]) << 0x10 & 0xff0000;//2
            old |= ((uint)_key[11]) << 0x18 & 0xff000000;//3

            old += (uint)raw.Length;

            _key[8] = (byte)(old & 0xff);//0
            _key[9] = (byte)(old >> 0x08 & 0xff);//1
            _key[10] = (byte)(old >> 0x10 & 0xff);//2
            _key[11] = (byte)(old >> 0x18 & 0xff);//3
        }

        public void decrypt(byte[] raw, uint size) {
            uint temp = 0;
            for (uint i = 0; i < size; i++) {
                uint temp2 = raw[i] & (uint)0xff;
                raw[i] = (byte)(temp2 ^ _key[i & 15] ^ temp);
                temp = temp2;
            }

            uint old = _key[8] & (uint)0xff;//0
            old |= ((uint)_key[9]) << 8 & 0xff00;//1
            old |= ((uint)_key[10]) << 0x10 & 0xff0000;//2
            old |= ((uint)_key[11]) << 0x18 & 0xff000000;//3

            old += size;

            _key[8] = (byte)(old & 0xff);//0
            _key[9] = (byte)(old >> 0x08 & 0xff);//1
            _key[10] = (byte)(old >> 0x10 & 0xff);//2
            _key[11] = (byte)(old >> 0x18 & 0xff);//3
        }

        public void encrypt(byte[] raw) {
            uint temp = 0;
            for (int i = 0; i < raw.Length; i++) {
                uint temp2 = raw[i] & (uint)0xff;
                temp = temp2 ^ _key[i & 15] ^ temp;
                raw[i] = (byte)temp;
            }

            uint old = _key[8] & (uint)0xff;
            old |= ((uint)_key[9]) << 8 & 0xff00;
            old |= ((uint)_key[10] << 0x10) & 0xff0000;
            old |= ((uint)_key[11] << 0x18) & 0xff000000;

            old += (uint)raw.Length;

            _key[8] = (byte)(old & 0xff);
            _key[9] = (byte)(old >> 0x08 & 0xff);
            _key[10] = (byte)(old >> 0x10 & 0xff);
            _key[11] = (byte)(old >> 0x18 & 0xff);
        }

        public void encrypt(byte[] raw, uint size) {
            uint temp = 0;
            for (uint i = 0; i < size; i++) {
                uint temp2 = raw[i] & (uint)0xff;
                temp = temp2 ^ _key[i & 15] ^ temp;
                raw[i] = (byte)temp;
            }

            uint old = _key[8] & (uint)0xff;
            old |= ((uint)_key[9]) << 8 & 0xff00;
            old |= ((uint)_key[10] << 0x10) & 0xff0000;
            old |= ((uint)_key[11] << 0x18) & 0xff000000;

            old += size;

            _key[8] = (byte)(old & 0xff);
            _key[9] = (byte)(old >> 0x08 & 0xff);
            _key[10] = (byte)(old >> 0x10 & 0xff);
            _key[11] = (byte)(old >> 0x18 & 0xff);
        }
    }//end of class
}
