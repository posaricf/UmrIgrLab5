using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    [SerializeField] private List<Material> _playerMaterials;
    private int _currentColor = 0;
    [SerializeField] private GameObject _player;
    private MeshRenderer _meshRenderer;
    [SerializeField] PhotonView _photonView;

    private void Awake()
    {
        _meshRenderer = _player.GetComponent<MeshRenderer>();
    }

    public void ChangeColor()
    {
        /*
         * pritiskom na slovo C mijenja se boja igraca na nacin da se iz liste materijala
         * (_playerMaterials) odabire sljedeca boja (_currentColor+1), ali je potrebno 
         * tu brojku staviti u interval postojeceg broja boja. Nakon odabira boje (_currentColor)
         * potrebno je tu boju postaviti za materijal (_meshRenderer) igraca.
         * Nakon sto je boja lokalno promijenjena, potrebno je promijeniti boju tog igraca na svim ostalim racunalima
         * pozivom udaljene procedure RPC_ChangeColor.
         */
        _currentColor = (_currentColor + 1) % _playerMaterials.Count;
        _meshRenderer.material = _playerMaterials[_currentColor];
        _photonView.RPC("RPC_ChangeColor", RpcTarget.All, _currentColor);
    }

    [PunRPC]

    void RPC_ChangeColor(int color)
    {
        //ovdje je samo potrebno postaviti trenutnu boju igraca na onu
        //koja je proslijedena ovoj proceduri.
        _meshRenderer.material = _playerMaterials[color];
    }
}
