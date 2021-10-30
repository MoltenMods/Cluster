using System.Net;
using System.Threading.Tasks;
using Blueprint.Messages.H2C;
using Blueprint.Messages.S2C;
using Microsoft.Extensions.ObjectPool;
using Singularity.Hazel;
using Singularity.Hazel.Api.Net.Messages;
using Singularity.Hazel.Udp;

namespace Cluster.Networking
{
    public partial class Client : ClientBase
    {
        public UdpClientConnection Connection { get; }

        private IPEndPoint _ipEndPoint;
        private ObjectPool<MessageReader> _readerPool;

        public Client(IPEndPoint ipEndPoint, ObjectPool<MessageReader> readerPool)
        {
            this._ipEndPoint = ipEndPoint;
            this._readerPool = readerPool;
            
            this.Connection = new UdpClientConnection(ipEndPoint, readerPool)
            {
                DataReceived = OnDataReceived,
                Disconnected = OnDisconnect
            };
        }

        public async Task ConnectAsync(uint authNonce = 0)
        {
            await this.Connection.ConnectAsync(this.GetConnectionData(authNonce));
        }

        public void Stop()
        {
            this.Connection.Stop();
        }

        private async ValueTask OnDataReceived(DataReceivedEventArgs e)
        {
            while (e.Message.Position < e.Message.Length)
            {
                await this.HandleMessageAsync(e.Message.ReadMessage(), e.Type);
            }
        }

        private ValueTask OnDisconnect(DisconnectedEventArgs e)
        {
            this.OnDisconnected?.Invoke(e.Message?.Copy(), e.Reason);
            
            return ValueTask.CompletedTask;
        }

        private ValueTask HandleMessageAsync(IMessageReader reader, MessageType type)
        {
            var flag = (Blueprint.Messages.MessageType) reader.Tag;
            
            this.OnMessage?.Invoke(reader.Copy(), flag);

            switch (flag)
            {
                case Blueprint.Messages.MessageType.HostGame:
                {
                    HostGameS2C.Deserialize(reader, out var gameCode);

                    this.OnHostedGame?.Invoke(gameCode);

                    break;
                }
                case Blueprint.Messages.MessageType.JoinGame:
                {
                    if (JoinGameS2C.TryDeserialize(reader, out var gameCode, out var joiningPlayerId, out var hostId))
                    {
                        this.OnPlayerJoined?.Invoke(gameCode, joiningPlayerId!.Value, hostId!.Value);
                    }
                    else
                    {
                        JoinGameS2C.TryDeserialize(reader, out var disconnectReason);
                        
                        this.OnPlayerJoinedError?.Invoke(disconnectReason!.Value);
                    }

                    break;
                }
                case Blueprint.Messages.MessageType.StartGame:
                {
                    StartGameH2C.Deserialize(reader, out var gameCode);
                    
                    this.OnStartedGame?.Invoke(gameCode);

                    break;
                }
                case Blueprint.Messages.MessageType.RemoveGame:
                {
                    RemoveGameS2C.Deserialize(reader, out var disconnectReason);
                    
                    this.OnRemovedGame?.Invoke(disconnectReason);
                    
                    break;
                }
                case Blueprint.Messages.MessageType.RemovePlayer:
                {
                    RemovePlayerS2C.Deserialize(
                        reader,
                        out var gameCode,
                        out var clientId,
                        out var hostId,
                        out var disconnectReason);
                    
                    this.OnRemovedPlayer?.Invoke(gameCode, clientId, hostId, disconnectReason);

                    break;
                }
                case Blueprint.Messages.MessageType.GameData:
                {
                    // TODO: implement game data parsing in Blueprint

                    break;
                }
                case Blueprint.Messages.MessageType.GameDataTo:
                {
                    break;
                }
                case Blueprint.Messages.MessageType.JoinedGame:
                {
                    JoinedGameH2C.Deserialize(
                        reader,
                        out var gameCode,
                        out var playerId,
                        out var hostId,
                        out var otherPlayerIds);
                    
                    this.OnJoinedGame?.Invoke(gameCode, playerId, hostId, otherPlayerIds);

                    break;
                }
                case Blueprint.Messages.MessageType.EndGame:
                {
                    EndGameH2C.Deserialize(reader, out var gameCode, out var gameOverReason, out var showAd);
                    
                    this.OnEndedGame?.Invoke(gameCode, gameOverReason, showAd);
                    
                    break;
                }
            }
            
            return ValueTask.CompletedTask;
        }
    }
}