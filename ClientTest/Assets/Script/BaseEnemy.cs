using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Game/Enemy")]
public class BaseEnemy : MonoBehaviour
{
  public Transform m_transform;
  

    // 动画组件
    protected Animator m_ani;

    // 寻路组件
    protected UnityEngine.AI.NavMeshAgent m_agent;

    // 主角实例
    protected  Player m_player;

    //怪物类型
    public string enemytype;

    //最大生命值
    public int max_life;

    // 移动速度
    protected float m_movSpeed = 2.5f;

    // 旋转速度
    protected  float m_rotSpeed = 5.0f;

    // 计时器
    protected float m_timer =2;

    // 当前生命值
    public   int m_life = 8;

    // 普通攻击伤害量
    protected int m_damage = 10;

    // 普通攻击攻击范围
    protected float m_attackdis = 2.5f;

    // 警戒范围
    protected float m_warndis = 5f;

    //击杀获得金币
    protected int m_gold = 100;

    //普通攻击判定
    bool isdamage = false;

    //受击
    bool isgethit = false;
    bool isdie = false;

    // // 出生点
    // protected EnemySpawn m_spawn;

    // // 死亡音效
    // public AudioClip die_audio;
    // protected bool isdiemusicplay=false;

    // // 攻击音效
    // public AudioClip attack_audio;
    // protected bool isattackmusicplay=false;


    // Use this for initialization
    void Start () {


        Init();




    }


    // Update is called once per frame
    void Update()
    {
   
        basecontrl();

    }
    //初始化
    public void Init()
    {
    

        m_transform = this.transform;
        // 获得动画播放器
        m_ani = this.GetComponent<Animator>();

        // 获得主角
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        m_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        m_agent.speed = m_movSpeed;

        // 获得寻路组件
       // m_agent.SetDestination(m_player.m_transform.position);

        // //依据难度系数调整属性(血量 1 1.5 2 攻击距离1 1.25 1.5 伤害1 1.5 2）
        // m_life = m_life + m_life * (PlayerPrefs.GetInt("m_dif") - 1) / 2;
        // m_attackdis = m_attackdis + m_attackdis * (PlayerPrefs.GetInt("m_dif") - 1) / 4;
        // m_damage= m_damage + m_damage * (PlayerPrefs.GetInt("m_dif") - 1) / 2;

        max_life = m_life;

    }

