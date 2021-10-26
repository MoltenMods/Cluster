﻿using Blueprint.Enums.Networking;
using Blueprint.Messages;
using Blueprint.Messages.Objects;

namespace Cluster.Networking
{
    public partial class Client
    {
        public delegate void MessageHandler(MessageType messageType);

        public event MessageHandler OnMessage;
        
        public delegate void HostedGameHandler(GameCode gameCode);

        public event HostedGameHandler OnHostedGame;

        public delegate void PlayerJoinedHandler(GameCode gameCode, uint joiningPlayerId, uint hostId);

        public event PlayerJoinedHandler OnPlayerJoined;

        public delegate void StartedGameHandler(GameCode gameCode);

        public event StartedGameHandler OnStartedGame;

        public delegate void RemovedGameHandler(DisconnectReason? disconnectReason);

        public event RemovedGameHandler OnRemovedGame;

        public delegate void RemovedPlayerHandler(
            GameCode gameCode,
            uint playerId,
            uint hostId,
            DisconnectReason disconnectReason);

        public event RemovedPlayerHandler OnRemovedPlayer;
        
        public delegate void JoinedGameHandler(GameCode gameCode, uint playerId, uint hostId, uint[] otherPlayerIds);

        public event JoinedGameHandler OnJoinedGame;

        public delegate void EndedGameHandler(GameCode gameCode, GameOverReason gameOverReason, bool showAd);

        public event EndedGameHandler OnEndedGame;
    }
}