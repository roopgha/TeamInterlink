using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleAdmin : MonoBehaviourPunCallbacks
{
	private const byte LIMIT = 2;

	[SerializeField] private GameObject[] titleObjects;
	//[SerializeField] private Text text;

	private Coroutine coroutine;
	private string originRoomCode = null;
	private string roomCode = null;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void ConnectSetting()
	{
		Application.targetFrameRate = 60;
		PhotonNetwork.AutomaticallySyncScene = true;
		//PhotonNetwork.JoinLobby();
	}

	private IEnumerator ConnectToMasterServer()
	{
		while (PhotonNetwork.NetworkClientState != ClientState.ConnectedToMasterServer)
		{
			yield return null;
		}
		SetObjectActive(titleObjects, true); // �¶��� ��ư �����ϱ�!!
		SetObjectActive(new string[] { "CancelCreateRoom", "Connecting", "ConnectFailed", "LoadingText" }, false);
	}

	private void Start()
    {
		PhotonNetwork.ConnectUsingSettings();
		SetObjectActive(titleObjects, false);
		SetObjectActive(new string[] { "LoadingText" }, true);
		StartCoroutine(ConnectToMasterServer());
	}

	private void Update()
	{
		if (CheckInternet.InternetConnected) return;

		if (PhotonNetwork.NetworkClientState == ClientState.Joined)
		{
			PhotonNetwork.LeaveRoom();
		}
		// ���ͳ� ���� ���ο� ���� UI ���� Ű��
		SetObjectActive(new string[] { "JoinRoom", "InputRoomCode", "CreateRoom" }, false);
		SetObjectActive(new string[] { "LoadingText" }, true);
	}

	public void StartOffline()
	{
		CheckInternet.gamePlayMode = false;
		SceneManager.LoadScene("Fight");
	}

	public void CreateRoom()
	{
		int number1 = System.DateTime.Today.Year + System.DateTime.Today.Month + System.DateTime.Today.Day;
		int number2 = System.DateTime.Now.Hour + System.DateTime.Now.Minute + System.DateTime.Now.Second;
		originRoomCode = (number1 * number2 * Random.Range(0, int.MaxValue)).ToString("X");

		// �� �ɼ� �����ϱ�.
		RoomOptions roomOptions = new RoomOptions
		{
			MaxPlayers = LIMIT,
			IsVisible = true,
			CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "RoomCode", originRoomCode } },
			CustomRoomPropertiesForLobby = new string[] { "RoomCode" } // ���⿡ Ű ���� ����ؾ�, ������ �� ���͸��� �����ϴ�.
		};

		PhotonNetwork.CreateRoom("Room_" + roomCode, roomOptions);

		coroutine = StartCoroutine(WatingAnotherPlayer());

		FindObject("StatusText").GetComponent<TextMeshProUGUI>().text = "�� �ڵ� : " + originRoomCode;

		SetObjectActive(new string[] { "JoinRoom", "InputRoomCode", "CreateRoom" }, false);
		StartCoroutine(DelayActiveByClientState(new string[] { "CancelCreateRoom" }, true, ClientState.Joined));
		CheckInternet.gamePlayMode = true;
	}

	public void CancelCreateRoom()
	{
		StopCoroutine(coroutine);
		PhotonNetwork.LeaveRoom();
		SetObjectActive(new string[] { "CancelCreateRoom" }, false);
		StartCoroutine(DelayActiveByClientState(new string[] { "JoinRoom", "InputRoomCode", "CreateRoom" }, true, ClientState.ConnectedToMasterServer));
		StartCoroutine(ClearText(FindObject("StatusText").GetComponent<TextMeshProUGUI>(), 0));
		CheckInternet.gamePlayMode = false;
	}

	//���� �Ǹ� ȣ��
	//public override void OnConnectedToMaster()
	//{
	//	SetObjectActive(titleObjects, true);
	//	SetObjectActive(new string[] { "CancelCreateRoom", "Connecting", "ConnectFailed", "LoadingText" }, false);
	//}

	//���� ����
	//public void Disconnect() => PhotonNetwork.Disconnect();

	////���� ������ �� ȣ��
	//public override void OnDisconnected(DisconnectCause cause)
	//{
	//	SetObjectActive(new string[] { "CancelCreateRoom" }, false);
	//	StartCoroutine(ClearText(FindObject("StatusText").GetComponent<Text>(), 0));
	//	CheckInternet.gamePlayMode = false;
	//}

	//�� ����
	public void JoinRoom()
	{
		if (string.IsNullOrEmpty(roomCode))
		{
			TextMeshProUGUI text = FindObject("StatusText").GetComponent<TextMeshProUGUI>();
			text.text = "�� �ڵ带 �Է����ּ���";
			StartCoroutine(ClearText(text, 1.5f));
			return;
		}

		ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable() { { "RoomCode", roomCode } };

		CheckInternet.gamePlayMode = true;
		PhotonNetwork.JoinRandomRoom(customProperties, 2);

		StartCoroutine(WatingConnectRoom());
	}

	private IEnumerator WatingConnectRoom()
	{
		SetObjectActive(new string[] { "Connecting" }, true);

		float countTime = 0f;
		while (countTime <= 5)
		{
			countTime += Time.deltaTime;
			yield return null;
		}

		SetObjectActive(new string[] { "Connecting" }, false);

		SetObjectActive(new string[] { "ConnectFailed" }, true);

		countTime = 0f;
		while (countTime <= 1.5f)
		{
			countTime += Time.deltaTime;
			yield return null;
		}

		SetObjectActive(new string[] { "ConnectFailed" }, false);
		CheckInternet.gamePlayMode = false;
	}
	
	//�濡 ���� ���� �� ȣ�� 
	public override void OnJoinedRoom()
	{
		if (!PhotonNetwork.IsMasterClient)
		{
			SceneManager.LoadScene("Fight");
		}
	}

	private IEnumerator WatingAnotherPlayer()
	{
		while (PhotonNetwork.CurrentRoom == null)
		{
			yield return null;
		}
		while (!PhotonNetwork.CurrentRoom.PlayerCount.Equals(PhotonNetwork.CurrentRoom.MaxPlayers))
		{
			if (!CheckInternet.InternetConnected)
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
			yield return null;
		}

		CheckInternet.gamePlayMode = true;
		SceneManager.LoadScene("Fight");
	}

	public void SetRoomCode(TextMeshProUGUI text)
	{
		roomCode = text.text;
	}



	private IEnumerator ClearText(TextMeshProUGUI text, float time)
	{
		float countTime = 0f;
		while (countTime <= time)
		{
			countTime += Time.deltaTime;
			yield return null;
		}
		text.text = null;
	}

	private IEnumerator DelayActiveByClientState(string[] objects, bool setValue, ClientState clientState)
	{
		while (true)
		{
			if (PhotonNetwork.NetworkClientState == clientState) break;
			yield return null;
		}
		SetObjectActive(objects, setValue);
	}

	private void SetObjectActive(string[] objects, bool setValue)
	{
		for (int count = 0; count < titleObjects.Length; count++)
		{
			for (int count1 = 0; count1 < objects.Length; count1++)
			{
				if (titleObjects[count].name.Equals(objects[count1]))
				{
					titleObjects[count].SetActive(setValue);
				}
			}
		}
	}
	private void SetObjectActive(GameObject[] objects, bool setValue)
	{
		for (int count = 0; count < titleObjects.Length; count++)
		{
			for (int count1 = 0; count1 < objects.Length; count1++)
			{
				if (titleObjects[count].name.Equals(objects[count1].name))
				{
					titleObjects[count].SetActive(setValue);
				}
			}
		}
	}
	private GameObject FindObject(string name)
	{
		for (int count = 0; count < titleObjects.Length; count++)
		{
			if (titleObjects[count].name.Equals(name))
			{
				return titleObjects[count];
			}
		}

		return null;
	}
}
