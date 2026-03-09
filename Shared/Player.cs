using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;

	public override void _EnterTree()
	{
		// 씬 트리에 들어올 때, 이 노드의 이름(숫자 ID)을 권한(Authority)으로 설정합니다.
        // 이렇게 하면 해당 ID를 가진 유저만 이 캐릭터를 조종할 수 있습니다.
        SetMultiplayerAuthority(int.Parse(Name));
	}
	
	public override void _Ready()
	{
		GetNode<Label>("Label").Text = "User " + Name;

		if(GetMultiplayerAuthority() == Multiplayer.GetUniqueId())
		{
			GetNode<Camera2D>("Camera2D").MakeCurrent();
		}

	}

	public override void _PhysicsProcess(double delta)
	{
		// 이 캐릭터의 권한을 가진 사람(나)이 아니면 조작 코드를 무시합니다.
        if (GetMultiplayerAuthority() != Multiplayer.GetUniqueId())
        {
            return;
        }

		Vector2 velocity = Velocity;

		/*
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		*/

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity = direction * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
