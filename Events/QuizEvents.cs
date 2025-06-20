// klasa do wywoływania eventów

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Models;

namespace projekt.Events
{
    public static class QuizEvents
    {
        public static event EventHandler? QuizAdded;
        public static event EventHandler<QuizEventArgs> QuizRemoved;

        public static void NotifyQuizAdded()
        {
            QuizAdded?.Invoke(null, EventArgs.Empty);
        }

        public static void NotifyQuizRemoved(int id) {
            QuizRemoved?.Invoke(null, new QuizEventArgs(id));
        }
    }
}
