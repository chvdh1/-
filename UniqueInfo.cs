using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

using Firebase;
using Firebase.Database;
using System;

public class JData
{
    public string nicname;

    public int coin;
    public int carrotNum;
    public int skillBtnposint;
    public float allSlider;
    public float bgmSlider;
    public float sfxSlider;
    public int highScore;
    public int rank;
    public bool carrotUnLack0;
    public bool carrotUnLack1;
    public bool carrotUnLack2;
    public bool carrotUnLack3;
    public bool carrotUnLack4;

    public int oneItems0;
    public int oneItems1;
    public int oneItems2;

    public int item0;
    public int item1;
    public int item2;
    public int item3;
    public int item4;
    public int item5;

    public JData(UniqueInfo uniq)
    {
       
        coin = uniq.coin;
        carrotNum = uniq.carrotNum;
        skillBtnposint = uniq.skillBtnposint;
        allSlider = uniq.allSlider;
        bgmSlider = uniq.bgmSlider;
        sfxSlider = uniq.sfxSlider;
        highScore = uniq.highScore;
        carrotUnLack0 = uniq.carrotUnLack[0];
        carrotUnLack1 = uniq.carrotUnLack[1];
        carrotUnLack2 = uniq.carrotUnLack[2];
        carrotUnLack3 = uniq.carrotUnLack[3];
        carrotUnLack4 = uniq.carrotUnLack[4];
        oneItems0 = uniq.oneItems[0];
        oneItems1 = uniq.oneItems[1];
        oneItems2 = uniq.oneItems[2];
        rank = uniq.rank;

        nicname = uniq.nicname;
        item0 = uniq.item[0];
        item1 = uniq.item[1];
        item2 = uniq.item[2];
        item3 = uniq.item[3];
        item4 = uniq.item[4];
        item5 = uniq.item[5];
    }
}
public class UniqueInfo : MonoBehaviour
{
    public static UniqueInfo Unique;
    public string nicname;

    public Audio ad;
    public int coin;
    public int carrotNum;
    public int skillBtnposint;
    public float allSlider;
    public float bgmSlider;
    public float sfxSlider;
    public int highScore;
    public int rank;
    public bool[] carrotUnLack;
    public int[] oneItems = new int[3];
    public int[] item = new int[6];

    //탑5 랭킹 시스템
    public string[] top5nic = new string[5];
    public int[] top5score = new int[5];

    public int[] top1items = new int[6];
    public int[] top2items = new int[6];
    public int[] top3items = new int[6];
    public int[] top4items = new int[6];
    public int[] top5items = new int[6];

