using Memora.BackEnd.Repositories.Models;
using Microsoft.EntityFrameworkCore;

namespace Memora.BackEnd.Repositories.DBContext;

public partial class PostgresContext : DbContext
{
    public PostgresContext()
    {
    }

    public PostgresContext(DbContextOptions<PostgresContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<AlbumPhoto> AlbumPhotos { get; set; }

    public virtual DbSet<AlbumTemplate> AlbumTemplates { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Door> Doors { get; set; }

    public virtual DbSet<Inventory> Inventories { get; set; }

    public virtual DbSet<InventoryItem> InventoryItems { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemDimension> ItemDimensions { get; set; }

    public virtual DbSet<Memory> Memories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderAlbum> OrderAlbums { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomItem> RoomItems { get; set; }

    public virtual DbSet<TemplatePage> TemplatePages { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserTheme> UserThemes { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("User Id=postgres.yzzispiaqactvbvsjwcw;Password=Hellomemora12345@;Server=aws-0-ap-southeast-1.pooler.supabase.com;Port=6543;Database=postgres;SSL Mode=Require;Trust Server Certificate=true;Keepalive=30;Timeout=15;CommandTimeout=30;Pooling=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("net", "request_status", new[] { "PENDING", "SUCCESS", "ERROR" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS" })
            .HasPostgresExtension("extensions", "pg_net")
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("albums_pkey");

            entity.ToTable("albums");

            entity.HasIndex(e => e.Id, "albums_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasDefaultValueSql("''::character varying")
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.TemplateId).HasColumnName("template_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Template).WithMany(p => p.Albums)
                .HasForeignKey(d => d.TemplateId)
                .HasConstraintName("albums_template_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Albums)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("albums_user_id_fkey");
        });

        modelBuilder.Entity<AlbumPhoto>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("album_photos_pkey");

            entity.ToTable("album_photos");

            entity.HasIndex(e => e.Id, "album_photos_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlbumId).HasColumnName("album_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasColumnType("character varying")
                .HasColumnName("image_url");
            entity.Property(e => e.PageNumber).HasColumnName("page_number");
            entity.Property(e => e.Position).HasColumnName("position");

            entity.HasOne(d => d.Album).WithMany(p => p.AlbumPhotos)
                .HasForeignKey(d => d.AlbumId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("album_photos_album_id_fkey");
        });

        modelBuilder.Entity<AlbumTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("album_templates_pkey");

            entity.ToTable("album_templates");

            entity.HasIndex(e => e.Id, "album_templates_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Id, "categories_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Door>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("doors_pkey");

            entity.ToTable("doors");

            entity.HasIndex(e => e.Id, "doors_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ColorHex).HasColumnName("color_hex");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.ImgUrl).HasColumnName("img_url");
        });

        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventories_pkey");

            entity.ToTable("inventories");

            entity.HasIndex(e => e.Id, "inventories_id_key").IsUnique();

            entity.HasIndex(e => e.UserId, "inventories_user_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.UserId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Inventory)
                .HasForeignKey<Inventory>(d => d.UserId)
                .HasConstraintName("inventories_user_id_fkey");
        });

        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("inventory_items_pkey");

            entity.ToTable("inventory_items");

            entity.HasIndex(e => e.Id, "inventory_items_id_key").IsUnique();

            entity.HasIndex(e => new { e.InventoryId, e.ItemId }, "inventory_items_inventory_id_item_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.InventoryId).HasColumnName("inventory_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'0'::bigint")
                .HasColumnName("quantity");

            entity.HasOne(d => d.Inventory).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.InventoryId)
                .HasConstraintName("inventory_items_inventory_id_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.InventoryItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("inventory_items_item_id_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("items_pkey");

            entity.ToTable("items");

            entity.HasIndex(e => e.Id, "items_id_key").IsUnique();

            entity.HasIndex(e => e.ItemImagePath, "items_item_image_path_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.DimensionId).HasColumnName("dimension_id");
            entity.Property(e => e.ItemImagePath)
                .HasColumnType("character varying")
                .HasColumnName("item_image_path");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.PuzzlePrice).HasColumnName("puzzle_price");
            entity.Property(e => e.ThemeId).HasColumnName("theme_id");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("items_category_id_fkey");

            entity.HasOne(d => d.Dimension).WithMany(p => p.Items)
                .HasForeignKey(d => d.DimensionId)
                .HasConstraintName("items_dimension_id_fkey");

            entity.HasOne(d => d.Theme).WithMany(p => p.Items)
                .HasForeignKey(d => d.ThemeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("items_theme_id_fkey");
        });

        modelBuilder.Entity<ItemDimension>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_dimension_pkey");

            entity.ToTable("item_dimension");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.H).HasColumnName("h");
            entity.Property(e => e.W).HasColumnName("w");
        });

        modelBuilder.Entity<Memory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("memories_pkey");

            entity.ToTable("memories");

            entity.HasIndex(e => e.Id, "memories_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.FilePath)
                .HasColumnType("character varying")
                .HasColumnName("file_path");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Title)
                .HasColumnType("character varying")
                .HasColumnName("title");

            entity.HasOne(d => d.Room).WithMany(p => p.Memories)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("memories_room_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");

            entity.ToTable("orders");

            entity.HasIndex(e => e.Id, "orders_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnName("total_price");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("orders_user_id_fkey");
        });

