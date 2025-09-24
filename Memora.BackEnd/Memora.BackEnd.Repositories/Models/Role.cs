using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class Role
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
