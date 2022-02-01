using System;

namespace RMDataManager.Library.Internal.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAdress { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
