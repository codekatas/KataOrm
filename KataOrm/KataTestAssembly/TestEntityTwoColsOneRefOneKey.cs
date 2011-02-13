using KataOrm.Attributes;

namespace KataTestAssembly
{
    [Table("TableInfoTestsOne")]
    public class TestEntityTwoColsOneRefOneKey
    {
        [PrimaryKey("TableKey")]
        public int TableKey { get; set; }

        [ReferenceColumn("ReferenceOne")]
        public TestEntityOneKeyTwoCols ReferenceOne { get; set; }

        [Column("ColumnOne")]
        public string ColumnOne { get; set; }

        [Column("ColumnTwo")]
        public string ColumnTwo { get; set; }

    }
}