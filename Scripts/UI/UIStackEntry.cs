namespace YeKostenko.CoreKit.UI
{
    public readonly struct UIStackEntry
    {
        public readonly UIWindow Window;
        public readonly bool Modal;

        public UIStackEntry(UIWindow window, bool modal)
        {
            Window = window;
            Modal = modal;
        }
    }
}