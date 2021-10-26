using System;
using System.Net;
using System.Threading.Tasks;
using Blueprint.Messages.H2C;
using Blueprint.Messages.Objects;
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

        public Client(IPEndPoint ipEndPoint, ObjectPool<MessageReader> readerPool)
        {
            this.Connection = new UdpClientConnection(ipEndPoint, readerPool)
            {
                DataReceived = OnDataReceived,
                Disconnected = OnDisconnect
            };
        }

        public async Task StartAsync()
        {
            await this.Connection.ConnectAsync(this.GetConnectionData());
        }

        public void Stop()
        {
            this.Connection.Stop();
        }

        private async ValueTask OnDataReceived(DataReceivedEventArgs e)
        {
            await Console.Out.WriteLineAsync("Got data");

            while (e.Message.Position < e.Message.Length)
            {
                await this.HandleMessageAsync(e.Message.ReadMessage(), e.Type);
            }
        }

        private async ValueTask OnDisconnect(DisconnectedEventArgs e)
        {
            await Console.Out.WriteLineAsync("disconnected");
        }

        private async ValueTask HandleMessageAsync(IMessageReader reader, MessageType type)
        {
            var flag = (Blueprint.Messages.MessageType) reader.Tag;

            switch (flag)
            {
                case Blueprint.Messages.MessageType.HostGame:
                {
                    HostGameS2C.Deserialize(reader, out var gameCode);

                    this.OnHostedGame?.Invoke(gameCode);

                    break;
                }
                case Blueprint.Messages.MessageType.JoinedGame:
                {
                    JoinedGameH2C.Deserialize(
                        reader,
                        out GameCode gameCode,
                        out uint playerId,
                        out uint hostId,
                        out uint[] otherPlayerIds);
                    
                    this.OnJoinedGame?.Invoke(gameCode, playerId, hostId, otherPlayerIds);

                    break;
                }
            }
        }
    }
}