    public DatabaseReference database;
    public LobbyManager lm;
    void Awake()
    {
        if (Unique == null)
            Unique = this;

        else if (Unique != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60; //실행 프레임 속도 60프레임으로 고정 시키기.. 코드
        QualitySettings.vSyncCount = 0;
        //모니터 주사율(플레임율)이 다른 컴퓨터일 경우 캐릭터 조작시 빠르게 움직일 수 있다.


        //데이터를 쓰려면 DatabaseReference 의 인스턴스가 필요
        database = FirebaseDatabase.DefaultInstance.RootReference;

        carrotUnLack[0] = true;

        ad = transform.GetChild(0).GetComponent<Audio>();
        ad.uniq = this;
        for (int i = 0; i < oneItems.Length; i++)
            oneItems[i] = -1;

        string fileName = Path.Combine(Application.persistentDataPath + "/Data.json");
        if (!File.Exists(fileName))
        {
            lm.Nicname();
            //SaveToJson();
        }
        else
        {
            LoadFromJson();
            lm.lobbyNicname.text = nicname;
        }
          
    }
    

    public void SaveToJson()
    {
        string fileName = Path.Combine(Application.persistentDataPath + "/Data.json");
        Debug.Log("SaveToJson : "+fileName);

        //if (File.Exists(fileName))
        //    File.Delete(fileName);

        JData data = new JData(Unique);
        string json = JsonUtility.ToJson(data);

        // 완성된 json string 문자열을 8비트 부호없는 정수로 변환
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // 변환된 바이트배열을 base-64 인코딩된 문자열로 변환
        string encodedJson = System.Convert.ToBase64String(bytes);

        // 변환된 값을 저장
        File.WriteAllText(fileName, encodedJson);

        //DatabaseReference 변수에 userId를 자식으로 변환된 JSON 파일을 업로드 => 서버에 저장
        database.Child("User Information").Child(nicname).SetRawJsonValueAsync(json);
    }
    public void LoadFromJson()
    {
        string fileName = Path.Combine(Application.persistentDataPath + "/Data.json");
        Debug.Log("LoadFromJson : "+fileName);
        if (File.Exists(fileName))
        {
            // json으로 저장된 문자열을 로드한다.
            string jsonFromFile = File.ReadAllText(fileName);

            // 읽어온 base-64 인코딩 문자열을 바이트배열로 변환
            byte[] bytes = System.Convert.FromBase64String(jsonFromFile);

            // 8비트 부호없는 정수를 json 문자열로 변환
            string decodedJson = System.Text.Encoding.UTF8.GetString(bytes);

            JData data = JsonUtility.FromJson<JData>(decodedJson);

            coin = data.coin;
            carrotNum = data.carrotNum;
            skillBtnposint = data.skillBtnposint;
            carrotUnLack[0] = data.carrotUnLack0;
            carrotUnLack[1] = data.carrotUnLack1;
            carrotUnLack[2] = data.carrotUnLack2;
            carrotUnLack[3] = data.carrotUnLack3;
            carrotUnLack[4] = data.carrotUnLack4;

            oneItems[0] = data.oneItems0;
            oneItems[1] = data.oneItems1;
            oneItems[2] = data.oneItems2;

            allSlider = data.allSlider;
            bgmSlider = data.bgmSlider;
            sfxSlider = data.sfxSlider;

            highScore = data.highScore;

            nicname = data.nicname;
            rank = data.rank;
            item[0] = data.item0;
            item[1] = data.item1;
            item[2] = data.item2;
            item[3] = data.item3;
            item[4] = data.item4;
            item[5] = data.item5;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            Top5RankCK();
    }


    public void DatabaseLoad(string nicname1)
    {
        //database의 자식(userId)를 task로 받음
        database.Child("User Information").Child(nicname1).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.Log("error");
            //task가 성공적이면
            else if (task.IsCompleted)
            {
                //DataSnapshot 변수를 선언하여 task의 결과 값을 받음
                DataSnapshot snapshot = task.Result;
             
                //snapshot의 자식 개수를 확인
                Debug.Log(snapshot.ChildrenCount);

                //snapshot의 자식의 정보를 가져와 대입
                coin = int.Parse(snapshot.Child("coin").Value.ToString()); //문자형울 int형으로 변환
               

                carrotNum = int.Parse(snapshot.Child("carrotNum").Value.ToString());
                skillBtnposint = int.Parse(snapshot.Child("skillBtnposint").Value.ToString());
                carrotUnLack[1] = snapshot.Child("carrotUnLack1").Value.ToString() == "True" ? true : false;
                carrotUnLack[2] = snapshot.Child("carrotUnLack2").Value.ToString() == "True" ? true : false; 
                carrotUnLack[3] = snapshot.Child("carrotUnLack3").Value.ToString() == "True" ? true : false; 
                carrotUnLack[4] = snapshot.Child("carrotUnLack4").Value.ToString() == "True" ? true : false;

                oneItems[0] = int.Parse(snapshot.Child("oneItems0").Value.ToString());
                oneItems[1] = int.Parse(snapshot.Child("oneItems1").Value.ToString());
                oneItems[2] = int.Parse(snapshot.Child("oneItems2").Value.ToString());

                allSlider = float.Parse(snapshot.Child("allSlider").Value.ToString());
                bgmSlider = float.Parse(snapshot.Child("bgmSlider").Value.ToString());
                sfxSlider = float.Parse(snapshot.Child("sfxSlider").Value.ToString());

                nicname = snapshot.Child("nicname").Value.ToString();
                highScore = int.Parse(snapshot.Child("highScore").Value.ToString());

                rank = int.Parse(snapshot.Child("rank").Value.ToString());
                item[0] = int.Parse(snapshot.Child("item0").Value.ToString());
                item[1] = int.Parse(snapshot.Child("item1").Value.ToString());
                item[2] = int.Parse(snapshot.Child("item2").Value.ToString());
                item[3] = int.Parse(snapshot.Child("item3").Value.ToString());
                item[4] = int.Parse(snapshot.Child("item4").Value.ToString());
                item[5] = int.Parse(snapshot.Child("item5").Value.ToString());

                Debug.Log("로드 성공");
            }
        });

       

    }

