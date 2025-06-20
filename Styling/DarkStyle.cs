// klasa stylu ciemnego

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Interfaces;

namespace projekt.Styling
{
    public class DarkStyle : IAppStyle
    {
        // Główne kolory
        public Color Background => Color.FromArgb(30, 30, 30);
        public Color Foreground => Color.WhiteSmoke;
        public Color Primary => Color.FromArgb(10, 132, 255);
        public Color Secondary => Color.FromArgb(70, 70, 70);
        public Color Accent => Color.FromArgb(255, 85, 85);
        public Color Border => Color.FromArgb(90, 90, 90);
        public Color ElementBackground => Color.FromArgb(50, 50, 50);

        // Czcionki
        public Font DefaultFont => new Font("Segoe UI", 9);
        public Font TitleFont => new Font("Segoe UI", 12, FontStyle.Bold);
        public Font ButtonFont => new Font("Segoe UI", 9, FontStyle.Bold);

        // Ikony

        public Image themeButton => Image.FromFile("Assets/moon.png");
        public Image confirmButton => Image.FromFile("Assets/checkmark_dark.png");
        public Image cancelButton => Image.FromFile("Assets/close_dark.png");
        public Image playButton => Image.FromFile("Assets/play_dark.png");
        public Image editButton => Image.FromFile("Assets/edit_dark.png");
        public Image searchButton => Image.FromFile("Assets/search.png");
        public Image deleteButton => Image.FromFile("Assets/delete_dark.png");

    }
}
