using KataOrm.Attributes;

namespace KataOrm.Test.TestData
{
    [Table("TableInfoTestsOne")]
    public class TableInfoTestsOne
    {
        [PrimaryKey("TableKey")]
        public int TableKey { get; set; } 

        [Column("ColumnOne")]
        public string ColumnOne { get; set; }
        
        [Column("ColumnTwo")]
        public string ColumnTwo { get; set; }

    }

    [Table("TableInfoTestsTwo")]
    public class TableInfoTestsTwo
    {

        [ReferenceColumn("ReferenceOne")]
        public TableInfoTestsOne ReferenceOne { get; set; }

        [PrimaryKey("TableKey")]
        public int TableKey { get; set; }

        [Column("ColumnOne")]
        public string ColumnOne { get; set; }

    }

    [Table("TableInfoTestsThree")]
    public class TableInfoTestsThree
    {
        [PrimaryKey("TableKey")]
        public int TableKey { get; set; }

        [Column("ColumnOne")]
        public string ColumnOne { get; set; }
    }

    [Table("TableInfoTestsFour")]
    public class TableInfoTestsFour
    {
        [PrimaryKey("TableKey")]
        public int TableKey { get; set; }

        [ReferenceColumn("ReferenceOne")]
        public TableInfoTestsOne ReferenceOne { get; set; }

        [Column("ColumnOne")]
        public string ColumnOne { get; set; }

        [Column("ColumnTwo")]
        public string ColumnTwo { get; set; }
    }
}