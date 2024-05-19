using dataAPI_som.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace dataAPI_som
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            InitComboDateFromDB();
        }

        private void InitComboDateFromDB()
        {
            using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(Models.Restaurant.SELECT_QUERY, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataSet dSet = new DataSet();
                adapter.Fill(dSet);
            }
        }

        private async void BtnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string KEY = "LzwlrU0FB%2FdPNvjFqIoL9PNTtGMGQOIOPhnpfwg5iJYcbhifORStdRaT8Ouby2D8ENMY6xGlP3yhbtRUln9UHg%3D%3D";
            //string openApiUri = $"http://www.gimhae.go.kr/openapi/tour/restaurant.do?KEY={KEY}&page=20";
            string openApiUri = "";
            string result = string.Empty;

            // WebRequest, WebResponse 객체 
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            int page = 0;
            string cats = TxtSearch.Text;
            // 각 카테고리마다 데이터 수가 다 달라서 데이터 요청 시 페이지 개수가 틀려서 냅다 반복문을 쓸 수 없다..

            switch (cats)
            {
                case "한식":
                    //page = 71;
                    page = 10;
                    break;

                case "중식":
                    page = 4;
                    break;

                case "일식":
                    page = 3;
                    break;

                case "양식":
                    page = 6;
                    break;

                case "횟집":
                    page = 3;
                    break;

                case "분식":
                    page = 2;
                    break;

                case "퓨전":
                    page = 1;
                    break;

                case "카페":
                    page = 13;
                    break;

                case "기타":
                    page = 21;
                    break;
            }

            try
            {
                for (int i = 1; i < page; i++)
                {
                    openApiUri = $"http://www.gimhae.go.kr/openapi/tour/restaurant.do?KEY={KEY}&page={i}&cats={cats}";
                    req = WebRequest.Create(openApiUri);
                    res = await req.GetResponseAsync();
                    reader = new StreamReader(res.GetResponseStream());
                    result += reader.ReadToEnd();
                }

                //req = WebRequest.Create(openApiUri);
                //res = await req.GetResponseAsync();
                //reader = new StreamReader(res.GetResponseStream());
                //result = reader.ReadToEnd();

                await this.ShowMessageAsync("결과", result);

            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            var jsonResult = JObject.Parse(JsonFinialCheck(result));
            var status = Convert.ToString(jsonResult["status"]);

            if (status == "OK")
            {
                var data = jsonResult["results"];
                var jsonArray = data as JArray; // json자체에서 []안에 들어간 배열데이터만 JArray 변환가능

                var restaurantInfo = new List<Restaurant>();
                foreach (var item in jsonArray)
                {
                    restaurantInfo.Add(new Restaurant()
                    {
                        Idx = Convert.ToInt32(item["idx"]),
                        Category = Convert.ToString(item["category"]),
                        Name = Convert.ToString(item["name"]),
                        Area = Convert.ToString(item["area"]),
                        Address = Convert.ToString(item["address"]),
                        Content = Convert.ToString(item["content"]),
                        Holiday = Convert.ToString(item["holiday"]),
                        Phone = Convert.ToString(item["phone"]),
                        MainMenu = Convert.ToString(item["mainMenu"]),
                        Xposition = Convert.ToDouble(item["xposition"]),
                        Yposition = Convert.ToDouble(item["yposition"]),
                    });
                }
                this.DataContext = restaurantInfo;
            }
        }

        private async void BtnSaveData_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GrdResult.Items.Count == 0)
            {
                await this.ShowMessageAsync("저장 오류", "검색 후 저장하십시오");
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var insRes = 0;
                    foreach (Restaurant item in GrdResult.Items)
                    {
                        SqlCommand cmd = new SqlCommand(Models.Restaurant.INSERT_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Idx", item.Idx);
                        cmd.Parameters.AddWithValue("@Category", item.Category);
                        cmd.Parameters.AddWithValue("@Name", item.Name);
                        cmd.Parameters.AddWithValue("@Area", item.Area);
                        cmd.Parameters.AddWithValue("@Address", item.Address);
                        cmd.Parameters.AddWithValue("@Content", item.Content);
                        cmd.Parameters.AddWithValue("@Holiday", item.Holiday);
                        cmd.Parameters.AddWithValue("@Phone", item.Phone);
                        cmd.Parameters.AddWithValue("@MainMenu", item.MainMenu);
                        cmd.Parameters.AddWithValue("@Xposition", item.Xposition);
                        cmd.Parameters.AddWithValue("@Yposition", item.Yposition);

                        insRes += cmd.ExecuteNonQuery();
                    }

                    if(insRes > 0)
                    {
                        await this.ShowMessageAsync("저장", "DB 저장 성공!");
                    }
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("저장오류", $"저장오류 {ex.Message}");
            }
            InitComboDateFromDB();
        }

        private void GrdResult_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var curItem = GrdResult.SelectedItem as Restaurant;

            var mapWindow = new MapWindow(curItem.Xposition, curItem.Yposition);
            mapWindow.Owner = this;
            //mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapWindow.ShowDialog();
        }

        private void TxtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TxtSearch.Text = "";
        }

        //역직렬화 문제해결해주는 함수
        private static string JsonFinialCheck(string msg)
        {
            string final = string.Empty;
            char[] arr = msg.ToCharArray();

            bool bln = true;
            foreach (char item in arr)
            {
                if (bln)
                {
                    if (item == '}')
                    {
                        final += item.ToString();
                        break;
                    }
                    else
                    {
                        final += item.ToString();
                    }
                }
            }

            return final;
        }
    }
}