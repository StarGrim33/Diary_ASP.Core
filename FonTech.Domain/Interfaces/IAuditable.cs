namespace FonTech.Domain.Interfaces
{
    public interface IAuditable
    {
        public DateTime CreatedAt { get; set; }

        public long CreatedBy { get; set; }

        public DateTime LastModifiedAt { get; set; }

        public long UpdatedBy { get; set; }
    }
}
