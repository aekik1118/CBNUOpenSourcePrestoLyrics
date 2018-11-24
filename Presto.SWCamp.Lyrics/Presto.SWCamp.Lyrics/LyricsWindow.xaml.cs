using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Presto.SWCamp.Lyrics
{
    /// <summary>
    /// LyricsWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsWindow : Window
    {
        public LyricsWindow()
        {
            InitializeComponent();

            LyricsParser lyricsParser = new LyricsParser("aa");
            lyricsParser.parsingLyrics();

			var timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(100)

			};

			timer.Tick += Timer_Tick;
			timer.Start();
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			textLyrics.Text = PrestoSDK.PrestoService.Player.Position.ToString();
		}
	}
}
