using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; } = String.Empty;
        public string Level { get; set; } = String.Empty;
        public List<Question>? Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public int Id { get; set; }
        public int? QuizId { get; set; }
        public required string Text { get; set; }
        public string QuestionType { get; set; } = "single";
        public List<Answer> Answers { get; set; } = new List<Answer>();
    }

    public class Answer
    {
        public int Id { get; set; }

        public int? QuestionId {  get; set; } 
        public required string Text { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuizItem
    {
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public string Username {  get; set; } = String.Empty;
        public string Level { get; set; } = String.Empty;
        
    }

    public class QuizSummaryItem
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public string Username { get; set; }
        public int QuestionCount { get; set; }
    }


    public class AnsweredQuestion
    {
        public string Text { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "single";
        public List<AnsweredAnswer> Answers { get; set; } = new();
        
    }

    public class AnsweredAnswer
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }

    }

    public class SummaryData
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; } = String.Empty;
        public string Level { get; set; } = String.Empty;
        public List<AnsweredQuestion> Answers { get; set; }
    }

    public class QuizStats
    {
        public int QuizId { get; set; }

        public int TimesSolved { get; set; }

        public DateTime? LastSolvedAt { get; set; }

        public float EarnedPoints { get; set; }

        public int TotalPoints { get; set; }

        public float AverageScore { get; set; }
    }

    public class QuizEventArgs : EventArgs
    {
        public int QuizId { get; }

        public QuizEventArgs(int quizId)
        {
            QuizId = quizId;
        }
    }

}
