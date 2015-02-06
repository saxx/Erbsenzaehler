using Erbsenzaehler.Models;

namespace Erbsenzaehler.ViewModels.ManageUsers
{
    public class DeleteViewModel
    {
        public DeleteViewModel Fill(Db db, Models.User user, Models.User currentUser)
        {
            Email = user.Email;
            IsCurrentUser = user.Id == currentUser.Id;
            return this;
        }


        public bool IsCurrentUser { get; set; }
        public string Email { get; set; }
    }
}