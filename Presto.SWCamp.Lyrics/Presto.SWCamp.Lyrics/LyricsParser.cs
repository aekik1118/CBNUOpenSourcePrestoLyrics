﻿using System;
using System.Collections.Generic;
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
        List<string> lyricsTimes = new List<string>();
        string title;

        public LyricsParser(string title)
        {
            this.title = title;
        }

        public void parsingLyrics()
        {
            string[] lines = File.ReadAllLines("C:\\Users\\cbnu\\Documents\\Presto.Lyrics.Sample\\Musics\\볼빨간사춘기 - 여행.lrc");

            for(int i=3; i<=lines.Length; i++)
            {
                var splitData = lines[i].Split(']');           

            }
      
        }
    }
}
