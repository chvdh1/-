using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class N_Player : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform mesh;
    public PhotonView PV;
    public Text txt;


    private void Update()
    {
        if(PV.IsMine)
            Move();
    }

    private void Move()
    {
        float asix = Input.GetAxisRaw("Horizontal");
        PV.RPC("FlipXRPC",RpcTarget.AllBuffered, asix);
    }

    [PunRPC]
    void FlipXRPC(float axis)
    {
        mesh.localScale = new Vector3(axis, 1, 1);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(txt.text); //SendNext�� ������Ʈ�� �ޱ� ������ ��� �����̵� �� �޴´�.
        else
            txt.text = (string)stream.ReceiveNext();
        // throw new System.NotImplementedException();
    }
}
