# Mapeditor
this is Mepeditor Project
# Mapeditor
this is Mepeditor Project
<p align="center">
    <img width="400px" src="https://github.com/Drinkwang/drinkwang.github.io/blob/master/img/git.png?raw=true">    
</p>


# # H5开发带有地图编辑器的游戏



* 首先我们需要一个编辑器相关的东西得借助什么。这块有很多种选择，比如xml，csv、json、甚至纯文本都是可以的

但是基于小巧和种种原因，我最终选择了json，并且决定地图编辑器用U3d实现，而游戏则用H5实现

首先思考何为地图编辑器，这个得从游戏开发中的脚本开始说起了，其实脚本语言和外置的json、xml高度相似，如果你读过游戏的编译原理这本书的话...



> 书籍地址:https://book.douban.com/subject/1927405/



[toc]

## 1.地图编辑器到底需要什么

### 1.如果采取json格式如何设计数据类型

首先在游戏里，必然存在地形、障碍物、敌人、特效多种类型的元素，当然如果要设计一些比较高端的元素可能需要在json中外部引用进行扩展脚本，这里就不在这一次探讨的范围，这次我们主要还是区分地形，障碍物、敌人、特效这几种情况进行设计。

不同的敌人、障碍物、地形都得设置不同的参数，json包括的作用就很好显示出来了，也就是葫芦娃套娃，一层套一层，单位放在单位类里面，而这个单位的属性又放在单位这个类里面，单位的属性又有具体的子属性。

为了方便大家的理解，我这里直接放出一个实例给大家看看

```json
{
    "enemy": [
        {
            "id": 207,
            "type": 2,
            "name": "瘦子",
            "posX": 55.0,
            "posY": 0.0,
            "C": 30.0,
            "Size": 0.10000000149011612,
            "shakeTime": 300.0,
            "speedipa": 300.0,
            "idleAni": "slim_idle",
            "beHitAni": "slim_die",
            "audioSrc": "Q",
            "isbeHit": true
        },
        {
            "id": 901,
            "type": 2,
            "name": "女神",
            "posX": 80.0,
            "posY": 0.0,
            "C": 10.0,
            "Size": -0.15000000596046449,
            "shakeTime": 100.0,
            "speedipa": 100.0,
            "idleAni": "Lady_idle",
            "beHitAni": "Lady_die",
            "audioSrc": "Q",
            "isbeHit": false
        },
        {
            "id": 207,
            "type": 2,
            "name": "瘦子2",
            "posX": 25.0,
            "posY": 0.0,
            "C": 30.0,
            "Size": 0.10000000149011612,
            "shakeTime": 300.0,
            "speedipa": 300.0,
            "idleAni": "slim_idle",
            "beHitAni": "slim_die",
            "audioSrc": "Q",
            "isbeHit": true
        }
    ],
    "Length": 100
}
```

引入眼帘就是各种参数了，这里参数并没有进行太过复杂的套娃，毕竟我目前开发这款游戏属于偏休闲横板游戏，自然也没有设计过于复杂的参数和套娃模式了。

这里还是给大家介绍下这些参数的作用。

| 字段名    |    数值类型     |                                                           值 | 作用                                                   |
| --------- | :-------------: | -----------------------------------------------------------: | ------------------------------------------------------ |
| id        |     number      |                                                          207 | 用于区分具体对象                                       |
| type      |     number      |                                                            2 | 进行功能函数调用                                       |
| name      |     String      |                                                 瘦子（敌人） | 用于给对象起名字                                       |
| posX      | number（float） |                                                         55.0 | 判断对象x位置                                          |
| posY      | number（float） | 0.0-为了避免参数太多，这里应该和x放在一起，string里逗号进行split分开比较好但是这里为了比较便于理解就分开了 | 判断对象y位置                                          |
| C         | number（float） |                                                         30.0 | 参数C，通过公式来计算，来为不同单位添加不同的打击感    |
| Size      | number（float） |                                          0.10000000149011612 | 参数SIze，通过公式来计算，来为不同单位添加不同的打击感 |
| shakeTime | number（float） |                                                        300.0 | 单位被击杀后晃动摄像机时间                             |
| idleAni   |     String      |                                                    slim_idle | 单位默认的动画文件                                     |
| beHitAni  |     string      |                                                     slim_die | 单位被打击事播放的动画文件                             |
| audioSrc  |     string      |                                                            Q | 单位死亡播放的声音文件                                 |
| isbeHit   |      bool       |                                                         true | 是否需要被击杀进行计分                                 |
| func1     |       obj       |                                                   everyThing | 预留字段，保留设计                                     |
| func2     |       obj       |                                                   everyThing | （可能用作后期扩展脚本）                               |

