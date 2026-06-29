namespace API.Interfaces
{
    public interface IIdoResponse<T>
    {
        List<T> Items { get; set; }
        string Bookmark { get; set; }
        bool MoreRowsExist { get; set; }
        bool Success { get; set; }
        string Message { get; set; }
    }

}
