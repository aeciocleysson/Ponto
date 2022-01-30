namespace MegaPonto.ViewModel
{
    public abstract class BaseViewModel
    {
        public int? Id { get; set; }
        public int Inserted { get; set; }
        public int IsDelete { get; set; }
    }
}
