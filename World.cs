using Godot;
using System;

public partial class World : Node3D
{
	private const int Port = 8910; // 임의의 포트 번호
    private const string Address = "127.0.0.1"; // 로컬 테스트용 주소

    private PackedScene _playerScene;
    private Node3D _playersContainer;
    private CanvasLayer _ui;

    public override void _Ready()
    {
        // Player 씬 미리 불러오기
        _playerScene = GD.Load<PackedScene>("res://Player.tscn"); // Player.tscn 경로가 맞는지 확인하세요!
        _playersContainer = GetNode<Node3D>("Players");
        _ui = GetNode<CanvasLayer>("UI");

        // 버튼 클릭 이벤트 연결
        GetNode<Button>("UI/HostButton").Pressed += OnHostButtonPressed;
        GetNode<Button>("UI/JoinButton").Pressed += OnJoinButtonPressed;

        // 누군가 서버에 접속했을 때 발생하는 이벤트 연결
        Multiplayer.PeerConnected += OnPeerConnected;
    }

    private void OnHostButtonPressed()
    {
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        peer.CreateServer(Port); // 서버 생성
        Multiplayer.MultiplayerPeer = peer;

        _ui.Hide(); // UI 숨기기

        // 방장(나 자신) 캐릭터 스폰 (서버의 고유 ID는 항상 1입니다)
        SpawnPlayer(1);
    }

    private void OnJoinButtonPressed()
    {
        ENetMultiplayerPeer peer = new ENetMultiplayerPeer();
        peer.CreateClient(Address, Port); // 클라이언트로 접속
        Multiplayer.MultiplayerPeer = peer;

        _ui.Hide(); // UI 숨기기
    }

    // 서버에 누군가 들어왔을 때 실행됨 (방장 측에서만 실행됨)
    private void OnPeerConnected(long id)
    {
        // 접속한 사람의 ID로 캐릭터를 만들어줌
        SpawnPlayer(id);
    }

    private void SpawnPlayer(long id)
    {
        // 1. 플레이어 인스턴스 생성
        Player player = _playerScene.Instantiate<Player>();
        
        // 2. 노드 이름을 접속자의 고유 ID로 설정 (네트워크 동기화에 필수)
        player.Name = id.ToString(); 

        // 3. 이 캐릭터의 '조종 권한'을 해당 ID를 가진 사람에게 부여!
        // (이 코드가 있어야 저번 시간에 만든 IsMultiplayerAuthority()가 작동합니다)
        player.SetMultiplayerAuthority((int)id);
		
		player.Position = new Vector3(0, 10, 0);

        // 4. Players 폴더에 추가 (이 순간 MultiplayerSpawner가 작동하여 모든 클라이언트에 복제됨)
		_playersContainer.AddChild(player);
    }
}
