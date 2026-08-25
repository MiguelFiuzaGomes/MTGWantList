using Microsoft.EntityFrameworkCore;
using MTGWantList.Models.Catalogue;

namespace MTGWantList.Data;

public class AppDbContext : DbContext
{

    // ASP.NET will provide the database configuration
    // when it creates this context through dependency injection.
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // Represents the Magic sets stored in our catalogue database.
    public DbSet<CardSet> CardSets { get; set; }

    // Represents the unique cards in our catalogue,
    // independent of set or printing.
    public DbSet<Card> Cards { get; set; }

    // Represents individual printings of cards within specific sets.
    public DbSet<Printing> Printings { get; set; }

    // Represents the concrete language + finish variants
    // of individual card printings.
    public DbSet<PrintingVariant> PrintingVariants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ------------------------------------------------------------
        // CardSet
        // ------------------------------------------------------------

        // A Magic set code should only exist once in our catalogue.
        // For example, there must never be two separate "FDN" sets.
        modelBuilder.Entity<CardSet>()
            .HasIndex(cardSet => cardSet.MtgJsonCode)
            .IsUnique();


        // ------------------------------------------------------------
        // Card
        // ------------------------------------------------------------

        // The Oracle ID identifies the underlying Magic card across
        // different printings, so duplicate non-null values are not allowed.
        //
        // PostgreSQL's partial index lets multiple cards have a NULL value
        // while still enforcing uniqueness whenever an Oracle ID exists.
        modelBuilder.Entity<Card>()
            .HasIndex(card => card.ScryfallOracleId)
            .IsUnique()
            .HasFilter("\"ScryfallOracleId\" IS NOT NULL");


        // ------------------------------------------------------------
        // Printing
        // ------------------------------------------------------------

        // MTGJSON UUIDs identify individual printings and therefore
        // must be unique in our catalogue.
        modelBuilder.Entity<Printing>()
            .HasIndex(printing => printing.MtgJsonUuid)
            .IsUnique();

        // Every Printing belongs to exactly one Card.
        //
        // Prevent a Card from being deleted while printings still reference it.
        // This protects catalogue and historical data from accidental deletion.
        modelBuilder.Entity<Printing>()
            .HasOne(printing => printing.Card)
            .WithMany()
            .HasForeignKey(printing => printing.CardId)
            .OnDelete(DeleteBehavior.Restrict);

        // Every Printing belongs to exactly one CardSet.
        modelBuilder.Entity<Printing>()
            .HasOne(printing => printing.CardSet)
            .WithMany()
            .HasForeignKey(printing => printing.CardSetId)
            .OnDelete(DeleteBehavior.Restrict);


        // ------------------------------------------------------------
        // PrintingVariant
        // ------------------------------------------------------------

        // The same physical combination should never be inserted twice.
        //
        // Example:
        // FDN #227 + English + foil
        // should have exactly one PrintingVariant record.
        modelBuilder.Entity<PrintingVariant>()
            .HasIndex(variant => new
            {
                variant.PrintingId,
                variant.Language,
                variant.Finish
            })
            .IsUnique();

        // Every variant belongs to exactly one printing.
        modelBuilder.Entity<PrintingVariant>()
            .HasOne(variant => variant.Printing)
            .WithMany()
            .HasForeignKey(variant => variant.PrintingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}