​      

| 这里看上去有些参数过多的感觉，但这些参数都只属于地图编辑器内部的字段，如果每一项每一次都要使用编辑器的人来填写，那么肯定会发疯，所以我们可以设置一个保存一堆**默认组件**的json，使用默认组件会自动为所有其他参数赋默认值。这样使用编辑器的人就舒服了。可以直接把对象调出来

   ```
{
    "enemy": [
	
		{
            "id": 901,
            "type": 2,
            "name": "女神",
            "posX": 5.0,
            "posY": 0.0,
            "C": 10.0,
            "Size": -0.15,
            "shakeTime": 100.0,
            "speedipa": 100.0,
			"idleAni": "Lady_idle",
            "beHitAni": "Lady_die",
            "audioSrc": "Q",
            "isbeHit": false
        },
        {
            "id": 101,
            "type": 2,
            "name": "草坪",
            "posX": 5.0,
            "posY": 0.0,
            "C": 10.0,
            "Size": 0.02,
            "shakeTime": 100.0,
            "speedipa": 100.0,
			"idleAni": "grass_idle.png",
            "beHitAni": "null",
            "audioSrc": "Q",
            "isbeHit": false
        },
        {
            "id": 102,
            "type": 1,
            "name": "酒瓶",
            "posX": 10.0,
            "posY": 0.0,
            "C": 20.0,
            "Size": 0.05,
            "shakeTime": 200.0,
            "speedipa": 200.0,
            "idleAni": "bottle_idle.png",
			"beHitAni": "null",
            "audioSrc": "duang",
            "isbeHit": false
        },
        {
            "id": 103,
            "type": 3,
            "name": "地刺",
            "posX": 15.0,
            "posY": 0.0,
            "C": 20.0,
            "Size": -0.25,
            "shakeTime": 200.0,
            "speedipa": 300.0,
			"idleAni": "spikes_idle.png",
            "beHitAni": "null",
            "audioSrc": "Q",
            "isbeHit": false
        },
        {
            "id": 201,
            "type": 2,
            "name": "碎石",
            "posX": 20.0,
            "posY": 0.0,
            "C": 10.0,
            "Size": 0.05,
            "shakeTime": 200.0,
            "speedipa": 200.0,            
			"idleAni": "debris.png",
            "beHitAni": "null",
            "audioSrc": "pu",
            "isbeHit": false
        },
        {
            "id": 202,
            "type": 2,
            "name": "水坑",
            "posX": 25.0,
            "posY": 0.0,
            "C": 20.0,
            "Size": 0.01,
            "shakeTime": 200.0,
            "speedipa": 200.0,            
			"idleAni": "pool_idle.png",
            "beHitAni": "null",
            "audioSrc": "pu",
            "isbeHit": false
        },
        {
            "id": 203,
            "type": 2,
            "name": "广告牌",
            "posX": 30.0,
            "posY": 0.0,
            "C": 20.0,
            "Size": 0.01,
            "shakeTime": 200.0,
            "speedipa": 200.0,            
			"idleAni": "NoPeeSign_idle",
            "beHitAni": "NoPeeSign_die",
            "audioSrc": "Q",
            "isbeHit": false
        },
        {
            "id": 205,
            "type": 2,
            "name": "汽车",
            "posX": 41.0,
            "posY": 0.0,
            "C": 50.0,
            "Size": 0.1,
            "shakeTime": 400.0,
            "speedipa": 400.0,            
			"idleAni": "car_idle",
            "beHitAni": "car_die",
            "audioSrc": "Q",
            "isbeHit": false
        },
        {
            "id": 206,
            "type": 2,
            "name": "油桶",
            "posX": 46.0,
            "posY": 0.0,
            "C": 40.0,
            "Size": 0.01,
            "shakeTime": 400.0,
            "speedipa": 400.0,            
			"idleAni": "oildrum_idle",
            "beHitAni": "oildrum_die",
            "audioSrc": "oil",
            "isbeHit": false
        },
        {
            "id": 207,
            "type": 2,
            "name": "瘦子",
            "posX": 51.0,
            "posY": 0.0,
            "C": 30.0,
            "Size": 0.1,
            "shakeTime": 300.0,
            "speedipa": 300.0,            
			"idleAni": "slim_idle",
            "beHitAni": "slim_die",
            "audioSrc": "Q",
            "isbeHit": true
        },
        {
            "id": 208,
            "type": 2,
            "name": "胖子",
            "posX": 56.0,
            "posY": 0.0,
            "C": 50.0,
            "Size": 0.1,
            "shakeTime": 400.0,
            "speedipa": 400.0,            
			"idleAni": "fat_idle",
            "beHitAni": "fat_die",
            "audioSrc": "Q",
            "isbeHit": true
        },
        {
            "id": 105,
            "type": 1,
            "name": "易拉罐",
            "posX": 66.0,
            "posY": 0.0,
            "C": 10.0,
            "Size": 0.05,
            "shakeTime": 100.0,
            "speedipa": 100.0,            
			"idleAni": "can_idle.png",
            "beHitAni": "null",
            "audioSrc": "duang",
            "isbeHit": false
        },
        {
            "id": 106,
            "type": 1,
            "name": "篮球",
            "posX": 71.0,
            "posY": 0.0,
            "C": 30.0,
            "Size": 1.0,
            "shakeTime": 100.0,
            "speedipa": 200.0,            
			"idleAni": "ball_idle.png",
            "beHitAni": "null",
            "audioSrc": "box",
            "isbeHit": false
        }
    ],
    "Length": 100
}
   ```

