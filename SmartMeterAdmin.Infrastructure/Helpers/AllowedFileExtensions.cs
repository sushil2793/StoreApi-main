namespace SmartMeterAdmin.Infrastructure.Helpers
{
    public static class AllowedFileExtensions
    {
        public static string[] GetAllowedFile(string fileType)
        {
            return fileType switch
            {
                "Excel" => GetExcelType(),
                "Image" => GetImagesType(),
                _ => null,
            };
        }

        private static string[] GetImagesType()
        {
            return new string[] {
                ".png",
                ".jpg",
                ".jpeg"
            };
        }
        private static string[] GetExcelType()
        {
            return new string[] {
                ".xlsx",
                ".xls"
            };
        }
    }
}
