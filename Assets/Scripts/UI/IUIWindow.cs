using System;

namespace UI
{
    public enum EUIType
    {
        None,
        TitleScreen,
        LoadingScreen,
        SkinSelectionScreen,
        HUD
    }

    public interface IUIWindow
    {
        public void Init(object data);
        public void Show(Action onShowComplete);
        public void Hide(Action onHideComplete);
    }
}
