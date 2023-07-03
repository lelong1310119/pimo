using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Infrastructure.Models.User;

namespace PismoWebInput.Core.Infrastructure.Models.UserManager
{
    public class EditUserManagerModel
    {
        public EditUserModel User { get; set; }
        public List<List<MasterModel>> Operators { get; set; }

        public EditUserManagerModel()
        {
            User = new EditUserModel();
            Operators = new List<List<MasterModel>>();
        }
    }
}
