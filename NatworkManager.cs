using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class NatworkManager : MonoBehaviourPunCallbacks //Photon����� ��.
{
    public Text statusText;
    public InputField roomInput, nickNameInput;

    private void Update()
    {
        statusText.text = PhotonNetwork.NetworkClientState.ToString(); //���� ����(����,���� ��, ���� ���) �˸�
    }

    #region �渮��Ʈ ����
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    // ����ư -2 , ����ư -1 , �� ����
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // �ִ�������
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // ����, ������ư
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // �������� �´� ����Ʈ ����
        multiple = (currentPage - 1) * CellBtn.Length;
        for (int i = 0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.GetChild(0).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.GetChild(1).GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int roomCount = roomList.Count;
        for (int i = 0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList)
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion

    #region ��������
    public void Connet() => PhotonNetwork.ConnectUsingSettings(); //���� ���� �Լ�

    public override void OnConnectedToMaster() //PhotonNetwork.ConnectUsingSettings() ȣ�� ���� �� �ڵ� ���� �Լ�
    {
        Debug.Log("���� ���� �Ϸ�");
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
        // base.OnConnectedToMaster();
    }

    public void Disconnet() => PhotonNetwork.Disconnect(); // ���� ����
    public override void OnDisconnected(DisconnectCause cause)//PhotonNetwork.Disconnect() �� �ڵ� ���� �Լ�
    {
        Debug.Log("���� ����");
        //base.OnDisconnected(cause);
    }

    public void JoinLobby() => PhotonNetwork.JoinLobby();//�κ� ����, ���� ������ �ƴϸ� �ϳ���?
    public override void OnJoinedLobby() //  PhotonNetwork.JoinLobby() ���� �� �ڵ� ���� �Լ�
    {
        Debug.Log("�κ� ���� �Ϸ�");
        myList.Clear();
        //base.OnJoinedLobby();
    }
    #endregion
    //�� �Լ��� ��� �����ϸ� �Ʒ� �Լ� ���� ����--------------------------------------------------------

    #region �� ����
    public void CreateRoom() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });
    //�ִ� �ο� 2���� roomInput.text�� �̸��� �� ����
    //PhotonNetwork.CreateRoom ���� �� �ڵ� ����Ǵ� �Լ� OnCreatedRoom(), OnJoinedRoom()
    public override void OnCreatedRoom() => Debug.Log("�� ����� �Ϸ�");
    public override void OnJoinedRoom() => Debug.Log("�� ���� �Ϸ�");
    //PhotonNetwork.CreateRoom ���� �� �ڵ� ����Ǵ� �Լ� OnCreateRoomFailed
    //�� �̸��� ��ġ�� ���
    public override void OnCreateRoomFailed(short returnCode, string message) => Debug.Log("�� ����� ����");
    //OnJoinedRoom()�Լ��� ���н� �ڵ� ����
    public override void OnJoinRoomFailed(short returnCode, string message) => Debug.Log("�� ���� ����");
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom(); //������ �濡 ����, ��Ī �ý��ۿ� ���� ���
    //PhotonNetwork.JoinRandomRoom() �����ϸ�  OnJoinedRoom(), �����ϸ�  OnJoinRoomFailed �ڵ� ����

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();//�� ������

    public override void OnPlayerEnteredRoom(Player newPlayer)//�÷��̾ �濡 ����Ǹ� �ڵ� ȣ�� �Լ�
    {

       // base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)//�÷��̾ �濡�� ������ �ڵ� ȣ�� �Լ�(�������, ������)
    {
        //base.OnPlayerLeftRoom(otherPlayer);
    }
    #endregion

    #region ä��
    public PhotonView PV;
    public InputField ChatInput;
    public Text[] ChatText;
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }
    [PunRPC]//�÷��̾ �����ִ� �� ��� �ο����� �����ϴ� �Լ�
    //PunRPC�� ȣ���ϱ� ���ؼ���
    //PV.RPC("ChatRPC"(�Լ� �̸�), RpcTarget.All(���), (�Ű� ����)PhotonNetwork.NickName + " : " + ChatInput.text);

    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // ������ ��ĭ�� ���� �ø�
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }

    #endregion

    [ContextMenu("����")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("���� �� �̸� : " + PhotonNetwork.CurrentRoom.Name);
            print("���� �� �ο��� : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("���� �� �ִ��ο��� : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playersNic = "�濡 �ִ� �÷��̾� ��� : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playersNic += PhotonNetwork.PlayerList[i].NickName + ", ";
            }
            print(playersNic);
        }
        else
        {
            print("������ �ο� �� : " + PhotonNetwork.CountOfPlayers);
            print("�� ���� : " + PhotonNetwork.CountOfRooms);
            print("��� ���� �ִ� �ο� �� : " + PhotonNetwork.CountOfPlayersInRooms);
            print("�κ� �ִ���? : " + PhotonNetwork.InLobby);
            print("����ƴ���? : " + PhotonNetwork.IsConnected);
        }
    }

}
