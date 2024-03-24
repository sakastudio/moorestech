using System.Threading;
using Client.Common;
using Client.Game.Context;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainGame.Control.UI.PauseMenu
{
    //ゲームが終了したときかメインメニューに戻るときはサーバーを終了させます
    public class BackToMainMenu : MonoBehaviour
    {
        [SerializeField] private Button backToMainMenuButton;

        private void Start()
        {
            backToMainMenuButton.onClick.AddListener(Back);
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        private void Back()
        {
            Disconnect();
            SceneManager.LoadScene(SceneConstant.MainMenuSceneName);
        }


        private void Disconnect()
        {
            MoorestechContext.VanillaApi.SendOnly.Save();
            Thread.Sleep(50);
            MoorestechContext.VanillaApi.Disconnect();
        }
    }
}