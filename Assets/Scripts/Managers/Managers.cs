using System;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class Managers : MonoBehaviour
{
    static Managers _instance; // 유일한 인스턴스를 담을 변수.
    static Managers Instance { get { Init(); return _instance; } } // 유일한 인스턴스를 참조하는 메서드.

    private CutSceneManager _cutScene = new CutSceneManager();
    private ScriptManager _script = new ScriptManager();
    private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();
    private DataManager _data = new DataManager();
    private ResourceManager _resource = new ResourceManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private GameManagerEx _game = new GameManagerEx();
    private Player _player = new Player();
    private UI_DialoguePopup _popup = new UI_DialoguePopup();

    public static CutSceneManager CutScene => Instance._cutScene;
    public static ScriptManager Script => Instance._script;
    public static SoundManager Sound => Instance._sound;
    public static UIManager UI => Instance._ui;
    public static DataManager Data => Instance._data;
    public static ResourceManager Resource => Instance._resource;
    public static SceneManagerEx Scene => Instance._scene;
    public static GameManagerEx Game => Instance._game;
    public static Player Player => Instance._player;
    public static UI_DialoguePopup Popup => Instance._popup;

    void Awake()
    {
        Init();
    }

    static void Init()
    {
        // Instance가 null일 때만 Managers를 찾아 Instance에 할당
        if (_instance != null) return;

        
        GameObject go = GameObject.Find("@Managers");
        if (go == null)
        {
            go = new GameObject{name = "@Managers"};
            go.AddComponent<Managers>();

        }


        DontDestroyOnLoad(go);
        _instance = go.GetComponent<Managers>();     


        _instance._data.Init();
        //_instance._dialogue.Set();
    }

    private void OnApplicationQuit()
    {
        Managers.Data.SaveData();
    }
}
