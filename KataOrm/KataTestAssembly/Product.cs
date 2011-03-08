using System;
using KataOrm.Attributes;

namespace KataTestAssembly
{
    [Table("Product")]
    public class Product
    {
        [PrimaryKey("ID")]
        public int Id { get; set; }

        [Column("Name")]
        public string Name { get; set; }

        [Column("Description")]
        public string Description { get; set; }

        [Column("SellStartDate")]
        public DateTime SellStartDate { get; set; }

        [Column("SellEndDate")]
        public DateTime SellEndDate { get; set; }
    }
}