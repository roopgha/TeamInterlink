using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeeroOffline : FighterOffline, IAction
{
	[SerializeField] private Image HPBar, FPBar;

	public float HP = 100, FP = 0;

	float chargedAttack = -1f;
	float jumpAttack = -1.5f;

	public int hit = -1;
	bool lhit = false;

	[SerializeField] private GameObject[] motionColliders = new GameObject[4];

	private Coroutine[] coroutines = new Coroutine[4];

	//private int before = -1;

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		HPBar.fillAmount = HP / 100f;
		FPBar.fillAmount = FP / 100f;

		base.Update();

		if (direction == IAction.Direction.Guard)
		{
			guard = true;
			m_animator.SetBool("Guard", guard);
		}
		else
		{
			guard = false;
			m_animator.SetBool("Guard", guard);
		}

		if (hit >= 0)
		{
			if (!guard)
			{
				switch (hit)
				{
					case 0:
						HP -= 2;
						FP += 7;
						break;
					case 1:
						HP -= 4;
						FP += 9;
						break;
					case 2:
						HP -= 6;
						FP += 13;
						break;
					case 3:
						HP -= 24;
						break;
					default:
						break;
				}
				m_animator.SetBool("Hit", true);
				StartCoroutine(hitMotion());
				if (HP < 0)
				{
					StartCoroutine(Death());
				}
				if (FP > 100)
				{
					FP = 100;
				}
				hit = -1;
			}
			else
			{
				switch (hit)
				{
					case 0:
						HP -= 0.5f;
						FP += 5;
						break;
					case 1:
						HP -= 1;
						FP += 6;
						break;
					case 2:
						HP -= 2;
						FP += 8;
						break;
					case 3:
						HP -= 10;
						break;
					default:
						break;
				}
				if (HP < 0) HP = 0;
				if (HP < 0)
				{
					StartCoroutine(Death());
				}
				if (FP > 100)
				{
					FP = 100;
				}
				hit = -1;
			}
		}

		chargedAttack += Time.deltaTime;

		jumpAttack += Time.deltaTime;

		if (guard) return;

		switch (attackType)
		{
			case IAction.Skill.Attack:
				Attack();
				break;

			case IAction.Skill.ChargedAttack:
				ChargedAttack();
				break;

			case IAction.Skill.JumpAttack:
				JumpAttack();
				break;

			case IAction.Skill.LethalMove:
				LethalMove();
				break;
		}
	}

	IEnumerator Death()
	{
		m_animator.SetBool("Death", true);
		float countTime = 0f;
		while (countTime <= 0.833f)
		{
			countTime += Time.deltaTime;
			m_procession = false;
			m_canJump = false;
			yield return null;
		}
		//yield return new WaitForSeconds(0.833f);
		gameObject.SetActive(false);
	}

	IEnumerator hitMotion()
	{
		yield return new WaitForSeconds(0.25f);
		m_animator.SetBool("Hit", false);
	}

	public void Attack()
	{
		if (attacking4) return;
		if (attacking3) return;
		if (attacking2) return;
		if (attacking1) return;
		if (transform.position.y > 1f) return;
		coroutines[0] = StartCoroutine(CAttack());
	}
	IEnumerator CAttack()
	{
		//before = 0;
		StopC(0);
		attacking1 = true;
		m_animator.SetBool("Attack", true);
		m_animator.SetBool("ChargedAttack", false);
		yield return new WaitForSeconds(0.2f);
		OnCollider(0);
		yield return new WaitForSeconds(0.2f);
		OnCollider(4);
		yield return new WaitForSeconds(0.017f);
		while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
		{
			yield return null;
		}
		m_animator.SetBool("Attack", false);
		m_animator.SetBool("ChargedAttack", false);
		attacking1 = false;
	}

	public void ChargedAttack()
	{
		if (attacking4) return;
		if (attacking3) return;
		if (attacking2) return;
		if (attacking1) return;
		if (chargedAttack < 0) return;
		if (transform.position.y > 1f) return;

		chargedAttack = -1f;
		coroutines[1] = StartCoroutine(CChargedAttack());
	}
	IEnumerator CChargedAttack()
	{
		//before = 1;
		StopC(1);
		attacking2 = true;
		m_animator.SetBool("ChargedAttack", true);
		m_animator.SetBool("Attack", false);
		yield return new WaitForSeconds(0.3f);
		OnCollider(1);
		yield return new WaitForSeconds(0.2f);
		OnCollider(4);
		yield return new WaitForSeconds(0.183f);
		m_animator.SetBool("ChargedAttack", false);
		m_animator.SetBool("Attack", false);
		attacking2 = false;
	}

	public void JumpAttack()
	{
		if (attacking4) return;
		if (attacking3) return;
		if (jumpAttack < 0) return;
		if (m_animator.GetFloat("AirSpeedY") <= 0) return;
		jumpAttack = -1.5f;
		coroutines[2] = StartCoroutine(CJumpAttack());
	}
	IEnumerator CJumpAttack()
	{
		//before = 2;
		StopC(2);
		attacking3 = true;
		m_animator.SetBool("JumpAttackStay", true);
		m_rigidbody2d.velocity += new Vector2(0, 3);
		while (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
		{
			yield return null;
		}
		while (!(Input.GetKeyUp(playerKeyArray[4]) || Input.GetKeyUp(playerKeyArray[5])))
		{
			m_animator.StopPlayback();
			if (m_animator.GetFloat("AirSpeedY") < 0) break;
			yield return null;
		}
		m_animator.SetBool("JumpAttackStay", false);
		yield return new WaitForSeconds(0.3f);
		m_animator.SetBool("JumpAttack", true);
		OnCollider(2);
		m_rigidbody2d.velocity += new Vector2(0, -100);
		yield return new WaitForSeconds(0.2f);
		m_animator.SetBool("JumpAttack", false);
		OnCollider(4);
		attacking3 = false;
	}

	public void LethalMove()
	{
		if (attacking4) return;
		if (attacking3) return;
		if (attacking2) return;
		if (attacking1) return;
		if (jumpAttack < 0) return;
		if (m_animator.GetFloat("AirSpeedY") <= 0 && !m_grounded) return;
		if (FP < 100) return;
		coroutines[3] = StartCoroutine(CLethalMove());
	}
	IEnumerator CLethalMove()
	{
		FP = 0;
		//before = 3;
		StopC(3);
		attacking4 = true;
		m_animator.SetBool("Attack", false);
		m_animator.SetBool("ChargedAttack", false);
		m_animator.SetBool("LethalMoveFirst", true);
		m_rigidbody2d.velocity += new Vector2(0, 3);
		yield return new WaitForSeconds(0.1f);
		OnCollider(3);
		yield return new WaitForSeconds(0.15f);
		OnCollider(4);
		yield return new WaitForSeconds(0.083f);
		if (lhit)
		{
			m_animator.SetBool("LethalMove", true);
			OnCollider(3);
			yield return new WaitForSeconds(0.3f);
			OnCollider(4);
			yield return new WaitForSeconds(0.117f);
			m_animator.SetBool("LethalMove", false);
			m_animator.SetBool("LethalMoveFirst", false);
		}
		else
		{
			m_animator.SetBool("LethalMoveMiss", true);
			yield return new WaitForSeconds(0.2f);
			m_animator.SetBool("LethalMoveMiss", false);
			m_animator.SetBool("LethalMoveFirst", false);
		}
		//m_grounded = true;
		lhit = false;
		yield return new WaitForSeconds(0.1f);
		attacking4 = false;
	}

	private void OnCollider(int index)
	{
		for (int count = 0; count < motionColliders.Length; count++)
		{
			if (m_spriteRenderer.flipX)
			{
				motionColliders[count].GetComponent<BoxCollider2D>().offset = new Vector2(-motionColliders[count].GetComponent<BoxCollider2D>().offset.x, motionColliders[count].GetComponent<BoxCollider2D>().offset.y);
			}
			else
			{
				motionColliders[count].GetComponent<BoxCollider2D>().offset = new Vector2(motionColliders[count].GetComponent<BoxCollider2D>().offset.x, motionColliders[count].GetComponent<BoxCollider2D>().offset.y);
			}
			if (count == index)
			{
				motionColliders[count].SetActive(true);
			}
			else
			{
				motionColliders[count].SetActive(false);
			}
		}
	}

	private void StopC(int index)
	{
		for (int count = 0; count < coroutines.Length; count++)
		{
			if (count != index)
			{
				if (coroutines[count] == null) continue;
				StopCoroutine(coroutines[count]);
				//switch (before)
				//{
				//	case 0:
				//		attacking1 = false;
				//		break;
				//	case 1:
				//		attacking2 = false;
				//		break;
				//	case 2:
				//		attacking3 = false;
				//		break;
				//	case 3:
				//		attacking4 = false;
				//		break;
				//	default:
				//		break;
				//}
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (isPlayer1)
		{
			if (collision.CompareTag("Player2"))
			{
				if (attacking1)
				{
					collision.GetComponent<ArkShaOffline>().hit = 0;
					FP += 3;
				}
				else if (attacking2)
				{
					collision.GetComponent<ArkShaOffline>().hit = 1;
					FP += 4;
				}
				else if (attacking3)
				{
					collision.GetComponent<ArkShaOffline>().hit = 2;
					FP += 6;
				}
				else if (attacking4)
				{
					collision.GetComponent<ArkShaOffline>().hit = 3;
					if (!lhit)
					{
						lhit = true;
					}
				}
			}
		}
		else
		{
			if (collision.CompareTag("Player1"))
			{
				if (attacking1)
				{
					collision.GetComponent<ArkShaOffline>().hit = 0;
					FP += 3;
				}
				else if (attacking2)
				{
					collision.GetComponent<ArkShaOffline>().hit = 1;
					FP += 4;
				}
				else if (attacking3)
				{
					collision.GetComponent<ArkShaOffline>().hit = 2;
					FP += 6;
				}
				else if (attacking4)
				{
					collision.GetComponent<ArkShaOffline>().hit = 3;
					if (!collision.GetComponent<ArkShaOffline>().guard)
					{
						if (!lhit)
						{
							lhit = true;
						}
					}
				}
			}
		}
	}
}
