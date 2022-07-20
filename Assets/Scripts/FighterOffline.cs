using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class FighterOffline : PrototypeOffline
{
	protected IAction.Direction direction = IAction.Direction.None;
	protected IAction.Skill attackType = IAction.Skill.None;
	public bool guard;

	protected override void Update()
	{
		base.Update();

		direction = IAction.Direction.None;
		attackType = IAction.Skill.None;

		//if (!Input.anyKeyDown) return;

		if (Input.GetKeyDown(playerKeyArray[1]))
		{
			direction = PhotonNetwork.IsMasterClient ? IAction.Direction.Left : IAction.Direction.Right;
		}
		else if (Input.GetKey(playerKeyArray[2]))
		{
			direction = IAction.Direction.Guard;
			m_procession = false;
		}
		else if (Input.GetKeyDown(playerKeyArray[3]))
		{
			direction = PhotonNetwork.IsMasterClient ? IAction.Direction.Right : IAction.Direction.Left;
		}
		else if (Input.GetKeyDown(playerKeyArray[0]))
		{
			direction = IAction.Direction.Up;
		}

		if (Input.GetKeyDown(playerKeyArray[4]))
		{
			attackType = IAction.Skill.Attack;

			if (!m_grounded)
			{
				attackType = IAction.Skill.JumpAttack;
			}
		}
		else if (Input.GetKeyDown(playerKeyArray[5]))
		{
			attackType = IAction.Skill.ChargedAttack;

			if (!m_grounded)
			{
				attackType = IAction.Skill.JumpAttack;
			}
		}
		else if (Input.GetKeyDown(playerKeyArray[6]))
		{
			attackType = IAction.Skill.LethalMove;
		}
	}
}
