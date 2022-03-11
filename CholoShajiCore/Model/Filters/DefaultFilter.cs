namespace CholoShajiCore.Model.Filters
{
    public class GridFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        private Sorting Sort { get; set; }
        private BetweenFilter Between { get; set; }

    }

    public class Sorting
    {
        public string  FieldName { get; set; }
        public string FieldType { get; set; }
        public bool IsAscending { get; set; }
    }

    public class BetweenFilter
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public object Value1 { get; set; }
        public object Value2 { get; set; }

    }

}
