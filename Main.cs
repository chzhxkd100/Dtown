using Godot;
using System;

public partial class Main : Node2D
{
	private ENetMultiplayerPeer _peer = new ENetMultiplayerPeer();
    private PackedScene _playerScene = GD.Load<PackedScene>("res://Shared/player.tscn"); // 저장한 Player 경로에 맞게 수정하세요

    public override void _Ready()
    {
        // 버튼 이벤트 연결
        GetNode<Button>("CanvasLayer/HostButton").Pressed += OnHostPressed;
        GetNode<Button>("CanvasLayer/JoinButton").Pressed += OnJoinPressed;

        // 누군가 접속하거나 나갔을 때 실행될 서버측 이벤트 연결
        Multiplayer.PeerConnected += AddPlayer;
        Multiplayer.PeerDisconnected += RemovePlayer;
    }

    private void OnHostPressed()
    {
        _peer.CreateServer(7000);
        Multiplayer.MultiplayerPeer = _peer;
        
        // 서버를 연 본인도 플레이어로서 소환
        AddPlayer(Multiplayer.GetUniqueId());
        GetNode<CanvasLayer>("CanvasLayer").Hide(); // UI 숨기기
    }

    private void OnJoinPressed()
    {
        _peer.CreateClient("127.0.0.1", 7000);
        Multiplayer.MultiplayerPeer = _peer;
        GetNode<CanvasLayer>("CanvasLayer").Hide(); // UI 숨기기
    }

    // 서버가 플레이어를 소환하는 함수
    private void AddPlayer(long peerId)
    {
		//서버만 캐릭터 생성
		if (!Multiplayer.IsServer())
        {
            return;
        }

        Player player = _playerScene.Instantiate<Player>();
        
        // ⭐️매우 중요: 노드 이름을 접속한 유저의 ID로 설정해야 권한(Authority)이 부여됩니다.
        player.Name = peerId.ToString(); 
        
        // Players 폴더 아래에 추가 (그러면 Spawner가 알아서 모든 클라이언트 화면에 복사해줍니다)
        GetNode<Node2D>("Players").AddChild(player);
    }

    private void RemovePlayer(long peerId)
    {
        var player = GetNodeOrNull<Node2D>($"Players/{peerId}");
        if (player != null)
        {
            player.QueueFree();
        }
    }
}
