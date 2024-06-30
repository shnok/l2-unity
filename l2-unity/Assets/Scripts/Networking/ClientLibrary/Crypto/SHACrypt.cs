using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SHACrypt
{
    public static string ComputeSha256Hash(string rawData) {
        // Create a SHA256   
        using (SHA256 sha256Hash = SHA256.Create()) {
            // ComputeHash - returns byte array  
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string   
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++) {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static byte[] ComputeSha256HashToBytes(string rawData) {
        // Create a SHA256
        using (SHA256 sha256Hash = SHA256.Create()) {
            // ComputeHash - returns byte array
            return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        }
    }
}
