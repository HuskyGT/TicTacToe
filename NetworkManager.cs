using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

namespace TicTacToe
{
    internal class NetworkManager : MonoBehaviourPunCallbacks
    {
        internal static NetworkManager instance;
        const byte turnCode = 42;
        const byte startDataCode = 43;
        bool recievedData = false;

        void Awake()
        {
            instance = this;
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        internal void OnEvent(EventData data)
        {
            if (data.Code != turnCode && data.Code != startDataCode)
                return;

            var sender = PhotonNetwork.CurrentRoom.GetPlayer(data.Sender);
            var eventData = (object[])data.CustomData;

            switch (data.Code)
            {
                case turnCode:
                    if (sender == null)
                        return;

                    Debug.Log($"Received Play Turn Event From {sender.NickName}");

                    Board.Instance.PlayTurn(sender, (byte)eventData[0], (byte)eventData[1]);
                    break;

                case startDataCode:
                    if (recievedData || eventData.Length != 10)
                        return;

                    Debug.Log($"Received Start Data From {sender.NickName}");
                    for (int i = 0; i < 9; i++)
                    {
                        byte row = (byte)(i / 3);
                        byte col = (byte)(i % 3);
                        var spaceCharactor = (byte)eventData[i];

                        //Debug.Log($"Setting {row}, {col} to {spaceCharactor}");
                        Board.Instance.ChangeSpace(spaceCharactor, row, col);
                    }
                    Board.Instance.players[0] = sender;
                    var num = (int)eventData[9];
                    if (num != -1)
                    {
                        Board.canPlay = false;
                        Board.Instance.players[1] = PhotonNetwork.LocalPlayer.Get(num);
                    }
                    recievedData = true;
                    break;
            }
        }

        internal void PlayTurn(byte x, byte y)
        {
            object[] contents = new object[]
            {
                x, y
            };
            SendRaiseEvent(turnCode, contents, ReceiverGroup.Others);
            Debug.Log($"Sent Event for space {x}, {y}");
        }

        IEnumerator HandleNewPlayer(Player player)
        {
            yield return new WaitForSeconds(0.3f);
            if (player.CustomProperties.ContainsKey("TicTacToe") && Board.Instance.players[0] == PhotonNetwork.LocalPlayer)
            {
                Debug.Log(player.NickName + " Has TicTacToe");
                Debug.Log($"Sending board information to {player.NickName}");
                SendBoardInformation(player);
            }
        }
        public override void OnPlayerEnteredRoom(Player player)
        {
            StartCoroutine(HandleNewPlayer(player));
        }

        internal void SendBoardInformation(Player player)
        {
            Board b = Board.Instance;
            object[] contents = new object[]
            {
                b.spaces[0, 0], b.spaces[0, 1], b.spaces[0, 2], b.spaces[1, 0], b.spaces[1, 1], b.spaces[1, 2], b.spaces[2, 0],  b.spaces[2, 1], b.spaces[2, 2],
                b.players[1] != null ? b.players[1].ActorNumber : -1
            };

            SendRaiseEvent(startDataCode, contents, player.ActorNumber);
        }

        public override void OnPlayerLeftRoom(Player player)
        {
            base.OnPlayerLeftRoom(player);
            if (player == Board.Instance.players[0] || player == Board.Instance.players[1])
                Board.Instance.ResetBoard();
        }

        public override void OnLeftRoom()
        {
            recievedData = false;
        }

        internal static void SendRaiseEvent(byte code, object contents)
        {
            var raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.All
            };
            PhotonNetwork.RaiseEvent(code, contents, raiseEventOptions, SendOptions.SendReliable);
        }
        internal static void SendRaiseEvent(byte code, object contents, params int[] targetActors)
        {
            var raiseEventOptions = new RaiseEventOptions()
            {
                TargetActors = targetActors
            };
            PhotonNetwork.RaiseEvent(code, contents, raiseEventOptions, SendOptions.SendReliable);
        }
        internal static void SendRaiseEvent(byte code, object contents, ReceiverGroup receivers)
        {
            var raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = receivers
            };
            PhotonNetwork.RaiseEvent(code, contents, raiseEventOptions, SendOptions.SendReliable);
        }
    }
}
