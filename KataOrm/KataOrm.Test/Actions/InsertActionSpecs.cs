using System;
using System.Data.SqlClient;
using System.Reflection;
using KataOrm.CrudActions;
using KataOrm.MetaStore;
using KataOrm.Test.ConcernsHelper;
using KataOrm.Test.Helper;
using KataOrm.Test.SpecHelpers;
using KataTestAssembly;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.Actions
{
    [TestClass]
    public class InsertActionSpecs : InstanceContextSpecification<IInsertAction>
    {
        private SqlConnection _connection;
        private MetaInfoStore _metaStore;
        private Product _productToSave;
        private SqlTransaction _transaction;
        private Assembly _assembly;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            var testBase = new TestBase();

            _productToSave = new Product()
                                 {
                                     ProductName = "First Product",
                                     Description = "Product Description",
                                     SellStartDate = DateTime.Now,
                                     SellEndDate = DateTime.Now.AddYears(1)
                                 };
            _connection = new SqlConnection(new HardCodedTestConfigurationSettings().ConnectionSettings);
            _metaStore = testBase.GetMetaInfo();
        }

        protected override void Because()
        {
            Sut.Insert(_productToSave);
        }

        protected override IInsertAction CreateSut()
        {
            return new InsertAction(_connection, _transaction, _metaStore);
        }

        [TestMethod]
        public void persisted_entity_should_have_the_identity_field_populated()
        {
            Assert.IsTrue(_productToSave.Id > 0);
        }
    }
}