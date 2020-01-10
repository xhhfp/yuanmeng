using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using LitJson;
using System.IO;
using System.Collections.Generic;

[AddComponentMenu("Game/GameManager")]
public class GameManager : MonoBehaviour {

    public static GameManager Instance = null;

    public int m_gold = 0;


    // public static int m_hiscore = 0;


    Player m_player;

    //重新开始标记
    public bool restartflag = false;

    public bool winflag = false;
    // public float flash_Speed = 5;

    // public bool restartflag = false;


    float curtime;
    float starttime;

    // Text txt_ammo;
    // Text txt_hiscore;
    Text txt_life;
    Text txt_gold;
    // Text txt_energy;
    // Text txt_weaname;
    // Text txt_gamedif;
    Text txt_usetime;
    // Text txt_medkitnum;
    // Text txt_drinknum;
    Text txt_enemylife;
    // Text txt_damagegrenum;
    // Text txt_dizzygrenum;
    // GameObject wincanvas;
    // Text txt_windif;
    // Text txt_wintime;
    // Text txt_winscore;
    // Text txt_cong;

    // Image image_hurt;
     Image image_enemy;

    Button button_win_again;
    Button button_win_quit;

    Button button_fail_again;
    Button button_fail_quit;
    Button button_fail_revive;

    Button button_pause_return;
    Button button_pause_quit;

    Scrollbar bar_playlife;

     Scrollbar bar_enemylife;

    // Scrollbar bar_playenergy;


    // public AudioClip win_audio;

    // public AudioClip lose_audio;
    GameObject base_canvas;
    GameObject win_canvas;
    GameObject fail_canvas;
    GameObject pause_canvas;

    GameObject[] enemys;
    void Start()
    {

        Time.timeScale = 1;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Instance = this;

        starttime = Time.time;

        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        base_canvas = GameObject.Find("base_canvas");
        foreach (Transform t in base_canvas.transform.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("txt_life") == 0)
            {
                txt_life = t.GetComponent<Text>();
            }
            else if (t.name.CompareTo("lifebar") == 0)
            {
                bar_playlife = t.GetComponent<Scrollbar>();
            }
            else if (t.name.CompareTo("enemy_image") == 0)
            {
                image_enemy = t.GetComponent<Image>();
            }
            if (t.name.CompareTo("txt_enemylife") == 0)
            {
                txt_enemylife = t.GetComponent<Text>();
            }
            else if (t.name.CompareTo("enemy_lifebar") == 0)
            {
                bar_enemylife = t.GetComponent<Scrollbar>();
            }
            else if (t.name.CompareTo("txt_gold") == 0)
            {
                txt_gold = t.GetComponent<Text>();
            }
            else if (t.name.CompareTo("txt_time") == 0)
            {
                txt_usetime = t.GetComponent<Text>();
            }

        }

        win_canvas = GameObject.Find("win_canvas");