    public void loadNicCK(string nic)
    {
        database.Child("User Information").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.Log("error");
            //task가 성공적이면
            else if (task.IsCompleted)
            {
                //DataSnapshot 변수를 선언하여 task의 결과 값을 받음
                DataSnapshot snapshot = task.Result;

                //snapshot의 자식 개수를 확인
                Debug.Log(snapshot.ChildrenCount);

                Debug.Log(snapshot.HasChild(nic));
                if (snapshot.HasChild(nic))
                    DatabaseLoad(nic);
            }
        });

        
    }

    public void CrateNicCK(string nic)
    {
        database.Child("User Information").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.Log("error");
            //task가 성공적이면
            else if (task.IsCompleted)
            {
                //DataSnapshot 변수를 선언하여 task의 결과 값을 받음
                DataSnapshot snapshot = task.Result;

                //snapshot의 자식 개수를 확인
                Debug.Log(snapshot.ChildrenCount);

                Debug.Log(snapshot.HasChild(nic));
                if (!snapshot.HasChild(nic))
                    nicname = nic;
            }
        });
    }


    public void Top5RankCK()
    {
        FirebaseDatabase.DefaultInstance.GetReference("User Information").OrderByChild("highScore").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
                Debug.Log("error");
            //task가 성공적이면
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                int drank = (int)snapshot.ChildrenCount;
                foreach (DataSnapshot data in snapshot.Children)
                {
                   
                    if(data.Child("nicname").Value.ToString() == nicname)
                    {
                        rank = drank;
                        Debug.Log(data.Child("nicname").Value + " : " + data.Child("highScore").Value+ " rank : "+ drank);
                    }
                    if(drank < 6)
                    {
                        Debug.Log(data.Child("nicname").Value + " : " + data.Child("highScore").Value + " rank : " + drank);
                        top5nic[drank-1] = data.Child("nicname").Value.ToString();
                        top5score[drank-1] = int.Parse(data.Child("highScore").Value.ToString());
                        switch (drank)
                        {
                            case 1:
                                top1items[0] = int.Parse(data.Child("item0").Value.ToString());
                                top1items[1] = int.Parse(data.Child("item1").Value.ToString());
                                top1items[2] = int.Parse(data.Child("item2").Value.ToString());
                                top1items[3] = int.Parse(data.Child("item3").Value.ToString());
                                top1items[4] = int.Parse(data.Child("item4").Value.ToString());
                                top1items[5] = int.Parse(data.Child("item5").Value.ToString());
                                break;
                            case 2:
                                top2items[0] = int.Parse(data.Child("item0").Value.ToString());
                                top2items[1] = int.Parse(data.Child("item1").Value.ToString());
                                top2items[2] = int.Parse(data.Child("item2").Value.ToString());
                                top2items[3] = int.Parse(data.Child("item3").Value.ToString());
                                top2items[4] = int.Parse(data.Child("item4").Value.ToString());
                                top2items[5] = int.Parse(data.Child("item5").Value.ToString());
                                break;
                            case 3:
                                top3items[0] = int.Parse(data.Child("item0").Value.ToString());
                                top3items[1] = int.Parse(data.Child("item1").Value.ToString());
                                top3items[2] = int.Parse(data.Child("item2").Value.ToString());
                                top3items[3] = int.Parse(data.Child("item3").Value.ToString());
                                top3items[4] = int.Parse(data.Child("item4").Value.ToString());
                                top3items[5] = int.Parse(data.Child("item5").Value.ToString());
                                break;
                            case 4:
                                top4items[0] = int.Parse(data.Child("item0").Value.ToString());
                                top4items[1] = int.Parse(data.Child("item1").Value.ToString());
                                top4items[2] = int.Parse(data.Child("item2").Value.ToString());
                                top4items[3] = int.Parse(data.Child("item3").Value.ToString());
                                top4items[4] = int.Parse(data.Child("item4").Value.ToString());
                                top4items[5] = int.Parse(data.Child("item5").Value.ToString());
                                break;
                            case 5:
                                top5items[0] = int.Parse(data.Child("item0").Value.ToString());
                                top5items[1] = int.Parse(data.Child("item1").Value.ToString());
                                top5items[2] = int.Parse(data.Child("item2").Value.ToString());
                                top5items[3] = int.Parse(data.Child("item3").Value.ToString());
                                top5items[4] = int.Parse(data.Child("item4").Value.ToString());
                                top5items[5] = int.Parse(data.Child("item5").Value.ToString());
                                break;
                        }
                    }
                    drank -= 1;
                }
            }
        });
    }
}

