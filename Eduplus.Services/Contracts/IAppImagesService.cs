using Eduplus.Domain.CoreModule;
using System.Collections.Generic;

namespace Eduplus.Services.Contracts
{
    public interface IAppImagesService
    {
        List<AppImages> GetAllSlideImages();
        void DeleteSlideImage(string imageId, string userId);
        byte[] GetStudentPhoto(string userCode);
        void SaveImage(AppImages model, string userId);
    }
}
