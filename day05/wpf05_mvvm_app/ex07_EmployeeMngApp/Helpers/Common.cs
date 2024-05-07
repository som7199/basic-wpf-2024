using MahApps.Metro.Controls.Dialogs;

namespace ex07_EmployeeMngApp.Helpers
{
    public class Common
    {
        public static readonly string CONNSTRING = "Data Source=localhost;" +
                                                    "Initial Catalog=EMS;" +
                                                    "Persist Security Info=True;" +
                                                    "User ID=ems_user;" +
                                                    "Encrypt=False;"+
                                                    "Password=ems_p@ss";
        // Metro 방식 다이얼로그 적용을 위해선 반드시 필요!
        public static IDialogCoordinator DialogCoordinator { get; set; }
    }
}
