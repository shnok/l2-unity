using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerNameDAO
{
    private static Dictionary<int, string> SERVERS = new Dictionary<int, string>();

    public static void InitServerList() {
        SERVERS[1] = "Bartz";
        SERVERS[2] = "Sieghardt";
        SERVERS[3] = "Kain";
        SERVERS[4] = "Lionna";
        SERVERS[5] = "Erica";
        SERVERS[6] = "Gustin";
        SERVERS[7] = "Devianne";
        SERVERS[8] = "Hindemith";
        SERVERS[9] = "Teon (EURO)";
        SERVERS[10] = "Franz (EURO)";
        SERVERS[11] = "Luna (EURO)";
        SERVERS[12] = "Sayha";
        SERVERS[13] = "Aria";
        SERVERS[14] = "Phoenix";
        SERVERS[15] = "Chronos";
        SERVERS[16] = "Naia (EURO)";
        SERVERS[17] = "Elhwynna";
        SERVERS[18] = "Ellikia";
        SERVERS[19] = "Shikken";
        SERVERS[20] = "Scryde";
        SERVERS[21] = "Frikios";
        SERVERS[22] = "Ophylia";
        SERVERS[23] = "Shakdun";
        SERVERS[24] = "Tarziph";
        SERVERS[25] = "Aria";
        SERVERS[26] = "Esenn";
        SERVERS[27] = "Elcardia";
        SERVERS[28] = "Yiana";
        SERVERS[29] = "Seresin";
        SERVERS[30] = "Tarkai";
        SERVERS[31] = "Khadia";
        SERVERS[32] = "Roien";
        SERVERS[33] = "Kallint (Non-PvP)";
        SERVERS[34] = "Baium";
        SERVERS[35] = "Kamael";
        SERVERS[36] = "Beleth";
        SERVERS[37] = "Anakim";
        SERVERS[38] = "Lilith";
        SERVERS[39] = "Thifiel";
        SERVERS[40] = "Lithra";
        SERVERS[41] = "Lockirin";
        SERVERS[42] = "Kakai";
        SERVERS[43] = "Cadmus";
        SERVERS[44] = "Athebaldt";
        SERVERS[45] = "Blackbird";
        SERVERS[46] = "Ramsheart";
        SERVERS[47] = "Esthus";
        SERVERS[48] = "Vasper";
        SERVERS[49] = "Lancer";
        SERVERS[50] = "Ashton";
        SERVERS[51] = "Waytrel";
        SERVERS[52] = "Waltner";
        SERVERS[53] = "Tahnford";
        SERVERS[54] = "Hunter";
        SERVERS[55] = "Dewell";
        SERVERS[56] = "Rodemaye";
        SERVERS[57] = "Ken Rauhel";
        SERVERS[58] = "Ken Abigail";
        SERVERS[59] = "Ken Orwen";
        SERVERS[60] = "Van Holter";
        SERVERS[61] = "Desperion";
        SERVERS[62] = "Einhovant";
        SERVERS[63] = "Shunaiman";
        SERVERS[64] = "Faris";
        SERVERS[65] = "Tor";
        SERVERS[66] = "Carneiar";
        SERVERS[67] = "Dwyllios";
        SERVERS[68] = "Baium";
        SERVERS[69] = "Hallate";
        SERVERS[70] = "Zaken";
        SERVERS[71] = "Core";
    }
      

    public static string GetServer(int id) {
        if (SERVERS == null || SERVERS.Count == 0) {
            InitServerList();
        }

        if(SERVERS.TryGetValue(id, out string value)) {
            return value;
        }

        return "Undefined";
    }
}
