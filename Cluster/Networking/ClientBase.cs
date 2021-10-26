using System.IO;
using Blueprint.Enums.Networking;

namespace Cluster.Networking
{
    public class ClientBase
    {
        public int BroadcastVersion { get; }
        
        public string PlayerName { get; set; }
        
        public GameKeywords Language { get; set; }
        
        public QuickChatMode ChatMode { get; set; }

        public ClientBase(
            int broadcastVersion = 50537300,
            string playerName = "player",
            GameKeywords language = GameKeywords.English,
            QuickChatMode chatMode = QuickChatMode.FreeChatOrQuickChat)
        {
            this.BroadcastVersion = broadcastVersion;
            this.PlayerName = playerName;
            this.Language = language;
            this.ChatMode = chatMode;
        }

        internal byte[] GetConnectionData()
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
            
            binaryWriter.Write(this.BroadcastVersion);
            binaryWriter.Write(this.PlayerName);
            binaryWriter.Write((uint) 1);     // should be a nonce
            binaryWriter.Write((uint) this.Language);
            binaryWriter.Write((byte) this.ChatMode);

            return memoryStream.ToArray();
        }
    }
}