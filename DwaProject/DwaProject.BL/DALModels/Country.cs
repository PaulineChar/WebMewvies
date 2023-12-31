﻿using System;
using System.Collections.Generic;

namespace DwaProject.BL.DALModels;

public partial class Country
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
