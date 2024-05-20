using dataAPI_som.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
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
        }

        private void InitComboDateFromDB()
        {
        }

        private async void BtnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string KEY = "LzwlrU0FB%2FdPNvjFqIoL9PNTtGMGQOIOPhnpfwg5iJYcbhifORStdRaT8Ouby2D8ENMY6xGlP3yhbtRUln9UHg%3D%3D";
            string openApiUri;
            string result = string.Empty;

            // WebRequest, WebResponse 객체 
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // 각 카테고리마다 데이터 수가 다 달라서 데이터 요청 시 페이지 개수가 틀려서 냅다 반복문을 쓸 수 없다..
            int page = 0;
            string cats = TxtSearch.Text;
            var allResults = new List<JObject>(); // JSON 결과를 저장할 리스트

            // 페이지 수가 5개가 넘어가면 데이터 불러오기 실패...
            switch (cats)
            {
                case "한식":
                    page = 71;
                    break;

                case "중식":
                    page = 4; break;

                case "일식":
                    page = 3; break;

                case "양식":
                    //page = 6;
                    page = 5; break;

                case "횟집":
                    page = 3; break;

                case "분식":
                    page = 2; break;

                case "퓨전":
                    page = 1; break;

                case "카페":
                    //page = 13;
                    page = 5; break;

                case "기타":
                    //page = 21;
                    page = 5; break;
                default:
                    await this.ShowMessageAsync("오류", "유효하지 않은 카테고리입니다.");
                    return;
            }

            try
            {
                for (int i = 1; i <= page; i++) // 페이지 1부터 시작
                {
                    openApiUri = $"http://www.gimhae.go.kr/openapi/tour/restaurant.do?KEY={KEY}&page={i}&cats={cats}";
                    req = WebRequest.Create(openApiUri);
                    res = await req.GetResponseAsync();
                    reader = new StreamReader(res.GetResponseStream());
                    result = reader.ReadToEnd();

                    var jsonResponse = JObject.Parse(result); // 개별 JSON 응답 파싱
                    var status = Convert.ToString(jsonResponse["status"]);
                    if (status == "OK")
                    {
                        var data = jsonResponse["results"] as JArray;
                        foreach (var item in data)
                        {
                            allResults.Add(item as JObject); // 결과 리스트에 추가
                        }
                    }
                }

                //await this.ShowMessageAsync("결과", allResults.ToString());


                if (allResults.Count > 0)
                {
                    var jsonResultList = new JArray(allResults); // 모든 결과를 JArray로 변환
                    await this.ShowMessageAsync("결과", jsonResultList.ToString());
                }
                else
                {
                    await this.ShowMessageAsync("결과", "데이터가 없습니다.");
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"OpenAPI 조회오류 {ex.Message}");
            }

            if (allResults.Count > 0)
            {
                //int count = 0;      // 데이터 잘 넘어오는지 확인용
                var restaurantInfo = new List<Restaurant>();
                foreach (var item in allResults)
                {
                    restaurantInfo.Add(new Restaurant()
                    {
                        //Count = count++,
                        Idx = Convert.ToInt32(item["idx"]),
                        Category = Convert.ToString(item["category"]),
                        Name = Convert.ToString(item["name"]),
                        Area = Convert.ToString(item["area"]),
                        Address = Convert.ToString(item["address"]),
                        Content = Convert.ToString(item["content"]),
                        Holiday = Convert.ToString(item["holiday"]),
                        Phone = Convert.ToString(item["phone"]),
                        Xposition = Convert.ToString(item["xposition"]),
                        Yposition = Convert.ToString(item["yposition"]),
                    }); ;
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
        }

        private void GrdResult_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var curItem = GrdResult.SelectedItem as Restaurant;
            double Xpositoin = Convert.ToDouble(curItem.Xposition);
            double Ypositoin = Convert.ToDouble(curItem.Yposition);

            var mapWindow = new MapWindow(Xpositoin, Ypositoin);
            mapWindow.Owner = this;
            //mapWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            mapWindow.ShowDialog();
        }

        private void TxtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TxtSearch.Text = string.Empty;
        }
    }
}