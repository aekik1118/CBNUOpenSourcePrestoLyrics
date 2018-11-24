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
        bool havingLyricPart = false;

        public string musicFilePath { get; set; }
        public string lyricFilePath { get; set; }

        public void parsingLyrics()
        {
            lyricFilePath = FindLyricFilePath();
            string[] lines = File.ReadAllLines(lyricFilePath);
            string partOwner = null;

            lyrics.Clear();
            lyricsMsTimes.Clear();

            for (int i = 3; i < lines.Length; i++)
            {
                //시간 부분 파싱
                var splitData = lines[i].Split(']');
                var time = TimeSpan.ParseExact(splitData[0].Substring(1), @"mm\:ss\.ff", CultureInfo.InvariantCulture);
                lyricsMsTimes.Add(time.TotalMilliseconds);

                //가사 부분 파싱
                if (splitData.Length > 2)
                {
                    if (!havingLyricPart)
                        havingLyricPart = true;

                    partOwner = splitData[1] + "]";
                    lyrics.Add(partOwner + splitData[2]);
                }
                else
                {
                    if(havingLyricPart)
                        lyrics.Add(partOwner + splitData[1]);                    
                    else
                        lyrics.Add(splitData[1]);
                }           
            }
        }

        public int CloseLyricsIndex(double msTime)
        {
            int i = 0;
            while(lyricsMsTimes[i] < msTime)
            {
                i++;
                if (i == lyricsMsTimes.Count)
                    break;
            }
            return i-1;
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
