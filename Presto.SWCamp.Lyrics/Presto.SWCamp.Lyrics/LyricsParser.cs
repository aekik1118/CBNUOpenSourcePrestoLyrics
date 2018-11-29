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
        int sizeOfLyricsMass = 1;
        int lyricsCount = 1; //가사 수
        public string musicFilePath { get; set; } // 음악 파일 경로
        public string lyricFilePath { get; set; } // 자막 파일 경로

        public int GetSizeOfLyricsMass() { return sizeOfLyricsMass; }
        public int GetLyricsCount() { return lyricsCount; }

        public void parsingLyricsFile()
        {
            lyricFilePath = FindLyricFilePath();
            GetLyricsTimeAndLyricsFromFile();
            lyricsCount = lyricsMsTimes.Count;
        }

        //입력된 msTime에 가장 가까운 가사 index값 반환
        public int CloseLyricsIndex(double msTime)
        {
            int left = 0, right = lyricsCount - 1;
            int index = (left+right) / 2;

            if (msTime < lyricsMsTimes[0])//시작 가사 예외처리
                return -1;

            if (msTime > lyricsMsTimes[lyricsMsTimes.Count - 1])//마지막 가사 예외처리
                return lyricsMsTimes.Count - 1;

            while (left+1 != right)
            {
                index = (left + right) / 2;

                if (lyricsMsTimes[index] <= msTime)
                {
                    if (lyricsMsTimes[index + 1] > msTime)
                    {
                        return index;
                    }
                    else
                    {
                        left = index+1;
                    }
                }
                else
                {
                    if (lyricsMsTimes[index - 1] <= msTime)
                    {
                        return index - 1;
                    }
                    else
                    {
                        right = index - 1;
                    }
                }
            }

            return left;
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

        private void GetLyricsTimeAndLyricsFromFile()
        {
            //가사파일로 부터 데이터를 받아 저장
            string[] lines = File.ReadAllLines(lyricFilePath);
            string partOwner = null;

            //기존의 가사와 가사시간 초기화
            lyrics.Clear();
            lyricsMsTimes.Clear();
            havingLyricPart = false;
            sizeOfLyricsMass = 1;
            lyricsCount = 1;

            //가서 첫줄에 시간정보와 가사정보를 파싱하여 맴버변수에 각각 저장
            var splitData = lines[3].Split(']');
            var time = TimeSpan.ParseExact(splitData[0].Substring(1), @"mm\:ss\.ff", CultureInfo.InvariantCulture);
            lyricsMsTimes.Add(time.TotalMilliseconds);

            if (splitData.Length > 2)//2이상인 경우 파트에 대한 정보가 저장되있다 가정
            {            
                havingLyricPart = true;
                partOwner = splitData[1] + "] "; //파트 주인 정보 저장
                lyrics.Add(partOwner + splitData[2]);
            }
            else
                lyrics.Add(splitData[1]);

            //이전의 line의 시간과 현재 파싱중인 line의 시간을 비교하여 가사의 줄수(sizeOfLyricsMass)를 구한다. 이와 동시에 가사 시간정보와 가사 저장
            for (int i = 4; i < lines.Length; i++)
            {
                //시간 부분 파싱
                splitData = lines[i].Split(']');
                time = TimeSpan.ParseExact(splitData[0].Substring(1), @"mm\:ss\.ff", CultureInfo.InvariantCulture);

                //이전 시간과 동일하면 sizeOfLyricsMass 증가
                if (lyricsMsTimes[0] == time.TotalMilliseconds)
                {
                    sizeOfLyricsMass++;

                    //같은 시간대이므로 이전과 같은 타이밍에 가사가 출력되야한다.

                    if (splitData.Length > 2)//2이상인 경우 파트에 대한 정보가 저장되있다 가정
                    {
                        partOwner = splitData[1] + "] "; //파트 주인 정보 저장
                        //첫번째 가사와 같은 타이밍에 나와야 하므로 partOwner + splitData[2]
                        lyrics[0] += ("\n" + partOwner + splitData[2]);
                    }
                    else
                    {
                        if (havingLyricPart)
                            lyrics[0] += ("\n" + partOwner + splitData[1]);
                        else
                            lyrics[0] += ("\n" + splitData[1]);
                    }


                }
                else //이전줄과 같은 시간대가 아니면 sizeOfLyricsMass 측정을 멈춘다
                {
                    lyricsMsTimes.Add(time.TotalMilliseconds);

                    if (splitData.Length > 2)//2이상인 경우 파트에 대한 정보가 저장되있다 가정
                    {
                        partOwner = splitData[1] + "] "; //파트 주인 정보 저장
                        lyrics.Add(partOwner + splitData[2]);
                    }
                    else
                    {
                        if (havingLyricPart)
                            lyrics.Add(partOwner + splitData[1]);
                        else
                            lyrics.Add(splitData[1]);
                    }
                    if (sizeOfLyricsMass == 1) // 한줄가사일때의 예외처리
                    {
                        lyrics.RemoveAt(1);
                        lyricsMsTimes.RemoveAt(1);
                    }
                    break;
                }
            }
            //sizeOfLyricsMass를 이용하여 가사뭉텅이로 처리
            for (int i = 3 + sizeOfLyricsMass; i < lines.Length; i+= sizeOfLyricsMass)
            {
                //시간 부분 파싱
                splitData = lines[i].Split(']');
                time = TimeSpan.ParseExact(splitData[0].Substring(1), @"mm\:ss\.ff", CultureInfo.InvariantCulture);
                lyricsMsTimes.Add(time.TotalMilliseconds);

                //가사 부분 파싱 //첫가사 제외 맨앞에 띄어쓰기 들어가는거 제거 필요
                string lyricsMass = "";

                for (int j=0; j< sizeOfLyricsMass; j++)
                {
                    if (i + j >= lines.Length)
                    {
                        lyrics.Add(lyricsMass);
                        return;
                    }
                   
                    if(j!= 0)
                    {
                        splitData = lines[i + j].Split(']');
                        lyricsMass += "\n";
                    }
                                
                    if (splitData.Length > 2)//2이상인 경우 파트에 대한 정보가 저장되있다 가정
                    {
                        partOwner = splitData[1] + "] "; //파트 주인 정보 저장
                        lyricsMass += (partOwner + splitData[2]);
                    }
                    else
                    {
                        if (havingLyricPart)
                            lyricsMass += (partOwner + splitData[1]);            
                        else
                            lyricsMass += (splitData[1]);                 
                    }
                }
                lyrics.Add(lyricsMass);
            }
        }

    }
}
