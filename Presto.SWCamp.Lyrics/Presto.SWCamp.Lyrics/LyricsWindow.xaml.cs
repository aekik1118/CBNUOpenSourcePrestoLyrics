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
        LyricsParser lyricsParser;
        int curLyricIndex = -2;
        bool choiceMusic = false;

        public LyricsWindow()
        {
            InitializeComponent();
            lyricsParser = new LyricsParser();

            

            var timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(100)
			};

            PrestoSDK.PrestoService.Player.StreamChanged += Player_StreamChanged;

            timer.Tick += Timer_Tick;
			timer.Start();
		}

        private void Player_StreamChanged(object sender, Common.StreamChangedEventArgs e)
        {        
            var musicFilePath = PrestoSDK.PrestoService.Player.CurrentMusic.Path;
            lyricsParser.musicFilePath = musicFilePath;
            lyricsParser.parsingLyrics();
            curLyricIndex = -1;
            textLyrics.Text = "가사 창";
            choiceMusic = true;
            //throw new NotImplementedException();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(choiceMusic)
            {
                if(lyricsParser.CloseLyricsIndex(PrestoSDK.PrestoService.Player.Position) >= 0)
                {
                    textLyrics.Text = lyricsParser.LyricsAt(lyricsParser.CloseLyricsIndex(PrestoSDK.PrestoService.Player.Position));
                }                                   
            }
            else
            {
                textLyrics.Text = "가사 창";
            }
        }
        //PrestoSDK.PrestoService.Player.Position
    }
}
