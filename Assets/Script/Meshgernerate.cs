using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class Meshgernerate : MonoBehaviour
{
    public InputField id;
    public InputField length;
    public InputField nameInput;
    public InputField type;
    public InputField posX;
    public InputField posY;
    public InputField C;
    public InputField Size;
    public InputField shakeTime;
    public InputField speedipa;
    public InputField beHitAni;
    public InputField audioSrc;
    public InputField idleAni;
    public GameObject beInstance;
    private Canvas canvas;
    public Toggle toggle;
    public GameObject groud;
    //    public Text groundLength;

    private String titleContents = "";
    private String showContent = "";
    private int boardLenth;
    public List<GameObject> enemyList;
    public GameObject deleteGameObj;
    private bool isShowWindow = false;
    private bool isDeleteWindow = false;
    private bool isOpenWindow = false;
    private bool isListWindows = false;
    // Start is called before the first frame update
    void Start()
    {
        deleteGameObj = null;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        enemyList = new List<GameObject>();
    }

    public void Find() {
        if (id.text.Length == 0)
        {
            titleContents = "查找id必须Id有值";
            isShowWindow = true;
            return;
        }
        String openFileName = Application.streamingAssetsPath + "/jsonData.json";
        int idValue = int.Parse(id.text);
        EnemyData enemyData = new EnemyData();

        if (!File.Exists(openFileName))
            return;
        StreamReader sr = new StreamReader(openFileName);
        if (sr == null)
            return;
        string json = sr.ReadToEnd();
        if (json.Length > 0) {
            enemyData = JsonUtility.FromJson<EnemyData>(json);
            foreach (Enemy t in enemyData.enemy) {
                if (idValue == t.id) {
                    id.text = t.id+"";
                    nameInput.text=t.name;
                    type.text=t.type+"";
                    posX.text=t.posX+"";
                    posY.text=t.posY+"";
                    C.text=t.C+"";
                    Size.text=t.Size+"";
                    shakeTime.text=t.shakeTime+"";
                    speedipa.text=t.speedipa+"";
                    beHitAni.text=t.beHitAni+"";
                    audioSrc.text=t.audioSrc+"";
                    toggle.isOn=t.isbeHit;
                    idleAni.text = t.idleAni;
                    return;    
                }
            }
        }

}

    // Update is called once per frame
    public void GenerateGroud() {
        if (length.text.Length > 0) {
            boardLenth = int.Parse(length.text);
            //  length.contentType =
            if (boardLenth > 0)
            {
                groud.SetActive(true);
                //groundLength.gameObject.SetActive(true);
                Text groundLength = groud.GetComponentInChildren<Text>();
                groundLength.text = "地图长度" + boardLenth + "米";

            }
            else
            {
                titleContents = "地形长度必须是一个合法的值";
                isShowWindow = true;
                return;
            }
        }
    }

    public void AddObj() {
        if (boardLenth ==0) {
            titleContents = "必须先创建一个地形";
            isShowWindow = true;
            return;
        }
        if (posX.text.Length > 0)
        {
            float posx = float.Parse(posX.text);
            if (posx < 0 || posx > boardLenth)
            {
                titleContents = "PosX不符合规范（大于0且小于地形的长度）";
                isShowWindow = true;
                return;
            }
            foreach (GameObject t in enemyList) {
                EnemyObj enemy = t.GetComponent<EnemyObj>();
                if (Math.Abs(posx - enemy.e.posX) < boardLenth / 30.0f) {
                    titleContents = "每个对象至少相隔地形长度的1/30";
                    isShowWindow = true;
                    return;
                }
                Debug.Log("实际相隔长度" + (posx - enemy.e.posX));
                Debug.Log("至少相隔长度"+(boardLenth / 30.0));

            }
        }
        if (nameInput.text.Length>0) {
            foreach (GameObject t in enemyList)
            {
                EnemyObj enemy = t.GetComponent<EnemyObj>();

                if (nameInput.text.Equals(enemy.name)){
                    nameInput.text = nameInput.text + "1";
                    titleContents = "对象名字不能相同";
                    isShowWindow = true;
                    return;
                }

            }
        }
        if (posY.text.Length > 0)
        {
            float posy = float.Parse(posY.text);
            if (posy > 17 || posy < -31)
            {
                titleContents = "PosY不符合规范（大于-31且小于17）";
                isShowWindow = true;
                return;
            }
        }



        if (type.text.Length > 0 && posX.text.Length > 0 && posY.text.Length > 0 && C.text.Length > 0 && Size.text.Length > 0 && shakeTime.text.Length > 0 && speedipa.text.Length > 0 && beHitAni.text.Length > 0 && audioSrc.text.Length > 0)
        {
            Enemy t = new Enemy
            {
                id = int.Parse(id.text),
                type = int.Parse(type.text),
                name = nameInput.text,
                posX = float.Parse(posX.text),
                posY = float.Parse(posY.text),
                C = Double.Parse(C.text),
                Size = float.Parse(Size.text),
                shakeTime = float.Parse(shakeTime.text),
                speedipa = float.Parse(speedipa.text),
                beHitAni = beHitAni.text,
                audioSrc = audioSrc.text,
                isbeHit = toggle.isOn,
                idleAni = idleAni.text

            };
            //530是终点
            float len=(t.posX / boardLenth) * 530;
            GameObject game =GameObject.Instantiate(beInstance, canvas.gameObject.transform);
            game.name = t.name;
            game.GetComponent<RectTransform>().anchoredPosition = new Vector2(-393 + len, -25 + t.posY);
            game.GetComponentInChildren<Text>().text = t.name;
            game.GetComponent<EnemyObj>().e = t;
            Button btn = game.GetComponent<Button>();
            enemyList.Add(game);
            btn.onClick.AddListener(() =>
            {
                if (btn != null)
                {

                    deleteGameObj = game;
                    //    enemyList.Remove(game);
                    Debug.Log(deleteGameObj.name);
                    titleContents = "选是删除对象，选否取消操作";
                      isDeleteWindow = true;
                }
            });
            Debug.Log(t.posX+"y:"+t.posY);
            // hakeTime.text.Length > 0 && speedipa.text.Length > 0 && beHitAni.text.Length > 0 && audioSrc.text.Length
           // GameObject.Instantiate(,,);

        }
        else
        {
            Debug.Log("窗口打开");
            isShowWindow = true;
            titleContents = "添加物体，你需要补充并填写数据完整";
       
        }

    }


  

    public void Generate() {
        if (enemyList.Count == 0) {
            isShowWindow = true;
            titleContents = "游戏地图必须有地形、至少一个对象(女神必须在场景布置，否则没法进入游戏）";
            return;
        }
        OnSave();


    }

    public void Load()
    {
        if (enemyList.Count > 0)
        {
            isOpenWindow = true;
            titleContents = "当前场景有还在编辑的对象，是否放弃编辑，打开场景";
            return;
        }
        OnOpen();


    }


    private void OnSave()
    {
        OpenFileName openFileName = new OpenFileName();

        openFileName.structSize = Marshal.SizeOf(openFileName);

        openFileName.filter = "json文件(*.json)\0*.json";

        openFileName.file = new string(new char[256]);

        openFileName.maxFile = openFileName.file.Length;

        openFileName.fileTitle = new string(new char[64]);

        openFileName.maxFileTitle = openFileName.fileTitle.Length;

        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径

        openFileName.title = "选择地图生成的路径";

        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

        if (LocalDialog.GetSaveFileName(openFileName))

        {

            Debug.Log(openFileName.file);

            Debug.Log(openFileName.fileTitle);

            EnemyData inputDate = new EnemyData
            {
                enemy = new Enemy[enemyList.Count]
            };
            for (int i=0;i<enemyList.Count;i++)
            {

                inputDate.enemy[i] = enemyList[i].GetComponent<EnemyObj>().e;

            }
            inputDate.Length = boardLenth;

            string json = JsonUtility.ToJson(inputDate, true);
            File.WriteAllText(openFileName.file+".json", json, Encoding.UTF8);

        }
    }

    public void OnList() {


        String openFileName = Application.streamingAssetsPath + "/jsonData.json";
     //   int idValue = int.Parse(id.text);
        EnemyData enemyData = new EnemyData();

        if (!File.Exists(openFileName))
            return;
        StreamReader sr = new StreamReader(openFileName);
        if (sr == null)
            return;
        string json = sr.ReadToEnd();
        if (json.Length > 0)
        {
            enemyData = JsonUtility.FromJson<EnemyData>(json);
            string tInput = "";
            Debug.Log(enemyData.enemy);
            foreach (Enemy t in enemyData.enemy)
            {

                tInput += "id：" + t.id + "\t";
                tInput += "name:" + t.name + "\n";
            }
            showContent = tInput;
            isListWindows = true;
        }
        else {

            titleContents = "请检查streamingAssets的json文件是否损坏，无法读取值";
            isShowWindow = true;
            return;
        }
       
    }

    private void OnOpen()
    {
        OpenFileName openFileName = new OpenFileName();

        openFileName.structSize = Marshal.SizeOf(openFileName);

        openFileName.filter = "json文件(*.json)\0*.json";

        openFileName.file = new string(new char[256]);

        openFileName.maxFile = openFileName.file.Length;

        openFileName.fileTitle = new string(new char[64]);

        openFileName.maxFileTitle = openFileName.fileTitle.Length;

        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径

        openFileName.title = "选择地图的路径";

        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        EnemyData enemyDate=null;
        if (LocalDialog.GetOpenFileName(openFileName))

        {

            Debug.Log(openFileName.file);

            Debug.Log(openFileName.fileTitle);
            if (!File.Exists(openFileName.file))
                return ;
            StreamReader sr = new StreamReader(openFileName.file);
            if (sr == null)
                return ;
            string json = sr.ReadToEnd(); 
            if (json.Length > 0) 
                enemyDate = JsonUtility.FromJson<EnemyData>(json);
            if (enemyDate != null) {

                //地图长度生成
                groud.SetActive(true);
                boardLenth = enemyDate.Length;
                Text groundLength = groud.GetComponentInChildren<Text>();
                groundLength.text = "地图长度" + boardLenth + "米";

                foreach (Enemy t in enemyDate.enemy) {

                    //530是终点
                    float len = (t.posX / boardLenth) * 530;
                    GameObject game = GameObject.Instantiate(beInstance, canvas.gameObject.transform);
                    game.name = t.name;
                    game.GetComponent<RectTransform>().anchoredPosition = new Vector2(-393 + len, -25 + t.posY);
                    game.GetComponentInChildren<Text>().text = t.name;
                    game.GetComponent<EnemyObj>().e = t;
                    Button btn = game.GetComponent<Button>();
                    enemyList.Add(game);
                    btn.onClick.AddListener(() =>
                    {
                        if (btn != null)
                        {

                            deleteGameObj = game;
                            //    enemyList.Remove(game);
                            Debug.Log(deleteGameObj.name);
                            titleContents = "选是删除对象，选否取消操作";
                            isDeleteWindow = true;
                        }
                    });
                    Debug.Log(t.posX + "y:" + t.posY);

                }
            }
        
        }
    }

    void OnGUI()
    {
        Rect windowRect;
        Rect deleteRect;
        Rect openRect;
        Rect listWindowRect;
        bool isShowWindow1 = isShowWindow;
        if (isShowWindow1 == true)
            windowRect = GUI.Window(0,new Rect(150,100, 320, 250), DoMyWindow, "提示");
        if (isDeleteWindow == true)
            deleteRect = GUI.Window(1, new Rect(150, 100, 320, 250), DeleteWindow, "是否删除这个对象");
        if(isOpenWindow==true)
            openRect = GUI.Window(2, new Rect(150, 100, 320, 250), DisCardWindow, "是否抛弃当前场景编辑的对象");
        if (isListWindows == true) {
            listWindowRect = GUI.Window(3, new Rect(150, 0, 400, 600), ListWindow, "当前所有可获取的对象");
        }

    }

    private void ListWindow(int id) {
        GUIStyle mystyle = new GUIStyle();
        mystyle.fontSize = 25;
        mystyle.fontStyle = FontStyle.Bold;
        mystyle.normal.textColor = new Color(163f / 256f, 163f / 256f, 163f / 256f, 256f / 256f);
   
        GUI.Label(new Rect(15, 50, 320, 250), showContent, mystyle);
        if (GUI.Button(new Rect(10, 500, 100, 50), "了解"))
        {
            isListWindows = false;
        }
    }

    private void DoMyWindow(int id)
    {
        GUI.Label(new Rect(10, 50, 320, 250), titleContents);
        if (GUI.Button(new Rect(10, 200, 100, 50), "了解"))
        {
            isShowWindow = false;
        }
    }

    private void DeleteWindow(int id)
    {
        GUI.Label(new Rect(10, 50, 320, 250), titleContents);
        if (GUI.Button(new Rect(150, 200, 100, 50), "是"))
        {
            isDeleteWindow = false;
       
            if (deleteGameObj != null)
            {
                enemyList.Remove(deleteGameObj);
                Destroy(deleteGameObj);
                deleteGameObj = null;
                return;
            }
           


        }
        if (GUI.Button(new Rect(10, 200, 100, 50), "否"))
        {
            isDeleteWindow = false;

        }
    }

    private void DisCardWindow(int id)
    {
        GUI.Label(new Rect(10, 50, 300, 250), titleContents);
        if (GUI.Button(new Rect(150, 200, 100, 50), "是"))
        {
            isOpenWindow = false;
            OnOpen();



        }
        if (GUI.Button(new Rect(10, 200, 100, 50), "否"))
        {
            isOpenWindow = false;

        }
    }
}
