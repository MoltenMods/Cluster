using System.Threading.Tasks;
using Blueprint.Enums.Networking;
using Blueprint.Messages.C2S;
using Blueprint.Messages.Objects;
using Singularity.Hazel;
using Singularity.Hazel.Api.Net.Messages;

namespace Cluster.Networking
{
    public partial class Client
    {
        public async ValueTask HostGame(GameOptionsData options, QuickChatMode chatMode = QuickChatMode.FreeChatOrQuickChat)
        {
            using var writer = MessageWriter.Get(MessageType.Reliable);
            HostGameC2S.Serialize(writer, options, chatMode);
            await this.Connection.SendAsync(writer);
        }

        public async ValueTask JoinGame(GameCode gameCode)
        {
            using var writer = MessageWriter.Get(MessageType.Reliable);
            JoinGameC2S.Serialize(writer, gameCode);
            await this.Connection.SendAsync(writer);
        }
    }
}