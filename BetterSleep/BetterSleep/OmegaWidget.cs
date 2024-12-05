namespace BetterSleep
{
    public abstract class OmegaWidget : ELayer, IOmegaWidget<OmegaWidget>
    {
        public abstract OmegaWidget Setup(object arg);

        private void LateUpdate()
        {
            foreach (Window window in this.windows)
            {
                window.cg.alpha = (window.setting.transparent ? window.Skin.transparency : 1f);
            }
        }
    }
}