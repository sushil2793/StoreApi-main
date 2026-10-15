namespace SmartMeterAdmin.ViewModel.Master
{
    public class TableColumnDto
    {
        public int TableId { get; set; }
        public int UserColumnId { get; set; }
        public int TemplateId { get; set; }
        public string ColumnDescription { get; set; }
        public string ColumnName { get; set; }
        public bool IsSelected { get; set; }
        public bool IsImageColumn { get; set; }
    }
}
