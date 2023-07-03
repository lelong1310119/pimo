using PismoWebInput.Core.Infrastructure.Models.Master;
using PismoWebInput.Core.Infrastructure.Models.User;

namespace PismoWebInput.Core.Infrastructure.Models.UserManager
{
    public class CreateUserManagerModel
    {
        public CreateUserModel User { get; set; }
        public List<List<MasterModel>> Operators { get; set; }

        public CreateUserManagerModel()
        {
            User = new CreateUserModel();
            Operators = new List<List<MasterModel>>();
        }
    }
}
