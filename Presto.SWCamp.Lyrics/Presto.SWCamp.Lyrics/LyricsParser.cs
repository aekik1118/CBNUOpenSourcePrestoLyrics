using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Presto.SWCamp.Lyrics
{
    public class LyricsParser
    {
        List<string> lyrics = new List<string>();
        List<double> lyricsMsTimes = new List<double>();

        public string musicFilePath { get; set; }
        public string lyricFilePath { get; set; }       

        public void parsingLyrics()
        {
            lyricFilePath = FindLyricFilePath();
            string[] lines = File.ReadAllLines(lyricFilePath);

            lyrics.Clear();
            lyricsMsTimes.Clear();

            for (int i=3; i< lines.Length; i++)
            {
               //시간 부분 파싱
               var splitData = lines[i].Split(']');
               var time = TimeSpan.ParseExact(splitData[0].Substring(1), @"mm\:ss\.ff", CultureInfo.InvariantCulture);
               lyrics.Add(splitData[1]);         
               lyricsMsTimes.Add(time.TotalMilliseconds);       
            }     
        }

        public int CloseLyricsIndex(double msTime)
        {
            int i = 0;
            while(lyricsMsTimes[i + 1] < msTime)
            {
                i++;
            }
            return i--;
        }

        public bool IsChangeLyric(int index, double msTime)
        {
            return lyricsMsTimes[index + 1] < msTime;
        }

        public string LyricsAt(int index)
        {
            return lyrics[index];
        }

        private string FindLyricFilePath()
        {
            return musicFilePath.Substring(0, musicFilePath.Length - 4) + ".lrc";
        }
    }
}
