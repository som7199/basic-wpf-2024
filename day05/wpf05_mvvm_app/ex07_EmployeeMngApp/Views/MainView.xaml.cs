using ex07_EmployeeMngApp.Helpers;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ex07_EmployeeMngApp.Views
{
    /// <summary>
    /// MainView.xaml에 대한 상호 작용 논리
    /// 여기는 프로그래밍 코딩은 안함!!!!!!
    /// </summary>
    public partial class MainView : MetroWindow
    {
        public MainView()
        {
            InitializeComponent();
            Common.DialogCoordinator = DialogCoordinator.Instance;      // 생성된 다이얼로그꾸미기 객체를 공통으로 이전
            this.DataContext = DialogCoordinator.Instance;
        }
    }
}
