using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace TicTacToe
{
    internal class NetworkManager : MonoBehaviourPunCallbacks
    {
        internal static NetworkManager instance;
        const byte turnCode = 42;

        void Awake()
        {
            instance = this;
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        void OnEvent(EventData data)
        {
            switch (data.Code)
            {
                case turnCode:
                    var sender = PhotonNetwork.CurrentRoom.GetPlayer(data.Sender);
                    if (sender == null)
                        return;

                    Debug.Log($"Received Play Turn Event From {sender.NickName}");
                    var eventData = (object[])data.CustomData;
                    Board.Instance.PlayTurn(sender, (byte)eventData[0], (byte)eventData[1]);
                    break;
            }
        }

        internal void PlayTurn(byte x, byte y)
        {
            object[] contents = new object[]
            {
                x, y
            };
            var raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.Others
            };
            PhotonNetwork.RaiseEvent(turnCode, contents, raiseEventOptions, SendOptions.SendReliable);
            Debug.Log($"Sent Event for space {x}, {y}");
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (otherPlayer == Board.Instance.players[0] || otherPlayer == Board.Instance.players[1])
                Board.Instance.ResetBoard();
        }
    }
}
