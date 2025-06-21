// serwis zarzadzajacy logiką quizów

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using projekt.Interfaces;
using projekt.Models;
using projekt.Utils;

namespace projekt.Services
{
    internal class QuizService : IQuizService
    {
        private readonly IDatabase _database;

        public QuizService(IDatabase database)
        {
            _database = database;
        }

        public async Task CreateQuizAsync(Quiz quizData)
        {
            await using var _conn = _database.GetConnection();
            await using var transaction = await _conn.BeginTransactionAsync();

            try
            {
                if (Session.CurrentUser != null && Session.CurrentUser.Id > 0)
                {
                    int quizId;

                    await using (var cmd = new NpgsqlCommand(
                        "INSERT INTO quizzes (title, userId, description, level) VALUES (@title, @userId, @description, @level) RETURNING id", _conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("title", quizData.Title);
                        cmd.Parameters.AddWithValue("userId", Session.CurrentUser.Id);
                        cmd.Parameters.AddWithValue("description", quizData.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("level", quizData.Level?.ToString() ?? (object)DBNull.Value);

                        quizId = (int)await cmd.ExecuteScalarAsync();
                    }

                    foreach (Question question in quizData.Questions)
                    {
                        int questionId;

                        await using (var cmd = new NpgsqlCommand(
                            "INSERT INTO questions (quizid, text, questiontype) VALUES (@quizid, @text, @questiontype) RETURNING id", _conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("quizid", quizId);
                            cmd.Parameters.AddWithValue("text", question.Text);
                            cmd.Parameters.AddWithValue("questionType", question.QuestionType);

                            questionId = (int)await cmd.ExecuteScalarAsync();
                        }

                        foreach (Answer answer in question.Answers)
                        {
                            await using (var cmd = new NpgsqlCommand(
                                "INSERT INTO answers (questionid, text, iscorrect) VALUES (@questionid, @text, @iscorrect)", _conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("questionid", questionId);
                                cmd.Parameters.AddWithValue("text", answer.Text);
                                cmd.Parameters.AddWithValue("iscorrect", answer.IsCorrect);

                                await cmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                MessageBox.Show("Błąd zapisu do bazy: " + ex.Message);
                Debug.WriteLine(ex.ToString());
            }
        }


        public async Task<List<Quiz>> GetQuizzesAsync(int? userId = null)
        {
            try
            {
                await using var _conn = _database.GetConnection();
                var quizzes = new List<Quiz>();

                await using var cmd = new NpgsqlCommand(@"
                    SELECT id, title, description, level, userId
                    FROM quizzes
                    WHERE (@userId IS NULL OR userId = @userId)", _conn);

                cmd.Parameters.AddWithValue("userId", (object?)userId ?? DBNull.Value);

                await using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    quizzes.Add(new Quiz
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Level = reader.IsDBNull(3) ? null : reader.GetString(3),
                        UserId = reader.GetInt32(4),
                    });
                }

                return quizzes;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd odczytu quizów: " + ex.Message);
                return new List<Quiz>();
            }
        }

        public async Task<Quiz> GetQuizByIdAsync(int quizId)
        {
            try
            {
                await using var _conn = _database.GetConnection();
                var quiz = await GetQuizOnlyAsync(quizId);
                if (quiz == null)
                    return null;

                quiz.Questions = await GetQuestionsForQuizAsync(quiz.Id);

                foreach (var question in quiz.Questions)
                {
                    question.Answers = await GetAnswersForQuestionAsync(question.Id);
                    
                }

                return quiz;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas pobierania quizu: {ex.Message}");
                return null;
            }
        }

        public async Task<Quiz> GetQuizOnlyAsync(int quizId)
        {
            await using var _conn = _database.GetConnection();
            await using var cmd = new NpgsqlCommand("SELECT id, title, description, level, userId FROM quizzes WHERE id = @quizId", _conn);
            cmd.Parameters.AddWithValue("quizId", quizId);

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Quiz
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Level = reader.IsDBNull(3) ? null : reader.GetString(3),
                    UserId = reader.GetInt32(4),
                    Questions = new List<Question>()
                };
            }

            return null;
        }

        public async Task<List<Question>> GetQuestionsForQuizAsync(int quizId)
        {
            await using var _conn = _database.GetConnection();
            var questions = new List<Question>();

            await using var cmd = new NpgsqlCommand("SELECT id, text, questiontype FROM questions WHERE quizid = @quizId", _conn);
            cmd.Parameters.AddWithValue("quizId", quizId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                questions.Add(new Question
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    QuestionType = reader.GetString(2),
                    Answers = new List<Answer>()
                });
            }

            return questions;
        }

        public async Task<List<Answer>> GetAnswersForQuestionAsync(int questionId)
        {
            await using var _conn = _database.GetConnection();
            var answers = new List<Answer>();

            await using var cmd = new NpgsqlCommand("SELECT id, text, iscorrect FROM answers WHERE questionid = @questionId", _conn);
            cmd.Parameters.AddWithValue("questionId", questionId);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                answers.Add(new Answer
                {
                    Id = reader.GetInt32(0),
                    Text = reader.GetString(1),
                    IsCorrect = reader.GetBoolean(2)
                });
            }

            return answers;
        }

        public async Task<List<QuizSummaryItem>> GetListItemData(int? userId = null)
        {
            try
            {
                await using var _conn = _database.GetConnection();
                var quizzes = new List<QuizSummaryItem>();

                await using var cmd = new NpgsqlCommand(@"
                    SELECT id, authorid, label, description, username, question_count 
                    FROM quiz_list_view 
                    WHERE (@userId IS NULL OR authorid = @userId) 
                    ORDER BY id DESC", _conn);

                cmd.Parameters.Add("userId", NpgsqlTypes.NpgsqlDbType.Integer).Value = (object)userId ?? DBNull.Value;

                await using var reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    quizzes.Add(new QuizSummaryItem
                    {
                        Id = reader.GetInt32(0),
                        AuthorId = reader.GetInt32(1),
                        Label = reader.GetString(2),
                        Description = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Username = reader.GetString(4),
                        QuestionCount = reader.GetInt32(5)
                    });
                }

                return quizzes;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return null;
            }
        }


        public async Task<bool> UpdateQuizAsync(Quiz quizData, int? quizId)
        {
            await using var _conn = _database.GetConnection();
            await using var transaction = await _conn.BeginTransactionAsync();
            MessageBox.Show($"{quizId.Value}");

            try
            {
                // Aktualizacja podstawowych danych quizu
                await using (var cmd = new NpgsqlCommand(
                    @"UPDATE quizzes SET title = @title, description = @description, level = @level WHERE id = @id", _conn, transaction))
                {
                    cmd.Parameters.AddWithValue("title", quizData.Title);
                    cmd.Parameters.AddWithValue("description", quizData.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("level", quizData.Level ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("id", quizId.Value);

                    await cmd.ExecuteNonQueryAsync();
                }

                // Usuń odpowiedzi powiązane z pytaniami tego quizu
                await using (var cmd = new NpgsqlCommand(
                    @"DELETE FROM answers WHERE questionid IN (SELECT id FROM questions WHERE quizid = @quizId)", _conn, transaction))
                {
                    cmd.Parameters.AddWithValue("quizId", quizId);
                    await cmd.ExecuteNonQueryAsync();
                }

                // Usuń pytania tego quizu
                await using (var cmd = new NpgsqlCommand(
                    "DELETE FROM questions WHERE quizid = @quizId", _conn, transaction))
                {
                    cmd.Parameters.AddWithValue("quizId", quizId);
                    await cmd.ExecuteNonQueryAsync();
                }

                // Dodaj nowe pytania i odpowiedzi
                foreach (var question in quizData.Questions)
                {
                    int questionId;
                    await using (var cmd = new NpgsqlCommand(
                        "INSERT INTO questions (quizid, text, questiontype) VALUES (@quizid, @text, @questiontype) RETURNING id", _conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("quizid", quizId);
                        cmd.Parameters.AddWithValue("text", question.Text);
                        cmd.Parameters.AddWithValue("questiontype", question.QuestionType);

                        questionId = (int)await cmd.ExecuteScalarAsync();
                    }

                    foreach (var answer in question.Answers)
                    {
                        await using (var cmd = new NpgsqlCommand(
                            "INSERT INTO answers (questionid, text, iscorrect) VALUES (@questionid, @text, @iscorrect)", _conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("questionid", questionId);
                            cmd.Parameters.AddWithValue("text", answer.Text);
                            cmd.Parameters.AddWithValue("iscorrect", answer.IsCorrect);

                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                MessageBox.Show("Błąd aktualizacji quizu: " + ex.ToString());
                Debug.WriteLine(ex.ToString());
                return false;
            }
        }


        public async Task<bool> DeleteQuizAsync(int quizId)
        {
            await using var _conn = _database.GetConnection();
            await using var transaction = await _conn.BeginTransactionAsync();

            try
            {

                await using (var statcmd = new NpgsqlCommand("DELETE FROM quiz_stats WHERE quiz_id = @quizId", _conn, transaction))
                {
                    statcmd.Parameters.AddWithValue("quizId", quizId);
                    await statcmd.ExecuteNonQueryAsync();
                }


                await using (var cmd = new NpgsqlCommand(
                    @"DELETE FROM answers WHERE questionid IN (SELECT id FROM questions WHERE quizid = @quizId)", _conn, transaction))
                {
                    cmd.Parameters.AddWithValue("quizId", quizId);
                    await cmd.ExecuteNonQueryAsync();
                }

                await using (var cmd = new NpgsqlCommand(
                    "DELETE FROM questions WHERE quizid = @quizId", _conn, transaction))
                {
                    cmd.Parameters.AddWithValue("quizId", quizId);
                    await cmd.ExecuteNonQueryAsync();
                }

                await using (var cmd = new NpgsqlCommand(
                    "DELETE FROM quizzes WHERE id = @quizId", _conn, transaction))
                {
                    cmd.Parameters.AddWithValue("quizId", quizId);
                    await cmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                MessageBox.Show("Błąd usuwania quizu: " + ex.Message);
                return false;
            }
        }

        // statystyczne

        public async Task<QuizStats> GetQuizStats(int quizId)
        {
            try
            {
                await using var _conn = _database.GetConnection();
                var cmd = new NpgsqlCommand(@"
                    SELECT quiz_id, times_solved, last_solved_at, earned_points, total_points, average_score
                    FROM quiz_stats WHERE quiz_id = @quizId", _conn);
                cmd.Parameters.AddWithValue("quizId", quizId);

                await using var reader = await cmd.ExecuteReaderAsync();


                if (await reader.ReadAsync())
                {
                    return new QuizStats
                    {
                        QuizId = reader.GetInt32(0),
                        TimesSolved = reader.GetInt32(1),
                        LastSolvedAt = reader.GetDateTime(2),
                        EarnedPoints = (float)reader.GetDouble(3),
                        TotalPoints = reader.GetInt32(4),
                        AverageScore = (float)reader.GetDouble(5),
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex) {
               
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        

        public async Task UpdateQuizStatistics(SummaryData quiz, double earnedPoints)
        {
            try
            {
                await using var _conn = _database.GetConnection();
                var cmd = new NpgsqlCommand(@"
                    UPDATE quiz_stats
                    SET
                        times_solved = times_solved + 1,
                        last_solved_at = NOW(),
                        earned_points = earned_points + @earned_points,
                        total_points = total_points + @total_points
                    WHERE quiz_id = @quiz_id;
                ", _conn);

                cmd.Parameters.AddWithValue("quiz_id", quiz.Id);
                cmd.Parameters.AddWithValue("earned_points", earnedPoints);
                cmd.Parameters.AddWithValue("total_points", quiz.Answers.Count);

                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd przy aktualizacji quiz_stats: {ex.Message}");
            }
        }
    }
}
