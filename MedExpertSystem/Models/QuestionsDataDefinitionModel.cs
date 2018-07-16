using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MedExpertSystem.Models
{
    public class QuestionsDataDefinitionModel
    {
        public int Index { get; set; }
        public string QuestionAnswerOptionOne { get; set; }
        public static string [] Queastions { get; set; }
        public static string[] Diagnosis { get; set; } 


    }
}