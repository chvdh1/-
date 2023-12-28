using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


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

        Application.targetFrameRate = 60; //���� ������ �ӵ� 60���������� ���� ��Ű��.. �ڵ�
        QualitySettings.vSyncCount = 0;
        //����� �ֻ���(�÷�����)�� �ٸ� ��ǻ���� ��� ĳ���� ���۽� ������ ������ �� �ִ�.

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

        // �ϼ��� json string ���ڿ��� 8��Ʈ ��ȣ���� ������ ��ȯ
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);

        // ��ȯ�� ����Ʈ�迭�� base-64 ���ڵ��� ���ڿ��� ��ȯ
        string encodedJson = System.Convert.ToBase64String(bytes);

        // ��ȯ�� ���� ����
        File.WriteAllText(fileName, encodedJson);
    }
    public void LoadFromJson()
    {
        string fileName = Path.Combine(Application.persistentDataPath + "/Data.json");
        Debug.Log("LoadFromJson : "+fileName);
        if (File.Exists(fileName))
        {
            // json���� ����� ���ڿ��� �ε��Ѵ�.
            string jsonFromFile = File.ReadAllText(fileName);

            // �о�� base-64 ���ڵ� ���ڿ��� ����Ʈ�迭�� ��ȯ
            byte[] bytes = System.Convert.FromBase64String(jsonFromFile);

            // 8��Ʈ ��ȣ���� ������ json ���ڿ��� ��ȯ
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

