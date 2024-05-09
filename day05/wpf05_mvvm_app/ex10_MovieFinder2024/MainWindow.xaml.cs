using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ex10_MovieFinder2024.Models;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace ex10_MovieFinder2024
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

        private async void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            //await this.ShowMessageAsync("검색", "검색을 시작합니다!!!");
            if (string.IsNullOrEmpty(TxtMovieName.Text))
            {
                await this.ShowMessageAsync("검색", "검색할 영화명을 입력하세요!!!");
                return;
            }
            SearchMovie(TxtMovieName.Text);
            isFavorite = false;         // 검색은 즐겨찾기 보기가 아님!
            ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
        }

        private async void SearchMovie(string movieName)
        {
            string tmdb_apiKey = "ce5478c7f8e15bfeafd85c8f18cf4a9c";    // TMDB 사이트에서 제공받은 API 키
            string encoding_movieName = HttpUtility.UrlEncode(movieName, Encoding.UTF8);
            string openApiUri = $"https://api.themoviedb.org/3/search/movie?api_key={tmdb_apiKey}" +
                                $"&language=ko-KR&page=1&include_adult=false&query={encoding_movieName}";
            Debug.WriteLine(openApiUri);

            string result = string.Empty;       // 결과값

            // openapi 실행 객체
            WebRequest req = null;
            WebResponse res = null;
            StreamReader reader = null;

            try
            {
                // tmdb api 요청
                req = WebRequest.Create(openApiUri);    // URL을 넣어서 객체를 생성
                res = await req.GetResponseAsync();     // 요청한 URL의 결과를 비동기응답으로 받음
                reader = new StreamReader(res.GetResponseStream()); // 
                result = reader.ReadToEnd();            // json 결과를 문자열로 저장

                Debug.WriteLine(result);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                // TODO : 메시지 박스로 출력
            }
            finally
            {
                reader.Close();
                res.Close();
            }

            // result string을 json으로 변경
            var jsonResult = JObject.Parse(result);     // type.Parse(string) 문자열을 type으로 변경
            //var total = Convert.ToInt32(jsonResult["total_results"]);
            var total = Int32.Parse(jsonResult["total_results"].ToString());
            //await this.ShowMessageAsync("검색 수", total.ToString());
            var results = jsonResult["results"];
            var jsonArray = results as JArray;  //results가 json 배열이기 때문에, JArray는 List와 동일해서 foreach 사용 가능!

            var movieItems = new List<MovieItem>();
            foreach (var item in jsonArray)
            {
                var movieItem = new MovieItem()
                {
                    // 프로퍼티라서 대문자로 시작, json 자체 키가 adult
                    // Convert.ToBoolean(object) == Boolean.Parse(string)
                    // Convert.ToDouble() == Double.Parse(string)

                    //Adult = Convert.ToBoolean(item["adult"]),
                    Adult = Boolean.Parse(item["adult"].ToString()),
                    Id = Int32.Parse(item["id"].ToString()),
                    Original_Language = item["original_language"].ToString(),
                    Original_Title = item["original_title"].ToString(),
                    Overview = item["overview"].ToString(),
                    Popularity =Double.Parse(item["popularity"].ToString()),
                    Poster_Path = item["poster_path"].ToString(),
                    Release_Date = item["release_date"].ToString(),
                    Title = item["title"].ToString(),
                    Vote_Average = Double.Parse(item["vote_average"].ToString()),
                    Vote_Count = Int32.Parse(item["vote_count"].ToString())
                };
                movieItems.Add(movieItem);
            }
            this.DataContext = movieItems;
        }

        private void TxtMovieName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSearch_Click(sender, e);         // 검색 버튼 클릭 이벤트 핸들러 실행
            }
        }

        private async void GrdResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // 재검색하면 데이터그리드 결과가 바뀌면서 이 이벤트가 다시 발생..
            try
            {
                var movie = GrdResult.SelectedItem as MovieItem;
                var poster_path = movie.Poster_Path;
                //await this.ShowMessageAsync("포스터", poster_path);

                if (string.IsNullOrEmpty(poster_path))
                {
                    ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
                }
                else
                {
                    var base_url = "https://image.tmdb.org/t/p/w300_and_h450_bestv2";
                    ImgPoster.Source = new BitmapImage(new Uri($"{base_url}{poster_path}", UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
            }
            
        }

        private async void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {
            //await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 추가합니다!");
            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("즐겨찾기", "추가할 영화를 선택하세요(복수 선택 가능)");
                return;
            }

            if (isFavorite == true)     // 즐겨찾기 보기 한 뒤 이미 즐겨찾기한 영화를 다시 즐겨찾기 하려고 할 때 막기 위함!
            {
                await this.ShowMessageAsync("즐겨찾기", "이미 즐겨찾기한 영화입니다!");
                return;
            }

            var addMovieItems = new List<MovieItem>();
            foreach (MovieItem item in GrdResult.SelectedItems)
            {
                addMovieItems.Add(item);
            }
            Debug.WriteLine(addMovieItems.Count);
            try
            {
                var insRes = 0;

                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    foreach (MovieItem item in addMovieItems)
                    {
                        // 저장되기 전에 이미 저장된 데이터인지 확인 후
                        SqlCommand chkCmd = new SqlCommand(MovieItem.CHECK_QUERY, conn);
                        chkCmd.Parameters.AddWithValue("@Id", item.Id);
                        var cnt = Convert.ToInt32(chkCmd.ExecuteScalar());  // COUNT(*) 등의 1row, 1column 값을 리턴할 때

                        if (cnt == 1) continue; // 이미 데이터가 있으면 패스!

                        SqlCommand cmd = new SqlCommand(Models.MovieItem.INSERT_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);
                        cmd.Parameters.AddWithValue("@Title", item.Title);
                        cmd.Parameters.AddWithValue("@Original_Title", item.Original_Title);
                        cmd.Parameters.AddWithValue("@Release_Date", item.Release_Date);
                        cmd.Parameters.AddWithValue("@Original_Language", item.Original_Language);
                        cmd.Parameters.AddWithValue("@Adult", item.Adult);
                        cmd.Parameters.AddWithValue("@Popularity", item.Popularity);
                        cmd.Parameters.AddWithValue("@Vote_Average", item.Vote_Average);
                        cmd.Parameters.AddWithValue("@Vote_Count", item.Vote_Count);
                        cmd.Parameters.AddWithValue("@Poster_Path", item.Poster_Path);
                        cmd.Parameters.AddWithValue("@Overview", item.Overview);

                        insRes += cmd.ExecuteNonQuery();  // 데이터 하나마다 INSERT 쿼리 실행
                    }
                }
                if (insRes == addMovieItems.Count)
                {
                    await this.ShowMessageAsync("즐겨찾기", $"즐겨찾기 {insRes}건 저장 성공!");
                }
                else
                {
                    await this.ShowMessageAsync("즐겨찾기", $"즐겨찾기 {addMovieItems.Count}건 중 {insRes}건 저장 성공!");
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 오류 {ex.Message}");
            }
            BtnViewFavorite_Click(sender, e);   // 저장 후 저장된 즐겨찾기 바로보기
        }

        private async void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {
            //await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 확인합니다!");
            this.DataContext = null;    // 데이터그리드에 보낸 데이터를 모두 삭제
            TxtMovieName.Text = string.Empty;

            List<MovieItem> favMovieItems = new List<MovieItem>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var cmd = new SqlCommand(Models.MovieItem.SELECT_QUERY, conn);
                    var adapter = new SqlDataAdapter(cmd);
                    var dSet = new DataSet();
                    adapter.Fill(dSet, "MovieItem");

                    foreach (DataRow row in dSet.Tables["MovieItem"].Rows)
                    {
                        var movieItem = new MovieItem()
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Title = Convert.ToString(row["title"]),
                            Original_Title = Convert.ToString(row["Original_Title"]),
                            Release_Date = Convert.ToString(row["Release_Date"]),
                            Original_Language = Convert.ToString(row["Original_Language"]),
                            Adult = Convert.ToBoolean(row["Adult"]),
                            Vote_Average = Convert.ToDouble(row["Vote_Average"]),
                            Vote_Count = Convert.ToInt32(row["Vote_Count"]),
                            Poster_Path = Convert.ToString(row["Poster_Path"]),
                            Overview = Convert.ToString(row["Overview"]),
                            Popularity = Convert.ToDouble(row["Popularity"]),
                            Reg_Date = Convert.ToDateTime(row["Reg_Date"]),
                        };
                        favMovieItems.Add(movieItem);
                    }
                    this.DataContext = favMovieItems;
                    isFavorite = true;      // 즐겨찾기 DB에서 
                    StsResult.Content = $"즐겨찾기 {favMovieItems.Count}건 조회완료";
                    ImgPoster.Source = new BitmapImage(new Uri("/No_Picture.png", UriKind.RelativeOrAbsolute));
                }
            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 조회 오류{ex.Message}");
            }

        }

        private async void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {
            //await this.ShowMessageAsync("즐겨찾기", "즐겨찾기 삭제합니다!");
            if (isFavorite == false)
            {
                await this.ShowMessageAsync("삭제", "즐겨찾기한 영화가 아닙니다!");
                return;
            }

            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("삭제", "삭제할 영화를 선택하세요.");
                return;
            }

            // 삭제 시작!
            try
            {
                using (SqlConnection conn = new SqlConnection(Helpers.Common.CONNSTRING))
                {
                    conn.Open();

                    var delRes = 0;

                    foreach (MovieItem item in GrdResult.SelectedItems)
                    {
                        SqlCommand cmd = new SqlCommand(Models.MovieItem.DELETE_QUERY, conn);
                        cmd.Parameters.AddWithValue("@Id", item.Id);

                        delRes += cmd.ExecuteNonQuery();
                    }
                    if (delRes == GrdResult.SelectedItems.Count)
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {delRes}건 삭제 ");
                    }
                    else
                    {
                        await this.ShowMessageAsync("삭제", $"즐겨찾기 {GrdResult.SelectedItems.Count}건 중 {delRes}건 삭제 ");

                    }
                }

            }
            catch (Exception ex)
            {
                await this.ShowMessageAsync("오류", $"즐겨찾기 삭제 오류{ex.Message}");
            }
            BtnViewFavorite_Click(sender, e);   // 즐겨찾기 보기를 재실행!
        }

        private async void BtnWatchTrailer_Click(object sender, RoutedEventArgs e)
        {
            if (GrdResult.SelectedItems.Count == 0)
            {
                await this.ShowMessageAsync("예고편 보기", "영화를 선택하세요.");
                return;
            }
            
            if (GrdResult.SelectedItems.Count > 1)
            {
                await this.ShowMessageAsync("예고편 보기", "영화를 하나만 선택하세요.");
                return;
            }

            var movieName = (GrdResult.SelectedItem as MovieItem).Title;

            var trailerWindow = new TrailerWindow(movieName);
            trailerWindow.Owner = this;
            trailerWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            trailerWindow.ShowDialog();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtMovieName.Focus();
        }

        // 데이터그리드 더블 클릭 시 발생 이벤트 핸들러
        private async void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine(GrdResult.SelectedItem);
            var curItem = GrdResult.SelectedItem as MovieItem;

            await this.ShowMessageAsync($"{curItem.Title} ({curItem.Release_Date})", curItem.Overview);
        }
    }
}