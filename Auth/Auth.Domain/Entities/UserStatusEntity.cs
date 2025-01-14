using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.Entities
{
    public partial class UserStatusEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<UserEntity> Users { get; set; }
    }
}