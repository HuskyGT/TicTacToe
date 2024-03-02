using UnityEngine;
using Photon.Pun;

namespace TicTacToe
{
    public class BoardSpace : MonoBehaviour
    {
        public bool canPlace = true;
        public byte x, y;
        public Renderer[] renderers;

        public void SetColor(Color color)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = color;
            }
        }
        bool PlayersHaveMod()
        {
            foreach(var player in PhotonNetwork.PlayerList)
            {
                var modCondition = player.CustomProperties.ContainsKey("TicTacToe") && player != PhotonNetwork.LocalPlayer;
                if (modCondition)
                {
                    return true;
                }
            }
            return false;
        }
        void OnTriggerEnter(Collider collider)
        {
            if (!PhotonNetwork.InRoom)
                return;

            if (!Board.canPlay || !canPlace)
                return;

            if (Board.Instance.lastPlayedPlayer == PhotonNetwork.LocalPlayer)
                return;

            if (Board.Instance.players[0] != null && Board.Instance.players[1] != null && Board.Instance.players[0] != PhotonNetwork.LocalPlayer && Board.Instance.players[1] != PhotonNetwork.LocalPlayer)
                return;

            var component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
            if (component == null)
                return;

            Board.Instance.PlayTurn(PhotonNetwork.LocalPlayer, x, y);
            NetworkManager.instance.PlayTurn(x, y);
        }
    }
}
