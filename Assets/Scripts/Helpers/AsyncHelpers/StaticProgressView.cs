namespace ScenesLoading
{
    public class StaticProgressView : ProgressView<float>
    {
        public StaticProgressView(float progress)
        {
            ProgresValue = progress.GetClamped01();
        }

        public void SetProgress(float progress)
        {
            ProgresValue = progress.GetClamped01();
        }
    }

}



