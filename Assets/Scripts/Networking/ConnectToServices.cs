using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class ConnectToServices : MonoBehaviourPunCallbacks
{
    public static ConnectToServices Instance;

    private CanvasManager _canvasManager;
    [SerializeField] TMP_InputField _roomInputField;
    [SerializeField] GameObject _warningText;
    [SerializeField] Transform _roomListContent;
    [SerializeField] ListItem _roomListing;
    [SerializeField] Transform _playerListContent;
    [SerializeField] PlayerListItem _playerListing;
    [SerializeField] private GameObject _startButton;


    private void Awake()
    {
        Instance = this;
        _canvasManager = this.GetComponent<CanvasManager>();
    }
    void Start()
    {
        //Sto je potrebno napraviti kako bi se korisnika spojilo s Photon posluziteljem u skladu s postavkama definiranim u datoteci PhotonServerSettings?
        Debug.Log("Connecting to master...");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master.");
        /*
         * Kako bi bilo moguce vidjeti postojece sobe, pridruziti im se ili stvoriti nove sobe
         * nakon spajanja na Photon posluzitelja potrebno je spojiti se u sobu namijenjenu za
         * prethodno navedene akcije. Koja je to soba?
         */
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString();
        Debug.Log("I joined lobby " + PhotonNetwork.NickName);
    }

    public void CreateRoom()
    {
        Debug.Log("Creating new room...");
        /* 
         * Prvo je potrebno provjeriti ima li soba definirano ime (koristenjem _roomInputField polja).
         * Sobu je moguce stvoriti koristeci uneseno ime. Ako imena nema, potrebno je korisniku 
         * prikazati tekst upozorenja (_warningText) i prekinuti stvaranje nove sobe. 
         * Nakon pokusaja stvaranja sobe, ovisno o uspjesnosti ce biti pozvan jedan od dva Callbacka - Proucite koje su to dvije metode.
         * Svaku od njih potrebno je implementirati u ovoj skripti. (Metoda1 i Metoda2)
         * 
         */

        if (_roomInputField.text.Length == 0)
        {
            _warningText.SetActive(true);
            _warningText.GetComponent<TextMeshProUGUI>().text = "Cannot create a room without a name!";
            return;
        }

        PhotonNetwork.CreateRoom(_roomInputField.text);

        _canvasManager.OpenLoadScreen();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Successfully joined the room.");
        //Metoda uspjesnog stvaranja sobe
        /*
         *Ova metoda poziva se kada je korisnik uspjesno usao u sobu. Prvo je potrebno otvoriti izbornik za sobu 
         * koristeci CanvasManager.
         * 
         * U ovom izborniku nalazi se objekt PlayerListContent. Uvjerite se u to koristecu hijerarhiju scene. 
         * PlayerListContent je kontejner za manje objekte (Assets->Prefabs->UI) PlayerListItem. 
         * Referenca na kontejner u ovoj skripti je _playerListContent.
         * Svaki PlayerListItem prikazuje ime jednog korisnika u trenutacnoj sobi. Na primjer, ako se u sobi
         * nalaze 3 igraca, svaki od njih ce u ovom kontejneru vidjeti 3 PlayerListItema sa imenima sebe i druga dva igraca.
         * 
         * Kako bi dobili ovakvu strukturu, prvo je potrebno isprazniti kontejner (jer je moguce da lokalno ostanu zapisi
         * iz prethodne sobe u kojoj je korisnik bio pridruzen. 
         * Nakon sto smo sigurni da je kontejner prazan, potrebno je dohvatiti listu igraca i za svakog igraca instancirati PlayerListItem objekt 
         * (cija se referenca nalazi u _playerListing varijabli). Potrebno je instancirati objekt tako da on bude dijete od kontejnera.
         * Nakon sto je objekt instanciran potrebno je podesiti odredene parametre kako bi zaista sadrzavao podatke o igracu. 
         * Pogledajte koju komponentu na sebi ima PlayerListItem prefab i koju metodu je potrebno pozvati nakon instanciranja tog objekta.
         * 
         * Za kraj, potrebno je podesiti da jedino vlasnik sobe vidi gumb za pocetak igre (_startButton).
         * 
         * 
         */

        _canvasManager.EnterRoom();

        foreach (Transform playerListItem in _playerListContent)
        {
            Destroy(playerListItem.gameObject);
        }

        PlayerListItem newPlayer = Instantiate(_playerListing, _playerListContent.transform);
        newPlayer.SetUp(PhotonNetwork.LocalPlayer);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Error on joining room.");
        //Metoda neuspjesnog stvaranja sobe
        /*
         * Potrebno je koristeci za to namijenjenu metodu iz CanvasManagera prikazati poruku pogreske korisniku.
         */
        _canvasManager.ShowError(message);
    }


    public void LeaveRoom()
    {
        Debug.Log("Leaving room...");
        /* 
         * Prilikom pritiska na LeaveRoom gumb dok se korisnik nalazi u sobi, poziva se ova metoda.
         * Potrebno je pozvati Photon metodu za napustanje sobe, a potom otvoriti izbornik tj. platno za ucitavanje 
         * (jedna od javnih metoda objekta CanvasManager). 
         * Ako je korisnik uspjesno napustio sobu, poziva se nova metoda (Metoda3) koju je potrebno implementirati 
         */
        PhotonNetwork.LeaveRoom();
        _canvasManager.OpenLoadScreen();
    }

    public override void OnLeftRoom()
    {
        Debug.Log("User left the room.");
        /* 
         * Ovdje je potrebno koristeci CanvasManager otvoriti  naslovni izbornik.
         */
        _canvasManager.OpenTitleScreen();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Rooms updated.");
        /* 
         * Ova metoda poziva se svaki put kada dode do promjene u listi soba
         * (ili kada se stvori nova soba, ili kada se postojeca zatvori).
         * Potrebno je azurirati postojecu listu na ekranu (FindRoomMenu).
         * OVaj izbornik takoder sadrzi kontejner za posebnu klasu za prikaz soba
         * cija se referenca nalazi u _roomListContent, a za klasu u _roomListing.
         * 
         * Prvo je potrebno isprazniti kontejner (isto kao u Metoda1)
         * Zatim, iterirajuci kroz listu soba potrebno je stvoriti _roomListing za svaku
         * sobu. (takoder isto kao u Metoda1). 
         * 
         * Pazite - roomList koji je predan pri pozivu ove funkcije sadrzi sve sobe - cak i one koje vise nisu
         * otvorene (mogu biti pune, zatvorene ili skrivene)
         * Zbog toga je za svaku sobu prvo potrebno provjeriti da li se moze prikazati ili ne, te ako moze onda
         * stvoriti _roomListing za tu sobu i podesiti njegove parametre. 
         * Proucite koji je to atribut elementa ove liste u pitanju.
         * 
         */

        foreach (Transform roomListing in _roomListContent)
        {
            Destroy(roomListing.gameObject);
        }


        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room);
            if (room.IsVisible && room.IsOpen && room.PlayerCount != 0)
            {
                ListItem newRoom = Instantiate(_roomListing, _roomListContent.transform);
                newRoom.SetUp(room);
            }
        }

    }

    public void JoinRoom(RoomInfo info)
    {
        Debug.Log("Joining room...");
        /*
         * U izborniku za prikaz soba (FindRoomMenu) nalazi se kontejner (RoomListContent) 
         * koji prikazuje postojece sobe. Slicno kao za prikaz korisnika u sobama, ovaj
         * kontejner prikazuje sobe koristeci objekte RoomListItem. Igraci se mogu pridruziti postojecim sobama 
         * tako da pritisnu na njih. Prilikom pritiska na objekt tipa RoomListItem poziva se odredena metoda. 
         * Ta metoda prosljeduje informacije o sobi za pridruzivanje ovoj metodi. 
         * U ovoj metodi potrebno je korisnika pridruziti u zeljenu sobu te pomocu CanvasManagera otvoriti
         * izbornik za ucitavanje.
         */
        PhotonNetwork.JoinRoom(info.Name);
        _canvasManager.OpenLoadScreen();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("New player entered room.");
        /*
         * Ova se metoda poziva svaki puta kada novi igrac ude u postojecu sobu. 
         * Kada igraci napustaju sobu, objekti PlayerListItem sami se pobrinu da se
         * maknu iz lista. No kada se novi igrac pridruzi, novi PlayerListItem treba biti dodan
         * na ekran svakog drugog igraca u toj sobi. Ova metoda se brine za to.
         * Potrebno je samo instancirati novi objekt kao u Metoda1 i namjestiti njegove postavke.
         * Svaki igrac biti ce obavijesten prilikom  pridruzivanja novog igraca i svaki od njih ce si sam
         * prikazati novog igraca.
         */
        PlayerListItem player = Instantiate(_playerListing, _playerListContent.transform);
        player.SetUp(newPlayer);
    }

    public void StartGame()
    {
        Debug.Log("Game starting...");
        /* Za pocetak igre potrebno je pomocu Photona promijeniti scenu. Pronadite kako se mijenja scena 
         * te kako se pomocu te metode referencira scena za ucitavanje. Isprobajte promijeniti scenu
         * na instanci vlasnika sobe te pogledajte je li se scena promijenila i drugim igracima. 
         */
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }
        /*
         * Potrebno je omoguciti da svi korisnici istovremeno mijenjaju scene zajedno i sa vlasnikom sobe.
         * Proucite kako se to ostvaruje. Tu liniju koda ubacite u OnConnectedToMaster metodu.
         */
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("Room master changed.");
        /*
         * U slucaju kada se u sobi nalazi barem jedan igrac pored vlasnika sobe, a vlasnik sobe napusti sobu,
         * soba se ne zatvara, vec se vlasnistvo prenosi na drugog igraca. 
         * 
         * Prisjetite se sto je omoguceno samo vlasniku sobe i u ovom djelu koda azurirajte to svojstvo
         * novom vlasniku.
         */
        PhotonNetwork.SetMasterClient(newMasterClient);
    }
}