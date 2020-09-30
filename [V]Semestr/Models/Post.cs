﻿using System;

namespace _V_Semestr.Models
{
    public class Post : Model
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Desciption { get; set; }
        public string Tags { get; set; }
        public string CoverImagePath { get; set; }
        public virtual Category Category { get; set; }
        public DateTime PostedOn { get; set; }

        //public int Views{get;set;} = 0;
        //теги
    }
}
