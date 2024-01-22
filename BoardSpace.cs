using UnityEngine;
using Photon.Pun;

namespace TicTacToe
{
    public class BoardSpace : MonoBehaviour
    {
        public bool canPlace = true;
        public byte x, y;

        void OnTriggerEnter(Collider collider)
        {
            if (!PhotonNetwork.InRoom || PhotonNetwork.CurrentRoom.PlayerCount < 2)
                return;

            if (!Board.canPlay || !canPlace)
                return;

            if (Board.Instance.lastPlayedPlayer == PhotonNetwork.LocalPlayer)
                return;

            var component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
            if (component == null)
                return;

            canPlace = false;
            Board.Instance.PlayTurn(PhotonNetwork.LocalPlayer, x, y);
            NetworkManager.instance.PlayTurn(x, y);
        }
    }
}