###  2.编辑器的代码实现

下面这个章节，着重描述如何设计出地图编辑器的核心功能

- 如何读写文件(读取/载入写好的界面)
- 设计地图编辑器的ui（懒狗就用OnGui，效率低，大家可以换成Ugui）
- 设计交互逻辑，增加限定条件

***

##### 如何读写文件(读取/载入写好的界面)

```c#
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
```

​	通过LocalDialog.GetSaveFileName,获取路径，然后通过jsonUtilityToJson将数组给序列化，具体enemy里面保存的就是我们json中各个字段

```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class Enemy
{
    public int id;
    public int type;
    public string name;
    public float posX;
    public float posY;
    public double C;
    public float Size;
    public float shakeTime;
    public float speedipa;
    public string idleAni;
    public string beHitAni;
    public string audioSrc;
    public bool isbeHit;

}
[Serializable]
public class EnemyData
{
    public Enemy[] enemy;
    public int Length;
}
```



然后我们看看读取的代码，其实也大同小异

```c#
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
```

前半部分是读取的代码，读取到json值赋予给地图编辑器的各个编辑窗口

**最后再来描述一种我之前讲的内容，预物体，大概的思路也是读取json，通过一张完整的json表（具体可以看第一节），然后通过id获取json表的值对象**



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



##### 设计地图编辑器的ui（懒狗就用OnGui，效率低，大家可以换成Ugui）



```c#
 void OnGUI(){
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
...
```


这里没什么好说的，一个ugui的简单实现，其实是懒得画ui。写了跟多无意义的代码，酌情使用哈



##### 设计交互逻辑，增加限定条件



    public void GenerateGroud() {
            if (length.text.Length > 0) {
                boardLenth = int.Parse(length.text);
                //  length.contentType =
                if (boardLenth > 0)
                {
                    groud.SetActive(true);
                    //groundLength.gameObject.SetActive(true);
                    Text groundLength = groud.GetComponentInChildren<Text>();
                    groundLength.text = "地图长度" + boardLenth + "米";       }
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

这里大概就是创建地形时，以及使用组件时，为游戏里可能报错的条件加入一个限制，限制地图编辑器的无脑使用，也为后期进行修改和程序提供了便利



## 游戏该怎么做

这个，这篇文章，主要还是讲地图编辑器相关的，但我会放出游戏地址和地图编辑器Github的相关代码。

游戏地址：[能玩的](https://dash-man-release.vercel.app/)

Github：[能康的](https://github.com/Drinkwang/Myunityproject)

虽然我觉得大家看看源码就应该能看懂这个项目了，不过我还是想看看大家的反馈，对游戏制作相关方面有没有兴趣，如果反馈的好，就会有下集哦
