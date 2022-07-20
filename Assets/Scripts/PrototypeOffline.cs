using Photon.Pun;
using UnityEngine;

public class PrototypeOffline : MonoBehaviour
{
	[Header("Variables")]
	[SerializeField] private float m_maxSpeed = 4.5f;
	[SerializeField] private float m_jumpForce = 7.5f;

	[Header("Effects")]
	[SerializeField] private GameObject m_RunStopDust;
	[SerializeField] private GameObject m_JumpDust;
	[SerializeField] private GameObject m_LandingDust;

	protected Animator m_animator;
	protected Rigidbody2D m_rigidbody2d;
	protected SpriteRenderer m_spriteRenderer;
	private Sensor m_groundSensor;
	private FighterAudio m_fighterAudio;
	protected bool m_grounded = false;
	public bool m_procession = false;
	public bool m_canJump = true;
	protected bool m_movePath = false;
	protected bool m_jump = false;
	public int m_facingDirection = 0;
	protected bool attacking1 = false;
	protected bool attacking2 = false;
	protected bool attacking3 = false;
	protected bool attacking4 = false;
	[SerializeField] protected bool isPlayer1 = true;
	protected KeyCode[] playerKeyArray = new KeyCode[7];

	protected virtual void Start()
	{
		m_animator = GetComponent<Animator>();
		m_rigidbody2d = GetComponent<Rigidbody2D>();
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_fighterAudio = transform.Find("Audio").GetComponent<FighterAudio>();
		m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor>();

		if (isPlayer1)
		{
			playerKeyArray[0] = KeyCode.W;
			playerKeyArray[1] = KeyCode.A;
			playerKeyArray[2] = KeyCode.S;
			playerKeyArray[3] = KeyCode.D;
			playerKeyArray[4] = KeyCode.J;
			playerKeyArray[5] = KeyCode.K;
			playerKeyArray[6] = KeyCode.L;
		}
		else
		{
			playerKeyArray[0] = KeyCode.UpArrow;
			playerKeyArray[1] = KeyCode.LeftArrow;
			playerKeyArray[2] = KeyCode.DownArrow;
			playerKeyArray[3] = KeyCode.RightArrow;
			playerKeyArray[4] = KeyCode.Alpha1;
			playerKeyArray[5] = KeyCode.Alpha2;
			playerKeyArray[6] = KeyCode.Alpha3;
		}
	}

	protected virtual void Update()
	{
		OnGround();

		IsFalling();

		Jump_And_AnimationState();

		Movement();

		SetAirspeed();
	}

	private void OnGround()
	{
		if (!m_grounded && m_groundSensor.State())
		{
			m_grounded = true;
			m_jump = false;
			m_animator.SetBool("Grounded", m_grounded);
			m_animator.SetBool("Jump", m_jump);
		}
	}

	private void IsFalling()
	{
		//Check if character just started falling
		if (m_grounded && !m_groundSensor.State())
		{
			m_grounded = false;
			m_jump = false;
			m_animator.SetBool("Grounded", m_grounded);
			m_animator.SetBool("Jump", m_jump);
		}
	}

	private void Movement()
	{
		if (Input.GetKey(playerKeyArray[3]) && !attacking1 && !attacking2 && !attacking3 && !attacking4)
		{
			m_procession = true;
			m_facingDirection = 1;
			m_spriteRenderer.flipX = false;
		}
		else if (Input.GetKey(playerKeyArray[1]) && !attacking1 && !attacking2 && !attacking3 && !attacking4)
		{
			m_procession = true;
			m_facingDirection = -1;
			m_spriteRenderer.flipX = true;
		}
		else
		{
			m_procession = false;
			m_facingDirection = 0;
		}

		m_rigidbody2d.velocity = new Vector2(m_facingDirection * m_maxSpeed, m_rigidbody2d.velocity.y);
	}

	private void SetAirspeed()
	{
		m_animator.SetFloat("AirSpeedY", m_rigidbody2d.velocity.y);
	}

	private void Jump_And_AnimationState()
	{
		if (Input.GetKeyDown(playerKeyArray[0]) && m_grounded && !attacking1 && !attacking2 && !attacking3 && !attacking4 && m_canJump)
		{
			m_grounded = false;
			m_jump = true;
			m_animator.SetBool("Jump", m_jump);
			m_animator.SetBool("Grounded", m_grounded);
			m_rigidbody2d.velocity = new Vector2(m_rigidbody2d.velocity.x, m_jumpForce);
			m_groundSensor.Disable(0.2f);
		}
		else if (m_procession)
		{
			m_animator.SetInteger("AnimState", 1);
		}
		else
		{
			m_animator.SetInteger("AnimState", 0);
		}
	}

	private void SpawnDustEffect(GameObject dust, float dustXOffset = 0)
	{
		if (dust != null)
		{
			Vector3 dustSpawnPosition = transform.position + new Vector3(dustXOffset * m_facingDirection, 0.0f, 0.0f);
			GameObject newDust = Instantiate(dust, dustSpawnPosition, Quaternion.identity);
			newDust.transform.localScale = newDust.transform.localScale.x * new Vector3(m_facingDirection, 1, 1);
		}
	}

	void AE_runStop()
	{
		m_fighterAudio.PlaySound("RunStop");
		float dustXOffset = 0.6f;
		SpawnDustEffect(m_RunStopDust, dustXOffset);
	}

	void AE_footstep()
	{
		m_fighterAudio.PlaySound("Footstep");
	}

	void AE_Jump()
	{
		m_fighterAudio.PlaySound("Jump");
		SpawnDustEffect(m_JumpDust);
	}

	void AE_Landing()
	{
		m_fighterAudio.PlaySound("Landing");
		SpawnDustEffect(m_LandingDust);
	}
}
