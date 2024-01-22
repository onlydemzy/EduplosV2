using Eduplos.Domain.CoreModule;
using System.Collections.Generic;

namespace Eduplos.Services.Contracts
{
    public interface IAppImagesService
    {
        List<AppImages> GetAllSlideImages();
        void DeleteSlideImage(string imageId, string userId);
        byte[] GetStudentPhoto(string userCode);
        void SaveImage(AppImages model, string userId);
    }
}
