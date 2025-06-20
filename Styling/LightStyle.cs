// klasa stylu jasnego ze zdefiniowanymi zmiennymi styli, czcionek oraz ikon

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using projekt.Interfaces;

namespace projekt.Styling
{
    public class LightStyle : IAppStyle
    {
        // Główne kolory
        public Color Background => Color.FromArgb(245, 245, 245);
        public Color Foreground => Color.FromArgb(30, 30, 30);
        public Color Primary => Color.FromArgb(0, 120, 215);
        public Color Secondary => Color.FromArgb(230, 230, 230);
        public Color Accent => Color.FromArgb(220, 53, 69);
        public Color Border => Color.FromArgb(200, 200, 200);
        public Color ElementBackground => Color.FromArgb(220, 220, 220);

        // Czcionki
        public Font DefaultFont => new Font("Segoe UI", 9);
        public Font TitleFont => new Font("Segoe UI", 12, FontStyle.Bold);
        public Font ButtonFont => new Font("Segoe UI", 9, FontStyle.Bold);

        // Ikony

        public Image themeButton => Image.FromFile("Assets/sun.png");
        public Image confirmButton => Image.FromFile("Assets/checkmark_light.png");
        public Image cancelButton => Image.FromFile("Assets/close_light.png");
        public Image playButton => Image.FromFile("Assets/play_light.png");
        public Image editButton => Image.FromFile("Assets/edit_light.png");
        public Image searchButton => Image.FromFile("Assets/search_light.png");
        public Image deleteButton => Image.FromFile("Assets/delete_light.png");


    }

}
