using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using KataOrm.Test.ConcernsHelper;
using KataOrm.Test.SpecHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.Helper
{
    [TestClass]
    public class Concerning_Kata_schema_manager : InstanceContextSpecification<IKataSchemaManager>
    {
        private string _sqlStatment;
        private IEnumerable<string> _result;
        private Assembly _assembly;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            _assembly = Assembly.GetExecutingAssembly();
            _sqlStatment =
            @"SET ANSI_NULLS ON
            GO
            SET QUOTED_IDENTIFIER ON
            GO
            CREATE TABLE [dbo].[TestEntityOneKeyTwoCols](
            [TableKey] [Int] IDENTITY(1,1) NOT NULL, 
            [ColumnOne] [VarChar] NULL, 
            [ColumnTwo] [VarChar] NULL, 
            CONSTRAINT [PK_TableKey] PRIMARY KEY CLUSTERED 
            ( [TableKey] ASC )
            WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
            )
            ON [PRIMARY]
            GO
";
        }

        protected override void Because()
        {
            _result = Sut.GetBatchesFromSqlStatement(_sqlStatment);
        }

        protected override IKataSchemaManager CreateSut()
        {
            return new KataSchemaManager(_assembly, new HardCodedTestConfigurationSettings());
        }

        [TestMethod]
        public void should_return_a_batch_of_3_statments()
        {
            _result.Count().ShouldBeEqualTo(3);
        }

    }
}