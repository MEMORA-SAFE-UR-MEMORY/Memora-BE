using System;
using System.Collections.Generic;

namespace Memora.BackEnd.Repositories.Models;

/// <summary>
/// chi tiết layout từng trang trong 1 template
/// </summary>
public partial class TemplatePage
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public long PageNumber { get; set; }

    public string LayoutUrl { get; set; } = null!;

    public long TemplatesId { get; set; }

    public virtual AlbumTemplate Templates { get; set; } = null!;
}
