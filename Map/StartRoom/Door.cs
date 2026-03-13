using Godot;
using System;

public partial class Door : Node3D
{
    private bool _isOpen = false;
    private bool _playerInRange = false;

    [Export]
    private AnimationPlayer _animPlayer;

    // 플레이어가 상호작용 키(예: 'E')를 눌렀을 때 실행
    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("interact") && _playerInRange)
        {
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        if (_isOpen)
        {
            _animPlayer.Play("Close");
        }
        else
        {
            _animPlayer.Play("Open");
        }
        _isOpen = !_isOpen;
    }

    // Area3D의 body_entered 시그널에 연결
    private void OnInteractionAreaBodyEntered(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            _playerInRange = true;
            GD.Print("플레이어 접근: 문 상호작용 가능");
        }
    }

    // Area3D의 body_exited 시그널에 연결
    private void OnInteractionAreaBodyExited(Node3D body)
    {
        if (body.IsInGroup("player"))
        {
            _playerInRange = false;
        }
    }
}
