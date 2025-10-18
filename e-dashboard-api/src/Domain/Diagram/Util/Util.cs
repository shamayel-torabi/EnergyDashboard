using System.Drawing;
using System.Text;

namespace EnergyDashboard.Domain.Diagram.Util;

public static class Utilility
{
    public static string GetSvgColorString(string prop, string value)
    {
        if (value == null)
        {
            return prop + ": none; ";
        }
        else
        {
            Color color = GetColorFromString(value);
            string str = prop + ": " + ToRGB(color) + "; ";
            float opacity = color.A / 255.0f;
            str += prop + "-opacity: " + opacity.ToString() + "; ";
            return str;
        }
    }

    public static string GetSvgStyles(Shapes.Style style)
    {
        StringBuilder st = new StringBuilder();
        string stroke = GetSvgColorString("stroke", style.StrokeStyle);
        string strokeWidth = style.StrokeWidth.ToString();
        string s = "stroke-width: " + strokeWidth + "; ";
        st.Append(stroke);
        st.Append(s);

        string fill = GetSvgColorString("fill", style.FillStyle);
        st.Append(fill);

        string opacity = style.Opacity.ToString();
        string o = "opacity: " + opacity + ";";
        st.Append(o);
        return st.ToString();
    }

    private static Color GetColorFromString(string input)
    {
        Color result = Color.FromArgb(1, 0, 0, 0);

        if (input.StartsWith("#"))
        {
            int red = 0, green = 0, blue = 0, alpha = 255;
            try
            {
                red = Convert.ToInt32(input.Substring(1, 2), 16);
                green = Convert.ToInt32(input.Substring(3, 2), 16);
                blue = Convert.ToInt32(input.Substring(5, 2), 16);

                if (input.Length > 6)
                    alpha = Convert.ToInt32(input.Substring(7, 2), 16);
                else
                    alpha = 255;
            }
            catch
            {
                //Debug.Write("Invalid Color value provided");
            }
            result = Color.FromArgb(alpha, red, green, blue);
        }
        else
        {
            result = Color.FromName(input);
        }
        return result;
    }

    private static string ToRGB(Color color)
    {
        return "rgb(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + ")";
    }

}
