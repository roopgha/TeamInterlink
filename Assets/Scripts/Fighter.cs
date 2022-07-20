using Photon.Pun;
using System;
using UnityEngine;

public class Fighter : Prototype, IAction
{
	private bool attacking = false;

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();

		if (!m_photonView.IsMine) return;

		IAction.Direction direction = IAction.Direction.None;
		
		if (!Input.anyKeyDown) return;

		if (Input.GetKeyDown(KeyCode.A))
		{
			direction = PhotonNetwork.IsMasterClient ? IAction.Direction.Left : IAction.Direction.Right;
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			direction = IAction.Direction.Guard;
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			direction = PhotonNetwork.IsMasterClient ? IAction.Direction.Right : IAction.Direction.Left;
		}
		else if (Input.GetKeyDown(KeyCode.W))
		{
			direction = IAction.Direction.Up;
		}

		IAction.Skill attackType = IAction.Skill.None;

		if (Input.GetKeyDown(KeyCode.J))
		{
			attackType = IAction.Skill.Attack;

			if (!m_grounded)
			{
				attackType = IAction.Skill.JumpAttack;
			}
		}
		else if (Input.GetKeyDown(KeyCode.K))
		{
			attackType = IAction.Skill.ChargedAttack;
			
			if (!m_grounded)
			{
				attackType = IAction.Skill.JumpAttack;
			}
		}
		else if (Input.GetKeyDown(KeyCode.L))
		{
			attackType = IAction.Skill.LethalMove;
		}

		print(attackType);
	}

	public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(transform.position);
			stream.SendNext(m_procession);
			stream.SendNext(m_grounded);
			stream.SendNext(m_spriteRenderer.flipX);
			stream.SendNext(m_jump);
		}
		else if (stream.IsReading)
		{
			try
			{
				transform.position = (Vector3)stream.ReceiveNext();
				m_procession = (bool)stream.ReceiveNext();
				m_grounded = (bool)stream.ReceiveNext();
				m_spriteRenderer.flipX = (bool)stream.ReceiveNext();
				m_jump = (bool)stream.ReceiveNext();
			}
			catch (NullReferenceException) { }
		}
	}

	public virtual void Attack()
	{

	}

	public virtual void ChargedAttack()
	{

	}

	public virtual void JumpAttack()
	{
		
	}

	public virtual void LethalMove()
	{

	}
}
