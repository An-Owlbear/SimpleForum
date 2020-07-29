using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleForum.Models
{
    public interface IPost
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public bool Deleted { get; set; }
    }
}