        foreach (Transform t in win_canvas.transform.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("win_again") == 0)
            {
                button_win_again = t.GetComponent<Button>();
                button_win_again.onClick.AddListener(delegate ()
                {
                    win_canvas.gameObject.SetActive(false);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);            
                });      
            }
            else if (t.name.CompareTo("win_quit") == 0)
            {
                button_win_quit = t.GetComponent<Button>();
                button_win_quit.onClick.AddListener(delegate ()
                {
                    win_canvas.gameObject.SetActive(false);
                    SceneManager.LoadScene("HOME");
                });;
            }
        }
        win_canvas.gameObject.SetActive(false);

        fail_canvas = GameObject.Find("fail_canvas");

        foreach (Transform t in fail_canvas.transform.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("fail_again") == 0)
            {
                button_fail_again = t.GetComponent<Button>();
                button_fail_again.onClick.AddListener(delegate ()
                {
                    fail_canvas.gameObject.SetActive(false);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            }
            else if (t.name.CompareTo("fail_quit") == 0)
            {
                button_fail_quit = t.GetComponent<Button>();
                button_fail_quit.onClick.AddListener(delegate ()
                {
                    fail_canvas.gameObject.SetActive(false);
                    SceneManager.LoadScene("HOME");
                });

            }
            else if (t.name.CompareTo("fail_revive") == 0)
            {
                button_fail_revive = t.GetComponent<Button>();
                button_fail_revive.onClick.AddListener(delegate ()
                {
                    Debug.Log("revive");
                });
            }
        }
        fail_canvas.gameObject.SetActive(false);

        pause_canvas = GameObject.Find("pause_canvas");

        foreach (Transform t in pause_canvas.transform.GetComponentsInChildren<Transform>())
        {
            if (t.name.CompareTo("pause_return") == 0)
            {
                button_pause_return = t.GetComponent<Button>();
                button_pause_return.onClick.AddListener(delegate ()
                {

                    pause_canvas.gameObject.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    Time.timeScale = 1;

                });
            }
            else if (t.name.CompareTo("pause_quit") == 0)
            {
                button_pause_quit = t.GetComponent<Button>();
                button_pause_quit.onClick.AddListener(delegate ()
                {
                    pause_canvas.gameObject.SetActive(false);
                    SceneManager.LoadScene("HOME");
                });
            }
        }
        pause_canvas.gameObject.SetActive(false);

        SetLife(m_player.now_life);

    }

    void Update()
    {
    

        updatetime();
        SetLife(m_player.now_life);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0;

            pause_canvas.gameObject.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

      
        enemys = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemys.Length == 0)
        {
            winflag = true;
        }
        if (winflag)
        {
            Win();
        }
    }

    public void updatetime()
    {
        curtime = Time.time;

        int usetime = (int)(curtime - starttime);

        txt_usetime.text = usetime / 60 + " m " + usetime % 60 + " s";
    }

    public void SetGold(int gold)
    {
        m_gold += gold;
        txt_gold.text = "Gold : " + m_gold; ;
    }

    public void SetLife(int life)
    {
        if (life < 0)
            life = 0;

        bar_playlife.size = 1.0f * life / m_player.max_life;
        bar_playlife.value = 0;

        txt_life.text = life.ToString();
        if (life < 20)
        {
            txt_life.color = Color.red;
        }
        else if (life < 60)
        {
            txt_life.color = Color.yellow;
        }
        else
        {
            txt_life.color = Color.green;
        }

        if (life <= 0 && restartflag == true)
        {
            Time.timeScale = 0;
            //this.GetComponent<AudioSource>().clip = lose_audio;
            //this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip);
            restartflag = false;
            fail_canvas.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }

    public void Win()
    {
        Time.timeScale = 0;

        //this.GetComponent<AudioSource>().clip = win_audio;
        //this.GetComponent<AudioSource>().PlayOneShot(this.GetComponent<AudioSource>().clip);

        win_canvas.gameObject.SetActive(true);

        //         txt_cong.text = "you win!";
        //         txt_windif.text = txt_gamedif.text;
        //         txt_wintime.text = txt_usetime.text;
        //         txt_winscore.text = txt_score.text;
        //         Savedata(ResultToJson(PlayerPrefs.GetInt("m_dif"),(int)(curtime - starttime), m_score));
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    // public void SetWeaname(string weaname)
    // {
    //     txt_weaname.text = weaname;
    //     if (weaname == "AK47")
    //     {
    //         txt_weaname.color = Color.red;
    //     }
    //     else if (weaname == "M249")
    //     {
    //         txt_weaname.color = Color.blue;
    //     }
    //     else if (weaname == "M16")
    //     {
    //         txt_weaname.color = Color.white;
    //     }
    // }

    // public void SetColor(bool damage)
    // {
    //     if (damage)
    //         image_hurt.color = Color.red;
    //     else
    //         image_hurt.color = Color.Lerp(image_hurt.color, Color.clear, flash_Speed * Time.deltaTime);
    // }


    public void Setenemylife(int life, int maxlife)
    {
        if (life < 0)
            life = 0;
        txt_enemylife.text = life.ToString();

        if (life < maxlife * 2 / 10)
        {
            txt_enemylife.color = Color.red;
        }
        else if (life < maxlife * 6 / 10)
        {
            txt_enemylife.color = Color.yellow;
        }
        else
        {
            txt_enemylife.color = Color.green;
        }
    
        bar_enemylife.size = 1.0f * life / maxlife;
        bar_enemylife.value = 0;

    }

    public void Setenemyimage(string enemytype)
    {
        if (enemytype == "Monster1")
        {
            Debug.Log("monster1");
            image_enemy.sprite = Resources.Load("monster1", typeof(Sprite)) as Sprite;
        }
        else if (enemytype == "Monster2")
        {
            image_enemy.sprite = Resources.Load("monster2", typeof(Sprite)) as Sprite;
        }

    }



    // public string ResultToJson(int dif, int time, int score)
    // {
    //     StringBuilder res = new StringBuilder();
    //     JsonWriter WriteDate = new JsonWriter(res);

    //     WriteDate.WriteObjectStart();
    //     WriteDate.WritePropertyName("dif");
    //     WriteDate.Write(dif.ToString());
    //     WriteDate.WritePropertyName("time");
    //     WriteDate.Write(time.ToString());
    //     WriteDate.WritePropertyName("score");
    //     WriteDate.Write(score.ToString());
    //     WriteDate.WriteObjectEnd();

    //     return res.ToString();
    // }

    // public void Savedata(string str)
    // {
    //     FileInfo fi = new FileInfo(Application.dataPath + "/Resources/Json.txt");
    //     StreamWriter sw = null;
    //     if (fi.Exists)
    //     {
    //         sw = fi.AppendText();
    //     }
    //     else
    //     {
    //         sw = fi.CreateText();
    //     }
    //     sw.WriteLine(str);
    //     sw.Close();
    // }

}