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
        bool choiceMusic = false; //음악 선택시 true

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
            lyricsParser.parsingLyricsFile(); // 노래의 가사를 파싱
            
            textLyrics.Text = ""; // 가사창 초기화
            textBeforeLyrics.Text = "";
            textNextLyrics.Text = "";
            choiceMusic = true;
            //throw new NotImplementedException();
        }

        private void printLyrics(int index)
        {
            if (index < 0)
            {
                textBeforeLyrics.Text = " ";
                textLyrics.Text = "간주 중";
                textNextLyrics.Text = lyricsParser.LyricsAt(index+1);
            }
            else if (index == 0)
            {
                textBeforeLyrics.Text = " ";
                textLyrics.Text = lyricsParser.LyricsAt(index);
                textNextLyrics.Text = lyricsParser.LyricsAt(index + 1);
            }
            else if (index == lyricsParser.GetLyricsCount()-1)
            {
                textBeforeLyrics.Text = lyricsParser.LyricsAt(index - 1);
                textLyrics.Text = lyricsParser.LyricsAt(index);
                textNextLyrics.Text = " ";
            }
            else
            {
                textBeforeLyrics.Text = lyricsParser.LyricsAt(index-1);
                textLyrics.Text = lyricsParser.LyricsAt(index);
                textNextLyrics.Text = lyricsParser.LyricsAt(index + 1);
            }


        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if(choiceMusic)
            {
                int index = lyricsParser.CloseLyricsIndex(PrestoSDK.PrestoService.Player.Position);
                printLyrics(index);

            }
            else
            {
                //음악이 선택되지 않았을때 디폴트 가사창
                textLyrics.Text = "";
            }
        }
    }
}
