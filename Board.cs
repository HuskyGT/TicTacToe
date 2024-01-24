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
        public bool gameStarted;
        public static bool canPlay = true;
        public List<BoardSpace> boardSpaces = new List<BoardSpace>();
        public Player[] players = new Player[2];
        public Player lastPlayedPlayer;
        public char[,] spaces =
        {
            { 'N','N','N' },
            { 'N','N','N' },
            { 'N','N','N' }
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
            spaces = new char[,]
            {
                { 'N','N','N' },
                { 'N','N','N' },
                { 'N','N','N' }
            };
            canPlay = true;
            gameStarted = false;
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
            var playerChar = players[0] == player ? 'X' : 'O';
            ChangeSpace(playerChar, x, y);
            HandleWinner();
        }

        void ChangeSpace(char value, byte x, byte y)
        {
            spaces[x, y] = value;
            var space = boardSpaces.First(space => space.x == x && space.y == y);
            space.canPlace = false;
            space.transform.GetChild(value == 'X' ? 0 : 1).gameObject.SetActive(true);
        }

        void HandleWinner()
        {
            var winner = GetWinner();
            if (winner == 'N')
                return;

            StartCoroutine(EndGame());
        }
        IEnumerator EndGame()
        {
            canPlay = false;
            yield return new WaitForSeconds(2);
            ResetBoard();
            canPlay = true;
        }
        char GetWinner()
        {
            for (int playerIndex = 0; playerIndex < 2; playerIndex++)
            {
                var player = players[playerIndex];
                var playerChar = players[0] == player ? 'X' : 'O';
                var diagonalsCondition = (spaces[0, 2] == playerChar && spaces[1, 1] == playerChar && spaces[2, 0] == playerChar) || (spaces[0, 0] == playerChar && spaces[1, 1] == playerChar && spaces[2, 2] == playerChar);
                if (diagonalsCondition)
                    return playerChar;

                for (int i = 0; i < 3; i++)
                {
                    var horizontalsCondition = spaces[i, 0] == playerChar && spaces[i, 1] == playerChar && spaces[i, 2] == playerChar;
                    var vertialsCondition = spaces[0, i] == playerChar && spaces[1, i] == playerChar && spaces[2, i] == playerChar;
                    if (horizontalsCondition || vertialsCondition)
                        return playerChar;
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
                return 'D';

            return 'N';
        }
    }
}