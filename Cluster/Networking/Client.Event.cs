using Blueprint.Messages.Objects;

namespace Cluster.Networking
{
    public partial class Client
    {
        public delegate void HostedGameHandler(GameCode gameCode);

        public event HostedGameHandler OnHostedGame;

        public delegate void JoinedGameHandler(GameCode gameCode, uint playerId, uint hostId, uint[] otherPlayerIds);

        public event JoinedGameHandler OnJoinedGame;
    }
}