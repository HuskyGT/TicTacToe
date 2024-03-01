﻿
using BepInEx;
using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Reflection;
using System.Text;
using UnityEngine;
using Utilla;

namespace TicTacToe
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [ModdedGamemode("TicTacToe", "TICTACTOE", Utilla.Models.BaseGamemode.Casual)]
    public class Plugin : BaseUnityPlugin
    {
        public static bool InModdedRoom;

        void Awake()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
            /*byte[] decodedBytes = Convert.FromBase64String(encodedText);
            string decodedText = System.Text.Encoding.UTF8.GetString(decodedBytes);*/

            byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes("480BF235B17212F");
            string encodedText = Convert.ToBase64String(encodedBytes);
            print(encodedText);
        }

        void OnEnable()
        {
            if (!InModdedRoom)
                return;
       
            Board.Instance.gameObject.SetActive(true);
        }

        void OnDisable()
        {
            Board.Instance.gameObject.SetActive(false);
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            var str = Assembly.GetExecutingAssembly().GetManifestResourceStream("TicTacToe.Resources.board");
            var bundle = AssetBundle.LoadFromStream(str);
            var board = GameObject.Instantiate(bundle.LoadAsset<GameObject>("Board"));
            board.gameObject.AddComponent<Board>();
            board.AddComponent<NetworkManager>();
            board.SetActive(false);
            bundle.Unload(false);
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            InModdedRoom = true;
            Board.Instance.gameObject.SetActive(true);
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            InModdedRoom = false;
            Board.Instance.ResetBoard();
            Board.Instance.gameObject.SetActive(false);
        }
    }
}
