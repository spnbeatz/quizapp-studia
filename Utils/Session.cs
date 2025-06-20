// klasa statyczna przechowujące dane o zalogowanym użytkowniku

using Microsoft.VisualBasic.ApplicationServices;
using projekt.Models;
using User = projekt.Models.User;

namespace projekt.Utils
{
    public static class Session
    {
        private static User? _currentUser;

        public static User? CurrentUser
        {
            get => _currentUser;
            set
            {
                if (_currentUser != value)
                {
                    _currentUser = value;
                    OnAuthenticationChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        public static bool IsAuthenticated => _currentUser != null;

        public static event EventHandler? OnAuthenticationChanged;
    }
}
