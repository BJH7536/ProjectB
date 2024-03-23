using System;
using UnityEngine;

public class UI_MainMenu : UI_Scene
{
    enum Buttons
    {
        LoadGameBtn,
        NewGameBtn,
        SettingsBtn,
        ExitGameBtn,
    }

    private void Start()
    {
        Init();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindButton(typeof(Buttons));
        
        GetButton((int)Buttons.LoadGameBtn).gameObject.BindEvent(LoadGame);
        GetButton((int)Buttons.NewGameBtn).gameObject.BindEvent(NewGame);
        GetButton((int)Buttons.SettingsBtn).gameObject.BindEvent(Settings);
        GetButton((int)Buttons.ExitGameBtn).gameObject.BindEvent(ExitGame);

        return true;
    }

    void LoadGame()
    {
        Managers.Scene.ChangeScene(Define.Scene.InGameScene);
        Managers.Game.thisGameis = Define.ThisGameis.LoadedGame;
    }

    void NewGame()
    {
        Managers.Scene.ChangeScene(Define.Scene.InGameScene);
        Managers.Game.thisGameis = Define.ThisGameis.NewGame;
    }

    void Settings()
    {
        Managers.UI.ShowPopupUI<UI_SettingsPopup>();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

}