using System;
using UnityEngine;

public class UI_InGame : UI_Scene
{
    enum Buttons
    {
        PauseBtn,
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
        
        GetButton((int)Buttons.PauseBtn).gameObject.BindEvent(Pause);

        return true;
    }

    public void Pause()
    {
        Managers.UI.ShowPopupUI<UI_PausePopup>();
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