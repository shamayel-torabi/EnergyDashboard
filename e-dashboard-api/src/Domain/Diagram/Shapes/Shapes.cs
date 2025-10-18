using System.Text.Json.Serialization;

namespace EnergyDashboard.Domain.Diagram.Shapes;

[JsonDerivedType(typeof(Point), nameof(Point))]
public class Point
{
    public Point(double x, double y)
    {
        this.X = x;
        this.Y = y;
    }
    public double X { get; set; }
    public double Y { get; set; }
}


[JsonDerivedType(typeof(Rectangle), nameof(Rectangle))]
[JsonDerivedType(typeof(Circle), nameof(Circle))]
[JsonDerivedType(typeof(Line), nameof(Line))]
[JsonDerivedType(typeof(PolyLine), nameof(PolyLine))]
public class Shape
{
    public Shape() {
        this.Angle = 0;
    }
    public double Angle { get; set; }

}
public class Rectangle : Shape
{
    public Rectangle(double x, double y , double width , double height)
    {
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
        this.Angle = 0;
    }
    public double X { get; set; }
    public double Y { get; set; }
    public double Height { get; set; }
    public double Width { get; set; }
    public int SelectionZoneWidth { get; set; }
}
public class Circle : Shape
{
    public Circle(double x, double y, double radius)
    {
        this.X = x;
        this.Y = y;
        this.Radius = radius;
    }
    public double X { get; set; }
    public double Y { get; set; }
    public double Radius { get; set; }
}
public class Line : Shape
{
    public Line(Point p1, Point p2)
    {
        this.P1 = p1;
        this.P2 = p2;
        this.SelectionZoneWidth = 4;
    }
    public Point P1 { get; set; }
    public Point P2 { get; set; }
    public int SelectionZoneWidth { get; set; }
}
public class PolyLine : Shape
{
    public PolyLine()
    {
        this.SelectionZoneWidth = 4;
        this.Points = new List<Point>();
    }
    public IList<Point> Points { get; set; }
    public int SelectionZoneWidth { get; set; }
}
