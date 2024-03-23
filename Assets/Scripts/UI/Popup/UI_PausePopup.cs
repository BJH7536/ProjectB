using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_PausePopup : UI_Popup
{
    enum Images
    {
        Background,
    }

    enum Buttons
    {
        SettingsBtn,
        BacktoMainMenuBtn,
        ExitBtn,
    }

    public override bool Init()
    {
        BindImage(typeof(Images));
        BindButton(typeof(Buttons));
        
        GetImage((int)Images.Background).gameObject.BindEvent(ClosePopupUI);
        GetButton((int)Buttons.SettingsBtn).gameObject.BindEvent(Settings);
        GetButton((int)Buttons.BacktoMainMenuBtn).gameObject.BindEvent(BacktoMainMenu);
        GetButton((int)Buttons.ExitBtn).gameObject.BindEvent(ExitGame);

        Time.timeScale = 0.0f;     // 일시정지
        
        return true;
    }

    public override void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
        Time.timeScale = 1.0f;
    }
    
    void BacktoMainMenu()
    {
        Managers.Data.SaveData();
        Managers.Scene.ChangeScene(Define.Scene.MainMenuScene);
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
