using Caliburn.Micro;

namespace ex06_caliburn_basic.ViewModels
{
    internal class MainViewModel : Conductor<object>
    {
        public string Greeting
        {
            get { return "헬로, 켈리번!!"; }
        }
    }
}
