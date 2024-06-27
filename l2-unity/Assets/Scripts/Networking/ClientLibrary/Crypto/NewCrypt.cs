namespace L2_login
{
    class NewCrypt
    {
        public static bool verifyChecksum(byte[] raw)
        {
            return verifyChecksum(raw, 0, raw.Length);
        }

        public static bool verifyChecksum(byte[] raw, int offset, int size)
        {
            // check if size is multiple of 4 and if there is more then only the checksum
            if ((size & 3) != 0 || size <= 4)
            {
                return false;
            }

            ulong chksum = 0;
            int count = size - 4;
            ulong check = ulong.MaxValue;
            int i;

            for (i = offset; i < count; i += 4)
            {
                check = (ulong)raw[i] & 0xff;
                check |= (ulong)raw[i + 1] << 8 & 0xff00;
                check |= (ulong)raw[i + 2] << 0x10 & 0xff0000;
                check |= (ulong)raw[i + 3] << 0x18 & 0xff000000;

                chksum ^= check;
            }

            check = (ulong)raw[i] & 0xff;
            check |= (ulong)raw[i + 1] << 8 & 0xff00;
            check |= (ulong)raw[i + 2] << 0x10 & 0xff0000;
            check |= (ulong)raw[i + 3] << 0x18 & 0xff000000;

            return check == chksum;
        }

        public static void appendChecksum(byte[] raw)
        {
            appendChecksum(raw, 0, raw.Length);
        }

        public static void appendChecksum(byte[] raw, int offset, int size)
        {
            ulong chksum = 0;
            int count = size - 4;
            ulong ecx;
            int i;

            for (i = offset; i < count; i += 4)
            {
                ecx = (ulong)raw[i] & 0xff;
                ecx |= (ulong)raw[i + 1] << 8 & 0xff00;
                ecx |= (ulong)raw[i + 2] << 0x10 & 0xff0000;
                ecx |= (ulong)raw[i + 3] << 0x18 & 0xff000000;

                chksum ^= ecx;
            }

            ecx = (ulong)raw[i] & 0xff;
            ecx |= (ulong)raw[i + 1] << 8 & 0xff00;
            ecx |= (ulong)raw[i + 2] << 0x10 & 0xff0000;
            ecx |= (ulong)raw[i + 3] << 0x18 & 0xff000000;

            raw[i] = (byte)(chksum & 0xff);
            raw[i + 1] = (byte)(chksum >> 0x08 & 0xff);
            raw[i + 2] = (byte)(chksum >> 0x10 & 0xff);
            raw[i + 3] = (byte)(chksum >> 0x18 & 0xff);
        }

        /**
	     * Packet is first XOR encoded with <code>key</code>
	     * Then, the last 4 bytes are overwritten with the the XOR "key".
	     * Thus this assume that there is enough room for the key to fit without overwriting data.
	     * @param raw The raw bytes to be encrypted
	     * @param key The 4 bytes (int) XOR key
	     */
        public static void encXORPass(byte[] raw, int key)
        {
            encXORPass(raw, 0, raw.Length, key);
        }

        /**
	     * Packet is first XOR encoded with <code>key</code>
	     * Then, the last 4 bytes are overwritten with the the XOR "key".
	     * Thus this assume that there is enough room for the key to fit without overwriting data.
	     * @param raw The raw bytes to be encrypted
	     * @param offset The begining of the data to be encrypted
	     * @param size Length of the data to be encrypted
	     * @param key The 4 bytes (int) XOR key
	     */
        public static void encXORPass(byte[] raw, int offset, int size, int key)
        {
            int stop = size - 8;
            int pos = 4 + offset;
            int edx;
            int ecx = key; // Initial xor key

            while (pos < stop)
            {
                edx = raw[pos] & 0xFF;
                edx |= (raw[pos + 1] & 0xFF) << 8;
                edx |= (raw[pos + 2] & 0xFF) << 16;
                edx |= (raw[pos + 3] & 0xFF) << 24;

                ecx += edx;

                edx ^= ecx;

                raw[pos] = (byte)(edx & 0xFF);
                raw[pos + 1] = (byte)(edx >> 8 & 0xFF);
                raw[pos + 2] = (byte)(edx >> 16 & 0xFF);
                raw[pos + 3] = (byte)(edx >> 24 & 0xFF);
                pos += 4;
            }

            raw[pos++] = (byte)(ecx & 0xFF);
            raw[pos++] = (byte)(ecx >> 8 & 0xFF);
            raw[pos++] = (byte)(ecx >> 16 & 0xFF);
            raw[pos++] = (byte)(ecx >> 24 & 0xFF);
        }

        public static void decXORPass(byte[] raw, int offset, int size, int key)
        {
            int stop = 4 + offset;
            int pos = size - 12;
            int edx;
            int ecx = key; // Initial xor key

            while (stop <= pos)
            {
                edx = raw[pos] & 0xFF;
                edx |= (raw[pos + 1] & 0xFF) << 8;
                edx |= (raw[pos + 2] & 0xFF) << 16;
                edx |= (raw[pos + 3] & 0xFF) << 24;

                edx ^= ecx;

                ecx -= edx;

                raw[pos] = (byte)(edx & 0xFF);
                raw[pos + 1] = (byte)(edx >> 8 & 0xFF);
                raw[pos + 2] = (byte)(edx >> 16 & 0xFF);
                raw[pos + 3] = (byte)(edx >> 24 & 0xFF);
                pos -= 4;
            }

            //raw[pos++] = (byte)(ecx & 0xFF);
            //raw[pos++] = (byte)(ecx >> 8 & 0xFF);
            //raw[pos++] = (byte)(ecx >> 16 & 0xFF);
            //raw[pos++] = (byte)(ecx >> 24 & 0xFF);
        }
    }
}
