using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Json데이터 저장 및 로드, 프레임 60 고정
public class JData
{
    public int coin;
    public int carrotNum;
    public int skillBtnposint;
    public float allSlider;
    public float bgmSlider;
    public float sfxSlider;
    public int highScore;
    public bool carrotUnLack0;
    public bool carrotUnLack1;
    public bool carrotUnLack2;
    public bool carrotUnLack3;
    public bool carrotUnLack4;

    public int oneItems0;
    public int oneItems1;
    public int oneItems2;

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
    }
}
public class UniqueInfo : MonoBehaviour
{
    public static UniqueInfo Unique;

    public Audio ad;
    public int coin;
    public int carrotNum;
    public int skillBtnposint;
    public float allSlider;
    public float bgmSlider;
    public float sfxSlider;
    public int highScore;

    public bool[] carrotUnLack;
    public int[] oneItems = new int[3];

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

        carrotUnLack[0] = true;

        ad = transform.GetChild(0).GetComponent<Audio>();
        ad.uniq = this;
        for (int i = 0; i < oneItems.Length; i++)
            oneItems[i] = -1;

        string fileName = Path.Combine(Application.persistentDataPath + "/Data.json");
        if (!File.Exists(fileName))
            SaveToJson();
        else
            LoadFromJson();
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
        }
    }
}

