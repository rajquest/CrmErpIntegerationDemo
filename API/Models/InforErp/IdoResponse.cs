using API.Interfaces;

namespace API.Models.InforErp
{
    public class IdoResponse<T> : IIdoResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public string? Bookmark { get; set; }
        public bool MoreRowsExist { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

}
