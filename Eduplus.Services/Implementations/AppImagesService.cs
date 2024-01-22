using Eduplos.Domain.CoreModule;
using Eduplos.Services.Contracts;
using KS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eduplos.Services.Implementations
{
    public class AppImagesService:IAppImagesService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AppImagesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<AppImages> GetAllSlideImages()
        {
            return _unitOfWork.AppImagesRepository.GetFiltered(a=>a.IncludeInSlide==true).ToList();
            
        }

        public void DeleteSlideImage(string imageId, string userId)
        {
            var dbImage = _unitOfWork.AppImagesRepository.Get(imageId);
            
            _unitOfWork.AppImagesRepository.Remove(dbImage);
            _unitOfWork.Commit(userId);
        }

        public void SaveImage(AppImages model, string userId)
        {
           
            model.Foto=null;
            _unitOfWork.AppImagesRepository.Add(model);
            _unitOfWork.Commit(userId);
        }



        public byte[] GetStudentPhoto(string userCode)
        {

            var st = _unitOfWork.StudentRepository.GetSingle(a => a.PersonId == userCode && !string.IsNullOrEmpty(a.PhotoId));
            if (st != null)
            {
                var chk = st.Photo.Foto;
                return chk;
            }

            else return null;

        }
    }
}