        modelBuilder.Entity<OrderAlbum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_albums_pkey");

            entity.ToTable("order_albums");

            entity.HasIndex(e => e.Id, "order_albums_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AlbumId).HasColumnName("album_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("'1'::bigint")
                .HasColumnName("quantity");

            entity.HasOne(d => d.Album).WithMany(p => p.OrderAlbums)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("order_albums_album_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderAlbums)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_albums_order_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Id, "roles_id_key").IsUnique();

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Rooms_pkey");

            entity.ToTable("rooms");

            entity.HasIndex(e => e.Id, "rooms_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.DoorId).HasColumnName("door_id");
            entity.Property(e => e.RoomName)
                .HasColumnType("character varying")
                .HasColumnName("room_name");
            entity.Property(e => e.ThemeId).HasColumnName("theme_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Door).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.DoorId)
                .HasConstraintName("rooms_door_id_fkey");

            entity.HasOne(d => d.Theme).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.ThemeId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("rooms_theme_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("rooms_user_id_fkey");
        });

        modelBuilder.Entity<RoomItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("room_items_pkey");

            entity.ToTable("room_items");

            entity.HasIndex(e => e.Id, "room_items_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.PosX).HasColumnName("pos_x");
            entity.Property(e => e.PosY).HasColumnName("pos_y");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.Rotation).HasColumnName("rotation");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserImageUrl)
                .HasColumnType("character varying")
                .HasColumnName("user_image_url");
            entity.Property(e => e.ZIndex)
                .HasDefaultValueSql("'0'::bigint")
                .HasColumnName("z_index");

            entity.HasOne(d => d.Item).WithMany(p => p.RoomItems)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("room_items_item_id_fkey");

            entity.HasOne(d => d.Room).WithMany(p => p.RoomItems)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("room_items_room_id_fkey");
        });

        modelBuilder.Entity<TemplatePage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("template_pages_pkey");

            entity.ToTable("template_pages", tb => tb.HasComment("chi tiết layout từng trang trong 1 template"));

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.LayoutUrl).HasColumnName("layout_url");
            entity.Property(e => e.PageNumber).HasColumnName("page_number");
            entity.Property(e => e.TemplatesId).HasColumnName("templates_id");

            entity.HasOne(d => d.Templates).WithMany(p => p.TemplatePages)
                .HasForeignKey(d => d.TemplatesId)
                .HasConstraintName("template_pages_templates_id_fkey");
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("themes_pkey");

            entity.ToTable("themes");

            entity.HasIndex(e => e.DoorId, "themes_door_id_key").IsUnique();

            entity.HasIndex(e => e.Id, "themes_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.DoorId).HasColumnName("door_id");
            entity.Property(e => e.FloorUrl)
                .HasColumnType("character varying")
                .HasColumnName("floor_url");
            entity.Property(e => e.ThemeName)
                .HasColumnType("character varying")
                .HasColumnName("theme_name");
            entity.Property(e => e.ThemePrice).HasColumnName("theme_price");
            entity.Property(e => e.WallUrl)
                .HasColumnType("character varying")
                .HasColumnName("wall_url");

            entity.HasOne(d => d.Door).WithOne(p => p.Theme)
                .HasForeignKey<Theme>(d => d.DoorId)
                .HasConstraintName("themes_door_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "Users_email_key").IsUnique();

            entity.HasIndex(e => e.Username, "Users_username_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasColumnType("character varying")
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasColumnType("character varying")
                .HasColumnName("fullname");
            entity.Property(e => e.PasswordHash)
                .HasColumnType("character varying")
                .HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber)
                .HasColumnType("character varying")
                .HasColumnName("phone_number");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Username)
                .HasColumnType("character varying")
                .HasColumnName("username");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        modelBuilder.Entity<UserTheme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_themes_pkey");

            entity.ToTable("user_themes");

            entity.HasIndex(e => e.Id, "user_themes_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.ThemeId).HasColumnName("theme_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Theme).WithMany(p => p.UserThemes)
                .HasForeignKey(d => d.ThemeId)
                .HasConstraintName("user_themes_theme_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UserThemes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_themes_user_id_fkey");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("payments_pkey");

            entity.ToTable("wallets");

            entity.HasIndex(e => e.UserId, "wallets_user_id_key").IsUnique();

            entity.Property(e => e.Id)
                .HasColumnType("character varying")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("created_at");
            entity.Property(e => e.Puzzles).HasColumnName("puzzles");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(now() AT TIME ZONE 'utc'::text)")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithOne(p => p.Wallet)
                .HasForeignKey<Wallet>(d => d.UserId)
                .HasConstraintName("wallets_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
