using System.Collections.Generic;

namespace ScenesLoading
{
    public class ProgressesView : ProgressView<float>
    {
        public override event System.Action OnProgressChanged;
        private IReadOnlyList<ProgressView<float>> progressViews;

        public ProgressesView()
        {
            ProgresValue = 1;
        }

        public ProgressesView(ProgressView<float> progressView1, ProgressView<float> progressView2)
        {
            List<ProgressView<float>> progressViews = new List<ProgressView<float>>(2);
            progressViews.Add(progressView1);
            progressViews.Add(progressView2);
            Init(progressViews);
        }
        public ProgressesView(ProgressView<float> progressView1, ProgressView<float> progressView2, ProgressView<float> progressView3)
        {
            List<ProgressView<float>> progressViews = new List<ProgressView<float>>(2);
            progressViews.Add(progressView1);
            progressViews.Add(progressView2);
            progressViews.Add(progressView3);
            Init(progressViews);
        }
        public ProgressesView(IReadOnlyList<ProgressView<float>> progressViews)
        {
            Init(progressViews);
        }

        public ProgressesView(params ProgressView<float>[] progressViews)
        {
            Init(progressViews);
        }
        private void Init(IReadOnlyList<ProgressView<float>> progressViews)
        {
            if(progressViews == null || progressViews.Count == 0)
            {
                ProgresValue = 1;
                return;
            }
            this.progressViews = progressViews;
            foreach (var item in progressViews)
            {
                item.OnProgressChanged += OnItemValueChanched;
            }
            OnItemValueChanched();
        }

        private void OnItemValueChanched()
        {
            float value = 0;
            int count = progressViews.Count;
            for (int i = 0; i < count; i++)
            {
                var progressView = progressViews[i];
                value += progressView.ProgresValue;
            }
            ProgresValue = value / count;
            OnProgressChanged?.Invoke();
        }
    }

}



