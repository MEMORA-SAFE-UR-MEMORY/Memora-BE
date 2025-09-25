using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

public partial class AlbumTemplate
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Album> Albums { get; set; } = new List<Album>();

    public virtual ICollection<TemplatePage> TemplatePages { get; set; } = new List<TemplatePage>();
}
