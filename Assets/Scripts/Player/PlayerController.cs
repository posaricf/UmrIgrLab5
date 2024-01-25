using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IOnEventCallback
{
    [SerializeField] private GameObject _cameraHolder;
    private Rigidbody _rigidbody;

    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _rotateSensitivity;
    [SerializeField] private Image _healthBar;
    [SerializeField] private GameObject _UI;

    [SerializeField] private Item[] _items;
    private int _currentlyEquippedItemIndex = -1;
    private int _previousItemIndex = -1;

    private float _verticalRotation;
    private bool _onFloor;
    private Vector3 _smoothMoveVelocity;
    private Vector3 _moveAmount;

    private float _maxRotation;
    private PhotonView _photonView;

    private float _health = 100f;
    private float _maxHealth = 100f;

    private PlayerManager _playerManager;

    private bool _activeWeapons = true;
    public bool _changeColor = false;

    private bool _gameOn = true;

    public enum EventCodes : byte
    {
        RefreshTimer,
        EndMatch
    }


    private void Awake()
    {
        _UI.SetActive(true);
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();
        _playerManager = PhotonView.Find((int)_photonView.InstantiationData[0]).GetComponent<PlayerManager>();
        _maxRotation = 90f;
        _onFloor = true;
    }

    private void Start()
    {
        if (!_photonView.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(_rigidbody);
            Destroy(_UI);
        }
        else
        {
            if (_activeWeapons) EquipItem(0);
            FindObjectOfType<GameUIManager>().SetPlayerPV(_photonView);
        }
    }

    private void Update()
    {
        if (_photonView.IsMine && _gameOn)
        {
            Look();
            Move();
            Jump();
            SwitchWeapon();
            ChangeColor();
            if (_activeWeapons) UseItem();
        }
    }

    private void ChangeColor()
    {
        if (!Input.GetKeyDown(KeyCode.C)) return;
        _changeColor = false;
        GetComponent<PlayerVisuals>().ChangeColor();
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * _rotateSensitivity);
        _verticalRotation += Input.GetAxisRaw("Mouse Y") * _rotateSensitivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -_maxRotation, _maxRotation);
        _cameraHolder.transform.localEulerAngles = Vector3.left * _verticalRotation;
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        _moveAmount = Vector3.SmoothDamp(_moveAmount, moveDirection * (Input.GetKey(KeyCode.LeftShift) ? _runSpeed : _walkSpeed), ref _smoothMoveVelocity, _smoothTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _onFloor)
        {
            _rigidbody.AddForce(transform.up * _jumpForce);
        }
    }

    public void OnFloor(bool onFloor)
    {
        _onFloor = onFloor;
    }

    private void FixedUpdate()
    {
        //all physics and movement here, so movement isnt impacted by FPS
        if (_photonView.IsMine)
        {
            if (!Physics.CheckBox(_rigidbody.position + transform.TransformDirection(_moveAmount * Time.deltaTime) + Vector3.up * 4, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity))
            {
                _rigidbody.MovePosition(_rigidbody.position + transform.TransformDirection(_moveAmount * Time.deltaTime));
            }
        }
    }

    private void EquipItem(int index)
    {
        if (_items.Length < 1) return;
        if (index == _currentlyEquippedItemIndex) return;
        if (index < 0) _currentlyEquippedItemIndex = _items.Length - 1;
        else if (index > (_items.Length - 1)) _currentlyEquippedItemIndex = 0;
        else _currentlyEquippedItemIndex = index;
        _items[_currentlyEquippedItemIndex]._itemGameObject.SetActive(true);

        if (_previousItemIndex != -1)
        {
            _items[_previousItemIndex]._itemGameObject.SetActive(false);
        }
        _previousItemIndex = _currentlyEquippedItemIndex;

        //syncing - sending info to other players
        if (_photonView.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", _currentlyEquippedItemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        }
    }

    private void SwitchWeapon()
    {
        for (int i = 0; i < _items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }

        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            //scrolling up 
            EquipItem(_currentlyEquippedItemIndex + 1);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            //scrolling up 
            EquipItem(_currentlyEquippedItemIndex + -1);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!_photonView.IsMine && targetPlayer == _photonView.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void UseItem()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _items[_currentlyEquippedItemIndex].Use();
        }
    }

    public void TakeDamage(float damage, Player shooter)
    {
        /*
         *Prilikom gadanja drugih igraca, iz oruzja se ispaljuju metci (prefab Bullet)
         * sa istoimenom skriptom Bullet. U toj se skripti detektira sudar s drugim tijelom,
         * (OnCollisionEnter) te se poziva ova metoda samo na racunalu igraca koji je ispucao
         * taj metak. Ova metoda se zatim odvija na napadacevom racunalu, na objektu igraca
         * koji je pogoden. Sada je potrebno pozvati ovu metodu na racunalu vlasnika pogodenog igraca
         * koristeci poziv udaljenih procedura (engl. Remote Procedure Calls - RPC). Radi
         * lakseg snalazenja u kodu, u ovom projektu sve RPC metode su imenovane tako da
         * zapocinju RPC_ prefiksom. Takoder, iznad svake takve metode potreban je [RPC] 
         * atribut.
         * U ovoj metodi, potrebno je pozvati RPC_TakeDamage metodu koja ce se izvesti na svim racunalima
         * jer ne znamo na kojem se tocno racunalu nalazi vlasnik pogodenog igraca.
         * 
         * Provjerite kako se poziva RPC metoda tj. pomocu kojih argumenata i koji je njihov redoslijed.
         * U ovom projektu, sve se RPC metode pozivaju na svim racunalima.
         */
        _photonView.RPC("RPC_TakeDamage", RpcTarget.All, damage, shooter);
    }

    [PunRPC]

    void RPC_TakeDamage(float damage, Player shooter)
    {
        /*
         * Ova se metoda odvija na racunalima svih korisnika, ali samo onaj korisnik 
         * koji je izgubio zdravlje treba izvrsiti ovaj kôd. 
         * Prvo je potrebno provjeriti da li je trenutni korisnik vlasnik ovog objekta.
         * Ako nije, potrebno je samo napraviti povratak iz metode, a ako je, onda je potrebno od
         * ukupnog zdravlja (_health) oduzeti stetu (damage) i azurirati _healthBar koji
         * je prikazan na ekranu igraca (Linija kôda koja slijedi).
         */

        if (!_photonView.IsMine)
        {
            return;
        }

        _health -= damage;
        if (_healthBar != null)
        {
            _healthBar.GetComponent<Image>().fillAmount = (_health / _maxHealth);
        }
        /*
         * Nakon toga potrebno je provjeriti da li je igracevo zdravlje ispod nule.
         * Ako je ispod nule, to znaci da je igrac unisten i potrebno je azurirati scoreboard
         * i unistiti objekt igraca.
         * Azuriranje scoreboarda odvija se pomocu metode RPC_KillScoreboard, a igrac se unistava
         * koristenjem metode Die().
         * RPC_KillScoreboard azuirira broj bodova za igraca koji je ispucao metak (varijabla shooter).
         * Pripazite na argumente koje saljete.
         * Postoji i metoda RPC_DeathScoreboard koji azurira bodove za smrti/unistavanja igraca, ali ona se poziva
         * kada se igrac unistava.
         */
        if (_health <= 0)
        {
            _photonView.RPC("RPC_KillScoreboard", RpcTarget.All, shooter);
            Die();
        }
    }

    private void Die()
    {
        /*
         * Ovdje je potrebno pozvati RPC_Deathscoreboard sa ispravnim argumentom.
         * Ispravni argument je onaj koji "govori" da se radi o trenutnom igracu tj. 
         * vlasniku ovog prefaba. (jedno od svojstava _photonView komponente).
         */
        _photonView.RPC("RPC_DeathScoreboard", RpcTarget.All, _photonView.Owner);
        _playerManager.Die();

    }
    [PunRPC]
    void RPC_DeathScoreboard(Player player)
    {
        FindObjectOfType<Scoreboard>().PlayerDied(player);
        FindObjectOfType<KDCounter>().Deaths(player);
    }

    [PunRPC]
    void RPC_KillScoreboard(Player player)
    {
        FindObjectOfType<Scoreboard>().PlayerKills(player);
        FindObjectOfType<KDCounter>().Kills(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "InstaDeath")
        {
            Die();
        }

    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code >= 200) return;
        int code = (int)photonEvent.Code;
        switch (code)
        {
            case ((int)EventCodes.EndMatch):
                EndGame();
                break;
            default:
                break;
        }
    }

    private void EndGame()
    {
        if (_photonView.IsMine)
        {
            _gameOn = false;
            _UI.SetActive(false);
            _rigidbody.isKinematic = false;
        }
    }
}
