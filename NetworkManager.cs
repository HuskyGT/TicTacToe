using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using PlayFab.ClientModels;
using System.Linq;
using UnityEngine;

namespace TicTacToe
{
    internal class NetworkManager : MonoBehaviourPunCallbacks
    {
        internal static NetworkManager instance;
        const byte turnCode = 87;
        const byte startDataCode = 88;
        bool recievedData = false;

        void Awake()
        {
            instance = this;
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        void OnEvent(EventData data)
        {
            //no errors before errors now so this is here now
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
                // Added startDataCode this is to recieve data for new players
                case startDataCode:
                    if (sender == null || recievedData) return;
                    if (eventData.Length != 10) { Debug.Log($"Start Data has wrong data amount from: {sender.NickName}"); return; }

                    Debug.Log($"Received Start Data Event From {sender.NickName}");
                    for (int i = 0; i < eventData.Length - 1; i++)
                    {
                        if (i <= 2)
                        {
                            Board b = Board.Instance;
                            b.spaces[0, i] = (char)eventData[i];
                            var space = b.boardSpaces.First(space => space.x == 0 && space.y == (byte)eventData[i]);
                            space.canPlace = false;
                            // Checks is char is used or not (X or O)
                            if ((char)eventData[i] != 'X' && (char)eventData[i] != 'O') { return; }
                            space.transform.GetChild((char)eventData[i] == 'X' ? 0 : 1).gameObject.SetActive(true);
                        }
                        else if (i <= 5)
                        {
                            Board b = Board.Instance;
                            b.spaces[1, i] = (char)eventData[i];
                            var space = b.boardSpaces.First(space => space.x == 1 && space.y == (byte)eventData[i]);
                            space.canPlace = false;
                            // Checks is char is used or not (X or O)
                            if ((char)eventData[i] != 'X' && (char)eventData[i] != 'O') { return; }
                            space.transform.GetChild((char)eventData[i] == 'X' ? 0 : 1).gameObject.SetActive(true);

                        }
                        else if (i <= 8)
                        {
                            Board b = Board.Instance;
                            b.spaces[2, i] = (char)eventData[i];
                            var space = b.boardSpaces.First(space => space.x == 2 && space.y == (byte)eventData[i]);
                            space.canPlace = false;
                            // Checks is char is used or not (X or O)
                            if ((char)eventData[i] != 'X' && (char)eventData[i] != 'O') { return; }
                            space.transform.GetChild((char)eventData[i] == 'X' ? 0 : 1).gameObject.SetActive(true);
                        }
                    }
                    var player = PhotonNetwork.PlayerList.First(player => player.UserId == (string)eventData[9]);
                    Board.Instance.players[0] = sender;
                    if (sender != player)
                    {
                        Board.Instance.players[1] = player;
                    }
                    Board.Instance.lastPlayedPlayer = player;
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
            var raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.Others
            };
            PhotonNetwork.RaiseEvent(turnCode, contents, raiseEventOptions, SendOptions.SendReliable);
            Debug.Log($"Sent Event for space {x}, {y}");
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (!newPlayer.CustomProperties.ContainsKey("TicTacToe") || Board.Instance.players[0] == null || Board.Instance.players[0] != PhotonNetwork.LocalPlayer || Board.Instance.lastPlayedPlayer == null)
                return;

            Debug.Log($"Sending board information");
            handleNewBoardPlayer(newPlayer);
        }

        // Added this to handle a new joined player to Re-Sync them with the game
        internal void handleNewBoardPlayer(Player newPlayer)
        {
            Board b = Board.Instance;
            object[] contents = new object[]
            {
                // Space data

                b.spaces[0, 0], b.spaces[0, 1], b.spaces[0, 2], b.spaces[1, 0], b.spaces[1, 1], b.spaces[1, 2], b.spaces[2, 0],  b.spaces[2, 1], b.spaces[2, 2],

                // General board data
                b.lastPlayedPlayer.UserId
            };

            var raiseEventOptions = new RaiseEventOptions()
            {
                Receivers = ReceiverGroup.Others
            };
            PhotonNetwork.RaiseEvent(startDataCode, contents, raiseEventOptions, SendOptions.SendReliable);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (otherPlayer == Board.Instance.players[0] || otherPlayer == Board.Instance.players[1])
                Board.Instance.ResetBoard();
        }

        public override void OnLeftRoom()
        {
            recievedData = false;
        }
    }
}
