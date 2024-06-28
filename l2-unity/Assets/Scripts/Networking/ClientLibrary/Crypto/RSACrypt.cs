using System;
using System.Security.Cryptography;
using System.Numerics;
using UnityEngine;

public class RSACrypt
{
    private RSAParameters _rsaParams;
    // hardcoded modulus
    private byte[] _modulus = new byte[] { 1, 0, 1 };


    public RSACrypt(byte[] exponent, bool needUnscramble) {
        if(needUnscramble) {
            UnscrambledRSAKey(exponent);
        }

        InitRSACrypt(exponent);
    }

    private void InitRSACrypt(byte[] exponent) {
        _rsaParams = new RSAParameters {
            Modulus = _modulus,
            D = exponent
        };
    }

    public void DecryptRSABlock(byte[] encryptedData) {
        // Initialize RSA with the parameters
        using (RSA rsa = RSA.Create()) {
            rsa.ImportParameters(_rsaParams);

            // Decrypt the data
            byte[] decryptedData = rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);

            // Convert decrypted data to string
            string decryptedMessage = System.Text.Encoding.UTF8.GetString(decryptedData);
            Console.WriteLine("Decrypted Message: " + decryptedMessage);
        }
    }

    public void UnscrambledRSAKey(byte[] rsaKey) {
        Debug.Log($"Scrambled RSA: {StringUtils.ByteArrayToString(rsaKey)}");

        // step 4 : xor last 0x40 bytes with  first 0x40 bytes
        for (int i = 0; i < 0x40; i++) {
            rsaKey[0x40 + i] = (byte)(rsaKey[0x40 + i] ^ rsaKey[i]);
        }
        // step 3 : xor bytes 0x0d-0x10 with bytes 0x34-0x38
        for (int i = 0; i < 4; i++) {
            rsaKey[0x0d + i] = (byte)(rsaKey[0x0d + i] ^ rsaKey[0x34 + i]);
        }
        // step 2 : xor first 0x40 bytes with  last 0x40 bytes 
        for (int i = 0; i < 0x40; i++) {
            rsaKey[i] = (byte)(rsaKey[i] ^ rsaKey[0x40 + i]);
        }
        // step 1 : 0x4d-0x50 <-> 0x00-0x04
        for (int i = 0; i < 4; i++) {
            byte temp = rsaKey[0x00 + i];
            rsaKey[0x00 + i] = rsaKey[0x4d + i];
            rsaKey[0x4d + i] = temp;
        }

        Debug.Log($"Unscrambled RSA: {StringUtils.ByteArrayToString(rsaKey)}");
    }
}
