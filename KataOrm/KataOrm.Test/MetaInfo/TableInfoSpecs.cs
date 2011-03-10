using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using KataOrm.MetaStore;
using KataOrm.Test.ConcernsHelper;
using KataOrm.Test.Helper;
using KataOrm.Test.SpecHelpers;
using KataTestAssembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.MetaInfo
{
    [TestClass]
    public class When_getting_insert_statement_parameters   : InstanceContextSpecification<TableInfo>
    {
        private Product _productToSave;
        private MetaInfoStore _metaStore;
        private List<AdoParameterInfo> _insertStatementParams;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            var testBase = new TestBase();

            _productToSave = new Product()
            {
                Name = "First Product",
                Description = "Product Description",
                SellStartDate = DateTime.Now,
                SellEndDate = DateTime.Now.AddYears(1)
            };
            _metaStore = testBase.GetMetaInfo();
        }
        protected override void Because()
        {
            _insertStatementParams = Sut.GetParametersForInsert(_productToSave);
        }

        protected override TableInfo CreateSut()
        {
            return _metaStore.TableInfos.Where(x => x.Value.TableName == "Product").First().Value;
        }


        [TestMethod]
        public void Should_return_the_correct_number_of_parameters_as_there_are_fields_in_the_entity()
        {
            _insertStatementParams.Count().ShouldBeEqualTo(4);
        }
    }
}