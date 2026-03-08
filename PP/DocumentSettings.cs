namespace PP
{
    public static class DocumentSettings
    {
        public static string UploadFile(IFormFile File, string FolderName)
        {
            var FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", FolderName);
            if (!Directory.Exists(FolderPath))
            {
                Directory.CreateDirectory(FolderPath);
            }
            var FileNAme = $"{Guid.NewGuid()}{Path.GetExtension(File.FileName)}";
            var FilePath = Path.Combine(FolderPath, FileNAme);

            using var stream = new FileStream(FilePath, FileMode.Create);
            File.CopyTo(stream);
            return FileNAme;
        }

        public static void DeleteFile(string FileName, string FolderName)
        {
            var FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", FolderName, FileName);
            if (File.Exists(FilePath))
            {
                File.Delete(FilePath);
            }
        }
    }
}