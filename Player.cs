using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 3.0f;
	public const float MouseSensitivity = 0.002f;
	public const float JumpVelocity = 4.5f;

	private Node3D _head;
    private Camera3D _camera;

	public override void _Ready()
    {
        _head = GetNode<Node3D>("Head");
        _camera = GetNode<Camera3D>("Head/Camera3D");

        // 이 캐릭터의 제어 권한이 나에게 있는지 확인 (로컬 플레이어)
        if (IsMultiplayerAuthority())
        {
            _camera.MakeCurrent(); // 내 카메라 활성화
            Input.MouseMode = Input.MouseModeEnum.Captured; // 마우스 커서 숨기기 및 고정
        }
    }

	public override void _Input(InputEvent @event)
    {
        // 내 캐릭터가 아니면 마우스 입력을 무시합니다.
        if (!IsMultiplayerAuthority()) return;

        if (@event is InputEventMouseMotion mouseMotion)
        {
            // 마우스 좌우 이동 -> 몸체 전체 회전
            RotateY(-mouseMotion.Relative.X * MouseSensitivity);
            
            // 마우스 상하 이동 -> 고개(Head)만 회전
            _head.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);

            // 고개가 너무 뒤로 넘어가거나 꺾이지 않도록 각도 제한 (-80도 ~ 80도)
            Vector3 headRot = _head.Rotation;
            headRot.X = Mathf.Clamp(headRot.X, Mathf.DegToRad(-80), Mathf.DegToRad(80));
            _head.Rotation = headRot;
        }
    }

	public override void _PhysicsProcess(double delta)
    {
        // 내 캐릭터가 아니면 이동 물리 연산을 무시 (서버가 위치를 동기화해줌)
        if (!IsMultiplayerAuthority()) return;

        Vector3 velocity = Velocity;

        // 중력 적용
        if (!IsOnFloor())
            velocity.Y -= 9.8f * (float)delta;

        // 키보드 이동 입력 처리 (Project Settings -> Input Map 설정 필요)
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
        
        // 캐릭터가 바라보는 방향을 기준으로 이동 벡터 계산
        Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
