using System;
using System.Threading.Tasks;
using Blueprint.Enums.Networking;
using Blueprint.Messages.C2S;
using Blueprint.Messages.InnerNetObjects;
using Blueprint.Messages.Objects;
using Singularity.Hazel;
using Singularity.Hazel.Api.Net.Messages;
using GameData = Blueprint.Messages.GameData.GameData;

namespace Cluster.Networking
{
    public partial class Client
    {
        public ValueTask Authenticate(
            int broadcastVersion = 50537300,
            Platform platform = Platform.Unknown,
            string productUserId = "")
        {
            throw new NotImplementedException();
        }
        
        // TODO: create GameData streams

        public async ValueTask Spawn(
            InnerNetObject innerNetObject,
            GameCode gameCode,
            SpawnFlags spawnFlags = SpawnFlags.None)
        {
            using var writer = MessageWriter.Get(MessageType.Reliable);
            
            GameData.StartGameDataMessage(writer, gameCode);
            innerNetObject.WriteSpawnMessage(writer, spawnFlags);
            writer.EndMessage();

            await this.Connection.SendAsync(writer);
        }

        public async ValueTask Update(InnerNetObject innerNetObject, GameCode gameCode)
        {
            using var writer = MessageWriter.Get(MessageType.Reliable);
            
            GameData.StartGameDataMessage(writer, gameCode);
            innerNetObject.WriteDataMessage(writer);
            writer.EndMessage();

            await this.Connection.SendAsync(writer);
        }
        
        public async ValueTask HostGame(
            GameOptionsData options,
            QuickChatMode chatMode = QuickChatMode.FreeChatOrQuickChat)
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