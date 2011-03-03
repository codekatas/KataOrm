using System;
using System.IO;
using System.Xml;
using KataOrm.Infrastructure;
using KataOrm.Test.ConcernsHelper;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KataOrm.Test.Infrastructure.Log4NetSpecs
{
    [TestClass]
    public class Log4NetInitializationCommandSpecs : InstanceContextSpecification<ICommand>
    {
        private XmlElement log4NetCofigElement;

        protected override void EstablishContext()
        {
            log4NetCofigElement = Log4NetConfigSection.CreateSectionToTarget(GetLocalLogingFile());
        }

        protected override void AfterEachTest()
        {
            base.AfterEachTest();
            File.Delete(GetLocalLogingFile());
        }

        private string GetLocalLogingFile()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
        }

        protected override void Because()
        {
            Sut.run();
        }

        protected override ICommand CreateSut()
        {
            return new Log4NetInitializationCommand(Log4NetConfigSection.CreateSectionToTarget(GetLocalLogingFile()));
        }

        [TestMethod]
        public void Should_be_able_to_log_to_the_file_specified_in_the_configuration_element()
        {
            LogManager.GetLogger(GetType()).Info("Hello");
            LogManager.Shutdown();
            Assert.IsTrue(File.Exists(GetLocalLogingFile()));
        }
    }

    public class Log4NetConfigSection
    {
        private const string log4netFormatString =
            @"<log4net>
        <appender name='RollingFileAppender' type='log4net.Appender.RollingFileAppender'>
            <file value='{0}' />
            <appendToFile value='true' />
            <rollingStyle value='Size' />
            <maxSizeRollBackups value='10' />
            <maximumFileSize value='100000KB' />
            <staticLogFileName value='true' />
            <layout type='log4net.Layout.PatternLayout'>
                <conversionPattern value='%d [%t] %-5p %c - %m%n' />
            </layout>
        </appender>

        <root>
            <level value='DEBUG' />
            <appender-ref ref='RollingFileAppender' />			
        </root>
    </log4net>
";


        public static XmlElement CreateSectionToTarget(string localLogingFile)
        {
            var document = new XmlDocument();
            document.LoadXml(string.Format(log4netFormatString, localLogingFile));
            return document.DocumentElement;
        }
    }
}