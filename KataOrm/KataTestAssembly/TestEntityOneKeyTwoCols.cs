using KataOrm.Attributes;

namespace KataTestAssembly
{
    [Table("TestEntityOneKeyTwoCols")]
    public class TestEntityOneKeyTwoCols
    {

        [PrimaryKey("TableKey")]
        public int TableKey { get; set; }

        [Column("ColumnOne")]
        public string ColumnOne { get; set; }

        [Column("ColumnTwo")]
        public string ColumnTwo { get; set; }

    }
}