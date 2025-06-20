using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Models;

namespace projekt.Interfaces
{
    public interface IQuizService : IDisposable
    {
        Task CreateQuizAsync(Quiz quizData);
        Task<List<Quiz>> GetQuizzesAsync(int? userId = null);
        Task<Quiz> GetQuizByIdAsync(int quizId);
        Task<Quiz> GetQuizOnlyAsync(int quizId);
        Task<List<Question>> GetQuestionsForQuizAsync(int quizId);
        Task<List<Answer>> GetAnswersForQuestionAsync(int questionId);
        Task<List<QuizSummaryItem>> GetListItemData(int? userId = null);
        Task<bool> UpdateQuizAsync(Quiz quizData, int? quizId);
        Task<bool> DeleteQuizAsync(int quizId);
        Task<QuizStats> GetQuizStats(int quizId);
        Task UpdateQuizStatistics(SummaryData quiz, double earnedPoints);
    }
}
