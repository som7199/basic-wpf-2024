using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ex02_wpf_controls
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void FrmMain_Loaded(object sender, RoutedEventArgs e)
        {
            var fonts = Fonts.SystemFontFamilies;

            foreach (var font in fonts)
            {
                CboFonts.Items.Add(font);
            }
        }

        private void CboFonts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           ChangeFont();
        }

        void ChangeFont()
        {
            if (CboFonts.SelectedIndex < 0) return;

            Debug.WriteLine(CboFonts.SelectedItem.ToString());

            TxtSampleText.FontFamily = CboFonts.SelectedItem as FontFamily;
            if (ChkItalic.IsChecked == true) TxtSampleText.FontStyle = FontStyles.Italic;
            else TxtSampleText.FontStyle = FontStyles.Normal;

            if (ChkBold.IsChecked == true) TxtSampleText.FontWeight = FontWeights.Bold;
            else TxtSampleText.FontStyle = FontStyles.Normal;
        }
        private void ChkBold_Checked(object sender, RoutedEventArgs e)
        {
            ChangeFont();
        }

        private void ChkItalic_Checked(object sender, RoutedEventArgs e)
        {
            ChangeFont();
        }

        private void ChkBold_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeFont();
        }

        private void ChkItalic_Unchecked(object sender, RoutedEventArgs e)
        {
            ChangeFont();
        }

    }
}