    //
    public void basecontrl()
    {
      
    
        if (m_player.now_life <= 0)
            return;
        if(m_life<=0)
        m_ani.SetBool("die",true);
        // 更新计时器
        m_timer -= Time.deltaTime;

        // 获取当前动画状态
        AnimatorStateInfo stateInfo = m_ani.GetCurrentAnimatorStateInfo(0);

        // 如果处于待机且不是过渡状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.idle")&& !m_ani.IsInTransition(0))
        {

            m_ani.SetBool("idle", false);

            // 停止寻路
            m_agent.ResetPath();

            // 待机一定时间

            if (m_timer > 0)
                return;

            //被攻击进入收击
             if (isgethit)
            {
                //m_ani.SetBool("gethit", true);
                isgethit = false;
                m_ani.Play("gethit", 0, 0f);
            }

            // 如果距离主角小于攻击范围，进入攻击动画状态
            else if (Vector3.Distance(m_transform.position, m_player.m_transform.position) < m_attackdis)
            {
               
                m_ani.SetBool("attack", true);

                isdamage = false;
            }
      
            //如果距离主角小于进阶范围或者被攻击，进入追逐状态
            else if(Vector3.Distance(m_transform.position, m_player.m_transform.position) < m_warndis || m_life<max_life)
            {
                // 重置定时器
                m_timer = 1;

                // 设置寻路目标点
                m_agent.SetDestination(m_player.m_transform.position);

                // 进入跑步动画状态
                m_ani.SetBool("run", true);
            }
        }

        // 如果处于跑步且不是过渡状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.run")&& !m_ani.IsInTransition(0))
        {
            m_ani.SetBool("run", false);

            // 每隔1秒重新定位主角的位置
            if (m_timer < 0)
            {
                m_agent.SetDestination(m_player.m_transform.position);

                m_timer = 1;
            }
            //被攻击进入收击
            if (isgethit)
            {
                // m_ani.SetBool("gethit", true);

                isgethit = false;
                m_ani.Play("gethit", 0, 0f);
            }
            // 如果距离主角小于攻击范围，向主角攻击
            else if (Vector3.Distance(m_transform.position, m_player.m_transform.position) <= m_attackdis)
            {
                // 停止寻路
                m_agent.ResetPath();
                // 进入攻击状态
                m_ani.SetBool("attack", true);

                isdamage = false;
            }
          
        }

        // 如果处于攻击且不是过渡状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.attack")&& !m_ani.IsInTransition(0))
        {
            // 面向主角
            RotateTo();
            m_ani.SetBool("attack", false);

            // //播放攻击音效
            // if (isattackmusicplay == false)
            // {
            //     this.GetComponent<AudioSource>().clip = attack_audio;
            //     this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip);
            //     isattackmusicplay = true;
            // }
            if(isgethit){
                //m_ani.SetBool("gethit",true);
                isgethit = false;
                m_ani.Play("gethit", 0, 0f);
            }
            // 如果动画将要播完，完成伤害判定
            else if (stateInfo.normalizedTime >= 0.5 && isdamage == false)
            {

                //如果player没有闪开伤害完成判定
                if (Vector3.Distance(m_transform.position, m_player.m_transform.position) <= m_attackdis )
                    m_player.GetGit(m_damage);

                isdamage = true;

            }
            else if (stateInfo.normalizedTime >= 1.0)
            {
                m_ani.SetBool("idle", true);

                // 重置计时器待机2秒
                m_timer = 2;
                // isattackmusicplay = false;
            }
        }
       // 如果处于被攻击
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.gethit") && !m_ani.IsInTransition(0))
        {
             m_ani.SetBool("gethit", false);
            if (isgethit)
            {
                isgethit = false;
                m_ani.Play("gethit", 0, 0f);
            }
            if(isdie){
                m_ani.SetBool("die",true);
            }
             //受击，完成静止
            else if(stateInfo.normalizedTime >= 1.0)
            {
                m_ani.SetBool("idle",true);
                m_timer=1.0f;
            }
        }
        // 如果处于死亡且不是过渡状态
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.die") &&!m_ani.IsInTransition(0))
        {
            m_ani.SetBool("death", false);

            // //播放死亡音效
            // if (isdiemusicplay == false)
            // {
            //     this.GetComponent<AudioSource>().clip = die_audio;
            //     this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip);
            //     isdiemusicplay = true;
            // }
    
            // // 加分
            // GameManager.Instance.SetScore(m_score);

            // 当播放完成死亡动画
            if (stateInfo.normalizedTime >= 1.0f)
            {
                // //更新敌人计数
                // m_spawn.m_enemyCount--;

                // 加分
                GameManager.Instance.SetGold(m_gold);


                // 销毁自身
                Destroy(this.gameObject);
            }
        }
        //维持x坐标为0
        m_transform.position = new Vector3(0, m_transform.position.y, m_transform.position.z);
    }
    // // 生成点怪物数量加一
    // public void Initspawn(EnemySpawn spawn)
    // {
    //     m_spawn = spawn;

    //     m_spawn.m_enemyCount++;
    // }
    
    // 转向目标点
    public void RotateTo()
    {
        // 获取目标（Player）方向
        Vector3 targetdir = m_player.m_transform.position - m_transform.position;
        // 计算出新方向
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetdir, m_rotSpeed * Time.deltaTime, 0.0f);
        // 旋转至新方向
        m_transform.rotation = Quaternion.LookRotation(newDir);
    }


    // 受到伤害
    public void OnDamage(int damage)
    {
        m_life -= damage;

        isgethit=true;
        // 如果生命值为0播放死亡动画
        if (m_life <= 0)
        {
            // m_ani.SetBool("death", true);
            // 停止寻路
            isdie=true;
            m_agent.ResetPath(); 
        }
    }
    //眩晕
    public void Bedizzy()
    {
        if (m_life > 0)
        {
            // 停止寻路
            m_agent.ResetPath();
            m_ani.SetBool("idle", true);
            // 重置计时器待机4秒
            m_timer = 4;
        }
    }
}
