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
