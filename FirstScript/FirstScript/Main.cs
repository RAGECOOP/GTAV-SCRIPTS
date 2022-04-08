using System;
using System.Collections.Generic;
using System.ComponentModel;

using CoopClient;

using GTA;

namespace FirstScript
{
    [Serializable]
    public class TestPacket
    {
        public string Content { get; set; }
    }

    public class Main : Script
    {
        public static readonly Dictionary<long, int> PlayerScore = new Dictionary<long, int>();
        public Main()
        {
            COOPAPI.OnModPacketReceived += ModPacketReceived;
            COOPAPI.OnChatMessage += ChatMessage;
        }

        private static void ModPacketReceived(long from, string mod, byte customID, byte[] bytes)
        {
            if (!mod.Equals("FirstScript"))
            {
                return;
            }

            bool playerFound = true;
            var fromPlayer = COOPAPI.GetPlayer(from);
            if (fromPlayer == null)
            {
                GTA.UI.Notification.Show($"Player by ID [{from}] not found!");
                playerFound = false;
            }

            switch (customID)
            {
                case 0:
                    GTA.UI.Notification.Show("customID ~b~0~s~ ~g~received~s~" + (playerFound ? $" from {fromPlayer.Username}!" : "!"));
                    break;
                default:
                    GTA.UI.Notification.Show($"customID ~b~{customID}~s~ ~r~not found~s~" + (playerFound ? $" from {fromPlayer.Username}!" : "!"));
                    return;
            }
        }

        private static void ChatMessage(string from, string message, CancelEventArgs args)
        {
            if (!COOPAPI.GetUsername().Equals(from))
            {
                return;
            }

            if (!message.Equals("test"))
            {
                return;
            }

            args.Cancel = true;

            // Send the packet to the server
            try
            {
                byte[] testPacket = new TestPacket() { Content = "Test content" }.Serialize();
                if (testPacket.Length > 0)
                {
                    COOPAPI.SendDataToServer("FirstScript", 0, testPacket);
                    COOPAPI.LocalChatMessage("FirstScript", "Packet was sent to the server!");
                }
                else
                {
                    COOPAPI.LocalChatMessage("FirstScript", "Packet[Content] is empty!");
                }
            }
            catch (Exception ex)
            {
                GTA.UI.Notification.Show($"~r~{ex.Message}");
            }
        }
    }
}
