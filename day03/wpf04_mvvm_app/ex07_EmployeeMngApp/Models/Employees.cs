namespace ex07_EmployeeMngApp.Models
{
    public class Employees
    {
        public int Id { get; set; }
        public string EmpName { get; set; }
        public decimal Salary { get; set; }
        public string DeptName { get; set; }
        public string Addr { get; set; }

        public static readonly string SELECT_QUERY = @"SELECT [Id]
                                                            ,[EmpName]
                                                            ,[Salary]
                                                            ,[DeptName]
                                                            ,[Addr]
                                                        FROM [dbo].[Employees]";

        public static readonly string INSERT_QUERY = @"INSERT INTO [dbo].[Employees]
                                                            ([EmpName]
                                                            ,[Salary]
                                                            ,[DeptName]
                                                            ,[Addr])
                                                        VALUES
                                                            (@EmpName
                                                            ,@Salary
                                                            ,@DeptName
                                                            ,@Addr)";

        public static readonly string UPDATE_QUERY = @"UPDATE [dbo].[Employees]
                                                            SET [EmpName] = @EmpName
                                                                ,[Salary] = @Salary
                                                                ,[DeptName] = @DeptName
                                                                ,[Addr] = @Addr
                                                            WHERE Id = @Id";
        public static readonly string DELETE_QUERY = @"DELETE FROM [dbo].[Employees]
                                                        WHERE Id = @Id";
    }
}
