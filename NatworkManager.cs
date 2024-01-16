using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class NatworkManager : MonoBehaviourPunCallbacks //Photon사용할 때.
{
    public Text statusText;
    public InputField roomInput, nickNameInput;

    private void Update()
    {
        statusText.text = PhotonNetwork.NetworkClientState.ToString(); //현재 상태(접속,접속 중, 실패 등등) 알림
    }

    #region 방리스트 갱신
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;
    List<RoomInfo> myList = new List<RoomInfo>();
    int currentPage = 1, maxPage, multiple;
    // ◀버튼 -2 , ▶버튼 -1 , 셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        // 최대페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
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

    #region 서버연결
    public void Connet() => PhotonNetwork.ConnectUsingSettings(); //서버 접속 함수

    public override void OnConnectedToMaster() //PhotonNetwork.ConnectUsingSettings() 호출 성공 후 자동 실행 함수
    {
        Debug.Log("서버 접속 완료");
        PhotonNetwork.LocalPlayer.NickName = nickNameInput.text;
        // base.OnConnectedToMaster();
    }

    public void Disconnet() => PhotonNetwork.Disconnect(); // 연결 끊기
    public override void OnDisconnected(DisconnectCause cause)//PhotonNetwork.Disconnect() 후 자동 실행 함수
    {
        Debug.Log("서버 끊김");
        //base.OnDisconnected(cause);
    }

    public void JoinLobby() => PhotonNetwork.JoinLobby();//로비 접속, 대형 게임이 아니면 하나만?
    public override void OnJoinedLobby() //  PhotonNetwork.JoinLobby() 성공 후 자동 실행 함수
    {
        Debug.Log("로비 접속 완료");
        myList.Clear();
        //base.OnJoinedLobby();
    }
    #endregion
    //위 함수가 모두 성공하면 아래 함수 실행 가능--------------------------------------------------------

    #region 방 연결
    public void CreateRoom() => PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions { MaxPlayers = 2 });
    //최대 인원 2명의 roomInput.text란 이름의 방 생성
    //PhotonNetwork.CreateRoom 성공 후 자동 실행되는 함수 OnCreatedRoom(), OnJoinedRoom()
    public override void OnCreatedRoom() => Debug.Log("방 만들기 완료");
    public override void OnJoinedRoom() => Debug.Log("방 참가 완료");
    //PhotonNetwork.CreateRoom 실패 후 자동 실행되는 함수 OnCreateRoomFailed
    //방 이름이 겹치는 경우
    public override void OnCreateRoomFailed(short returnCode, string message) => Debug.Log("방 만들기 실패");
    //OnJoinedRoom()함수가 실패시 자동 실행
    public override void OnJoinRoomFailed(short returnCode, string message) => Debug.Log("방 참가 실패");
    public void JoinRandomRoom() => PhotonNetwork.JoinRandomRoom(); //랜덤한 방에 입장, 매칭 시스템에 자주 사용
    //PhotonNetwork.JoinRandomRoom() 성공하면  OnJoinedRoom(), 실패하면  OnJoinRoomFailed 자동 실행

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();//방 떠나기

    public override void OnPlayerEnteredRoom(Player newPlayer)//플레이어가 방에 입장되면 자동 호출 함수
    {

       // base.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)//플레이어가 방에서 떠날때 자동 호출 함수(연결끊김, 나가기)
    {
        //base.OnPlayerLeftRoom(otherPlayer);
    }
    #endregion

    #region 채팅
    public PhotonView PV;
    public InputField ChatInput;
    public Text[] ChatText;
    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }
    [PunRPC]//플레이어가 속해있는 방 모든 인원에게 전달하는 함수
    //PunRPC를 호출하기 위해서는
    //PV.RPC("ChatRPC"(함수 이름), RpcTarget.All(대상), (매개 변수)PhotonNetwork.NickName + " : " + ChatInput.text);

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
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }

    #endregion

    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            print("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            print("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            print("현재 방 최대인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playersNic = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playersNic += PhotonNetwork.PlayerList[i].NickName + ", ";
            }
            print(playersNic);
        }
        else
        {
            print("접속한 인원 수 : " + PhotonNetwork.CountOfPlayers);
            print("방 개수 : " + PhotonNetwork.CountOfRooms);
            print("모든 방의 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            print("로비에 있는지? : " + PhotonNetwork.InLobby);
            print("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }

}
