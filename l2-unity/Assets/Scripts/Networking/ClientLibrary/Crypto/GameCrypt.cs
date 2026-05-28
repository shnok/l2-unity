namespace L2_login
{

    public class GameCrypt
    {
        private readonly byte[] _inkey = new byte[16];
        private readonly byte[] _outkey = new byte[16];
        private bool _isEnabled;

        public void SetKey(byte[] key)
        {
            _isEnabled = true;
            key.CopyTo(_inkey, 0);
            key.CopyTo(_outkey, 0);

            _inkey[8] = (byte)0xc8;
            _inkey[9] = (byte)0x27;
            _inkey[10] = (byte)0x93;
            _inkey[11] = (byte)0x01;
            _inkey[12] = (byte)0xa1;
            _inkey[13] = (byte)0x6c;
            _inkey[14] = (byte)0x31;
            _inkey[15] = (byte)0x97;

            _outkey[8] = (byte)0xc8;
            _outkey[9] = (byte)0x27;
            _outkey[10] = (byte)0x93;
            _outkey[11] = (byte)0x01;
            _outkey[12] = (byte)0xa1;
            _outkey[13] = (byte)0x6c;
            _outkey[14] = (byte)0x31;
            _outkey[15] = (byte)0x97;
        }

        public void Decrypt(byte[] raw)
        {
            if (!_isEnabled)
                return;

            uint num1 = 0;
            for (int index = 0; index < raw.Length; ++index)
            {
                uint num2 = raw[index] & (uint)byte.MaxValue;
                raw[index] = (byte)(num2 ^ _inkey[index & 15] ^ num1);
                num1 = num2;
            }

            uint num3 = ((_inkey[8] & (uint)byte.MaxValue) | (uint)((_inkey[9] << 8) & 65280) | (uint)((_inkey[10] << 16) & 16711680) | (uint)((_inkey[11] << 24) & -16777216)) + (uint)raw.Length;
            _inkey[8] = (byte)(num3 & byte.MaxValue);
            _inkey[9] = (byte)((num3 >> 8) & byte.MaxValue);
            _inkey[10] = (byte)((num3 >> 16) & byte.MaxValue);
            _inkey[11] = (byte)((num3 >> 24) & byte.MaxValue);
        }

        public void Encrypt(byte[] raw)
        {
            if (!_isEnabled)
                _isEnabled = true;
            else
            {
                uint num1 = 0;
                for (int index = 0; index < raw.Length; ++index)
                {
                    num1 = (raw[index] & (uint)byte.MaxValue) ^ _outkey[index & 15] ^ num1;
                    raw[index] = (byte)num1;
                }

                uint num2 = ((_outkey[8] & (uint)byte.MaxValue) | (uint)((_outkey[9] << 8) & 65280) | (uint)((_outkey[10] << 16) & 16711680) | (uint)((_outkey[11] << 24) & -16777216)) + (uint)raw.Length;
                _outkey[8] = (byte)(num2 & byte.MaxValue);
                _outkey[9] = (byte)((num2 >> 8) & byte.MaxValue);
                _outkey[10] = (byte)((num2 >> 16) & byte.MaxValue);
                _outkey[11] = (byte)((num2 >> 24) & byte.MaxValue);
            }
        }
    }
}
