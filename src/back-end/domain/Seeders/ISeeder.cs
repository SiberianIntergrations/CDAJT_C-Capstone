namespace back_end.Domain.Seeders
{
    /// <summary>
    /// Interface for database seeders.
    /// </summary>
    public interface ISeeder
    {
        /// <summary>
        /// Seeds the database with initial data.
        /// </summary>
        void Seed();
    }
}