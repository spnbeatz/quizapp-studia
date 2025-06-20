// klasa przechowująca ustawienia aplikacji

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt.Config
{
    public class AppSettings
    {
        public string Language { get; set; } = "pl";
        public string Theme { get; set; } = "light";
    }
}
