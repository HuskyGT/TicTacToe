using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace TicTacToe
{
    public class Board : MonoBehaviour
    {
        public static Board Instance;
        public static bool canPlay = true;
        public List<BoardSpace> boardSpaces = new List<BoardSpace>();
        public Player[] players = new Player[2];
        public Player lastPlayedPlayer;
        GameObject winningLine;
        BoardSpace[] winningSpaces;
        //this is the lazy way of fixing my issues
        public byte[,] spaces =
        {
            { 3,3,3 },
            { 3,3,3 },
            { 3,3,3 }
        };

        void Awake()
        {
            Instance = this;
            for (int i = 0; i < 9; i++)
            {
                var space = gameObject.transform.GetChild(i).gameObject.GetComponent<BoardSpace>();
                space.x = (byte)(i / 3);
                space.y = (byte)(i % 3);
                boardSpaces.Add(space);
            }
        }

        public void ResetBoard()
        {
            foreach (BoardSpace space in boardSpaces)
            {
                space.transform.GetChild(0).gameObject.SetActive(false);
                space.transform.GetChild(1).gameObject.SetActive(false);
                space.canPlace = true;
            }
            spaces = new byte[,]
            {
                { 3,3,3 },
                { 3,3,3 },
                { 3,3,3 }
            };
            canPlay = true;
            lastPlayedPlayer = null;
            players[0] = null;
            players[1] = null; 
        }

        public void PlayTurn(Player player, byte x, byte y)
        {
            if (players[0] == null)
            {
                players[0] = player;
            }
            if (players[1] == null && players[0] != player)
            {
                players[1] = player;
            }

            if (player != players[0] && player != players[1])
                return;

            if (x > 3 || x < 0 || y < 0 || y > 3)
                return;

            lastPlayedPlayer = player;
            byte playerChar = (byte)(players[0] == player ? 0 : 1);
            ChangeSpace(playerChar, x, y);
            HandleWinner();
        }

        public void ChangeSpace(byte value, byte x, byte y)
        {
            spaces[x, y] = value;
            var space = boardSpaces.First(space => space.x == x && space.y == y);
            if (value != 3)
            {
                space.canPlace = false;
                space.transform.GetChild(value == 0 ? 0 : 1).gameObject.SetActive(true);
            }
        }

        void HandleWinner()
        {
            var winner = GetWinner();
            if (winner == 3)
                return;

            StartCoroutine(EndGame());
        }
        IEnumerator EndGame()
        {
            canPlay = false;
            //winningLine.SetActive(true);
            //winningLine.transform.position = winningSpaces[1].transform.position;
            //winningLine.transform.LookAt(winningSpaces[0].transform.position);
            yield return new WaitForSeconds(2);
            ResetBoard();
            canPlay = true;
        }
        byte GetWinner()
        {
            //what a fucking mess
            for (int playerIndex = 0; playerIndex < 2; playerIndex++)
            {
                var player = players[playerIndex];
                byte playerChar = (byte)(players[0] == player ? 0 : 1);
                //var diagonalsCondition = (spaces[0, 2] == playerChar && spaces[1, 1] == playerChar && spaces[2, 0] == playerChar) || (spaces[0, 0] == playerChar && spaces[1, 1] == playerChar && spaces[2, 2] == playerChar);
                /*if (diagonalsCondition)
                {
                    return playerChar;
                }*/
                if (spaces[0, 2] == playerChar && spaces[1, 1] == playerChar && spaces[2, 0] == playerChar)
                {
                    winningSpaces = new BoardSpace[] { boardSpaces.First(space => space.x == 0 && space.y == 2), boardSpaces.First(space => space.x == 1 && space.y == 1), boardSpaces.First(space => space.x == 2 && space.y == 0) };
                    return playerChar;
                }
                if (spaces[0, 0] == playerChar && spaces[1, 1] == playerChar && spaces[2, 2] == playerChar)
                {
                    winningSpaces = new BoardSpace[] { boardSpaces.First(space => space.x == 0 && space.y == 0), boardSpaces.First(space => space.x == 1 && space.y == 1), boardSpaces.First(space => space.x == 2 && space.y == 2) };
                    return playerChar;
                }

                for (int i = 0; i < 3; i++)
                {
                    var horizontalsCondition = spaces[i, 0] == playerChar && spaces[i, 1] == playerChar && spaces[i, 2] == playerChar;
                    var vertialsCondition = spaces[0, i] == playerChar && spaces[1, i] == playerChar && spaces[2, i] == playerChar;
                    if (horizontalsCondition)
                    {
                        BoardSpace[] array = new BoardSpace[3];
                        for (int j = 0; j < 3; j++)
                        {
                            array[j] = boardSpaces.First(space => space.x == i && space.y == j);
                        }
                        winningSpaces = array;
                        return playerChar;
                    }
                    if (vertialsCondition)
                    {
                        BoardSpace[] array = new BoardSpace[3];
                        for (int j = 0; j < 3; j++)
                        {
                            array[j] = boardSpaces.First(space => space.x == j && space.y == i);
                        }
                        winningSpaces = array;
                        return playerChar;
                    }
                }
            }
            int placedCount = 0;
            foreach (var space in boardSpaces)
            {
                if (!space.canPlace)
                {
                    placedCount++;
                }
            }

            if (placedCount == 9)
                return (byte)3;

            return (byte)3;
        }
    }
}