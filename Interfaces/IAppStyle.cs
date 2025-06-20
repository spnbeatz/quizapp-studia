using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt.Interfaces
{
    public interface IAppStyle
    {
        // Kolory
        Color Background { get; }
        Color Foreground { get; }
        Color Primary { get; }
        Color Secondary { get; }
        Color Accent { get; }
        Color Border { get; }
        Color ElementBackground { get; }

        // Fonty
        Font DefaultFont { get; }
        Font TitleFont { get; }
        Font ButtonFont { get; }

        // Ikony

        Image themeButton { get; }
        Image confirmButton {  get; }
        Image cancelButton {  get; }
        Image playButton { get; }
        Image editButton { get; }
        Image searchButton {  get; }
        Image deleteButton {  get; }
    }
}
