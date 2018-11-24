using Presto.SDK;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

//가사 정보를 파싱 하는 class
namespace Presto.SWCamp.Lyrics
{
    public class LyricsParser
    {
        List<string> lyrics = new List<string>(); // 가사 정보 리스트
        List<double> lyricsMsTimes = new List<double>(); // 가사의 시간 정보 리스트
        bool havingLyricPart = false;

        public string musicFilePath { get; set; } // 음악 파일 경로
        public string lyricFilePath { get; set; } // 자막 파일 경로

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
                if (splitData.Length > 2)//2이상인 경우 파트에 대한 정보가 저장되있다 가정
                {
                    if (!havingLyricPart)
                        havingLyricPart = true;

                    partOwner = splitData[1] + "]"; //파트 주인 정보 저장
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

        //입력된 msTime에 가장 가까운 가사 index값 반환
        public int CloseLyricsIndex(double msTime)
        {
            int left = 0, right = lyricsMsTimes.Count-1;
            int index = (left+right) / 2;

            while(left < right)
            {
                index = (left + right) / 2;

                if (lyricsMsTimes[index] < msTime)
                {
                    if (lyricsMsTimes[index + 1] > msTime)
                    {
                        return index;
                    }
                    else
                    {
                        left = index;
                    }
                }
                else
                {
                    if (lyricsMsTimes[index + 1] < msTime)
                    {
                        return index;
                    }
                    else
                    {
                        right = index;
                    }
                }
            }
            return index;
        }

        //index번째 가사 반환
        public string LyricsAt(int index)
        {
            return lyrics[index];
        }

        //가사 파일 검색
        private string FindLyricFilePath()
        {
            return musicFilePath.Substring(0, musicFilePath.Length - 4) + ".lrc";
        }
    }
}
