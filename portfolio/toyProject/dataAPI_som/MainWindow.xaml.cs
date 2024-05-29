using dataAPI_som.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace dataAPI_som
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        // flag값
        private bool isFavorite = false;    // 즐겨찾기인지 API로 검색한건지 false => openAPI, true => 즐겨찾기 보기

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            TxtSearch.Focus();
        }

        private async void BtnSearch_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            isFavorite = false;         // 검색은 즐겨찾기 보기가 아님!

            string KEY = "LzwlrU0FB%2FdPNvjFqIoL9PNTtGMGQOIOPhnpfwg5iJYcbhifORStdRaT8Ouby2D8ENMY6xGlP3yhbtRUln9UHg%3D%3D";
            string openApiUri;
            string result = string.Empty;

            // WebRequest, WebResponse 객체 
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            // 각 카테고리마다 데이터 수가 다 달라서 데이터 요청 시 페이지 개수가 틀리기 때문에 반복문 사용X
            int page = 0;
            string cats = TxtSearch.Text;
            var allResults = new List<JObject>(); // JSON 결과를 저장할 리스트

            switch (cats)
            {
                case "한식":
                    page = 71;
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
                    //await this.ShowMessageAsync("결과", jsonResultList.ToString());
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
                //int count = 0;      // 데이터 개수 (조회 건수)
                var restaurantInfo = new List<Restaurant>();
                foreach (var item in allResults)
                {
                    restaurantInfo.Add(new Restaurant()
                    {
                        //Count = count++,    // 데이터 잘 넘어오는지 확인
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
                StsResult.Content = $"{restaurantInfo.Count}건 조회 완료";
            }
        }

        private void GrdResult_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var curItem = GrdResult.SelectedItem as Restaurant;
            double Xposition = Convert.ToDouble(curItem.Xposition);
            double Yposition = Convert.ToDouble(curItem.Yposition);

            var mapWindow = new MapWindow(Xposition, Yposition);
            mapWindow.Owner = this;
            mapWindow.ShowDialog();
        }

        private void TxtSearch_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TxtSearch.Text = string.Empty;
        }

        private void TxtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearch_Click(sender, e);     // 검색 버튼 클릭 이벤트 핸들러 실행
            }
        }

        // 즐겨찾기한 식당 DB 저장
        private async void BtnAddFavorite_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("즐겨찾기", "추가할 식당을 선택하세요(복수 선택 가능)");
                return;
            }

            if (isFavorite == true)     // 즐겨찾기 보기 한 뒤 이미 즐겨찾기한 식당을 다시 즐겨찾기 하려고 할 때 막기 위함!
            {
                await this.ShowMessageAsync("즐겨찾기", "이미 즐겨찾기한 식당입니다!");
                return;
            }

            var addRestaurants = new List<Restaurant>();
            foreach (Restaurant item in GrdResult.SelectedItems)
            {
                addRestaurants.Add(item);
            }

            Debug.WriteLine(addRestaurants.Count);
            try
            {
                var insRes = 0;
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    foreach (Restaurant item in addRestaurants)
                    {
                        // 저장하기 전 이미 저장된 데이터인지 확인(식당 이름으로 확인)
                        SqlCommand chkCmd = new SqlCommand(Restaurant.CHECK_QUERY, conn);
                        chkCmd.Parameters.AddWithValue("@Name", item.Name);
                        var cnt = Convert.ToInt32(chkCmd.ExecuteScalar());  // COUNT(*) 등의 1row, 1column 값을 리턴할 때
                        if (cnt == 1) continue; // 이미 데이터가 있으면 패스!

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

                        insRes += cmd.ExecuteNonQuery();    // 데이터 하나마다 INSERT 쿼리 실행
                    }
                }
                if (insRes == addRestaurants.Count)
                {
                    await this.ShowMessageAsync("즐겨찾기", $"즐겨찾기 {insRes}건 저장 성공!");
                }
                else
                {
                    await this.ShowMessageAsync("즐겨찾기", $"즐겨찾기 {addRestaurants.Count}건 중 {insRes}건 저장 성공!");
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 오류 {ex.Message}");
            }
            BtnViewFavorite_Click(sender, e);   // 저장 후 저장된 즐겨찾기 바로보기
        }

        // 즐겨찾기한 식당만 보이기!
        private async void BtnViewFavorite_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.DataContext = null;    // 데이터그리드에 보낸 데이터 모두 삭제
            TxtSearch.Text = string.Empty;

            List<Restaurant> favRestaurants = new List<Restaurant>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var cmd = new SqlCommand(Models.Restaurant.SELECT_QUERY, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "Restaurant");

                    foreach (DataRow row in dSet.Tables["Restaurant"].Rows)
                    {
                        var restaurant = new Restaurant()
                        {
                            Idx = Convert.ToInt32(row["Idx"]),
                            Category = Convert.ToString(row["Category"]),
                            Name = Convert.ToString(row["Name"]),
                            Area = Convert.ToString(row["Area"]),
                            Address = Convert.ToString(row["Address"]),
                            Content = Convert.ToString(row["Content"]),
                            Holiday = Convert.ToString(row["Holiday"]),
                            Phone = Convert.ToString(row["Phone"]),
                            Xposition = Convert.ToString(row["Xposition"]),
                            Yposition = Convert.ToString(row["Yposition"]),
                        };
                        favRestaurants.Add(restaurant);
                    }
                    this.DataContext = favRestaurants;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {favRestaurants.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 조회 오류{ex.Message}");
            }
        }

        // 카테고리별 즐겨찾기 추가한 식당 보이기
        private async void CboViewFavorite_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedCategory = (e.AddedItems[0] as ComboBoxItem)?.Content.ToString();
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                showFavoriteRestaurant(selectedCategory);
            }
        }

        private async void showFavoriteRestaurant(string cate)
        {
            this.DataContext = null;    // 데이터그리드에 보낸 데이터 모두 삭제
            TxtSearch.Text = string.Empty;

            List<Restaurant> favRestaurants = new List<Restaurant>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(Models.Restaurant.SELECT_DISTINCT_QUERY, conn);
                    cmd.Parameters.AddWithValue("@Category", cate); // @ Category 매개변수 추가

                    var reader = await cmd.ExecuteReaderAsync();    // 비동기적으로 데이터 읽기
                    while (await reader.ReadAsync())                // 각 행을 읽어서 Restaurant 객체 생성 후 리스트에 추가
                    {
                        var restaurant = new Restaurant()
                        {
                            Idx = reader.GetInt32(reader.GetOrdinal("Idx")),
                            Category = reader.GetString(reader.GetOrdinal("Category")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Area = reader.GetString(reader.GetOrdinal("Area")),
                            Address = reader.GetString(reader.GetOrdinal("Address")),
                            Content = reader.GetString(reader.GetOrdinal("Content")),
                            Holiday = reader.GetString(reader.GetOrdinal("Holiday")),
                            Phone = reader.GetString(reader.GetOrdinal("Phone")),
                            Xposition = reader.GetString(reader.GetOrdinal("Xposition")),
                            Yposition = reader.GetString(reader.GetOrdinal("Yposition")),
                        };
                        favRestaurants.Add(restaurant);
                    }

                    reader.Close();
                    this.DataContext = favRestaurants;
                    isFavorite = true;
                    StsResult.Content = $"즐겨찾기 {favRestaurants.Count}건 조회완료";
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 조회 오류: {ex.Message}");
            }
        }

    }
}
