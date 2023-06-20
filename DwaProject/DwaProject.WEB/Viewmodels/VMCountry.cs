namespace DwaProject.WEB.Viewmodels
{
    public class VMCountry
    {
        public int? Id { get; set; }

        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;

        public override string ToString()
        {
            return Name + " (" + Code + ")";
        }
    }
}
