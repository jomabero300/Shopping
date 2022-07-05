namespace TSShopping.Helpers
{
    public interface IImageHelper
    {
        Task<string> DeleteImageAsync(string File,string Folder);
        string DeleteImageAsync(string File);        
        Task<string> UploadFileAsync(IFormFile ProFile, string Folder);
        Task<Guid> UploadImageAsync(IFormFile ProFile, string Folder);
    }
}