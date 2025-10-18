using System.Text;

namespace EnergyDashboard.Domain.Diagram.Shapes
{
    public interface ITransmisionLineProperties : IProperties
    {
        string DispachingCode { get; set; }
        string IgmcCode { get; set; }
        int ToolId { get; set; }
        double TransferCapacity { get; set; }
        int LineFeederType { get; set; }
        string SrcShapeName { get; set; }
        string DesShapeName { get; set; }
    }

    public class TransmisionLineProperties : ITransmisionLineProperties
    {
        public string Name { get; set; }
        public double Voltage { get; set; }
        public string DispachingCode { get; set; }
        public string IgmcCode { get; set; }
        public int ToolId { get; set; }
        public double TransferCapacity { get; set; }
		public int LineFeederType { get; set; }
        public string SrcShapeName { get; set; }
        public string DesShapeName { get; set; }


        public TransmisionLineProperties()
        {
            this.SrcShapeName = string.Empty;
            this.DesShapeName = string.Empty;
        }
    }

    public class TransmisionLine: LineConnectShape
    {
        public TransmisionLine() : base()
        {
            this.Shape = new PolyLine();
            this.Properties = new TransmisionLineProperties();
        }
        public TransmisionLine(string name) : base(name)
        {
            this.Shape = new PolyLine();
            this.Properties = new TransmisionLineProperties();
        }
        public new TransmisionLineProperties Properties { get; set; }

        public int ConnectType { get; set; }

        public override string toSVG()
        {
            StringBuilder markup = new StringBuilder();
            var s = this.Shape as PolyLine;

            markup.Append("\t<g id=\"" + this.Id + "\">\n");

            for (int i = 0; i < s.Points.Count - 1; i++)
            {
                markup.Append(
                    "\t\t<line " +
                    "x1=\"" + s.Points[i].X + "\"" +
                    " y1=\"" + s.Points[i].Y + "\"" +
                    " x2=\"" + s.Points[i + 1].X + "\"" +
                    " y2=\"" + s.Points[i + 1].Y + "\"" +
                    " style=\"" + Util.Utilility.GetSvgStyles(this.Style) + "\"/>\n");
            }
            markup.Append("\t</g>\n");
            return markup.ToString();
        }
    }
}
