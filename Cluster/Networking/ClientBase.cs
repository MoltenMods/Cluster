using Blueprint.Enums.Networking;
using Blueprint.Messages.C2S;

namespace Cluster.Networking
{
    public class ClientBase
    {
        public int Id { get; internal set; }
        
        public int HostId { get; internal set; }
        
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

        internal byte[] GetConnectionData(uint authNonce = 0)
        {
            return HandshakeC2S.Serialize(
                this.BroadcastVersion,
                this.PlayerName,
                authNonce,
                this.Language,
                this.ChatMode);
        }
    }
}