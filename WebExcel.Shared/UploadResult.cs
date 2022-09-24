namespace WebExcel.Shared
{
    public class UploadResult
    {
        public Dictionary<string, List<string>>? Data { get; set; }
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public int ErrorCode { get; set; }
    }
}