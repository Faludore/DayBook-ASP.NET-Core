using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using WebApiAngularIdentity.Controllers;
using WebApiAngularIdentity.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc.Core;
using System.Linq;
using System.Collections.Generic;
using Record = WebApiAngularIdentity.Models.Record;

namespace RecordUnitTest
{
    public class UnitTest1
    {   
        [Fact]
        public void Get_All_Records ()
        {
            var authenticationContext = new Mock<AuthenticationContext>();
            authenticationContext.Setup(repo => repo.Records.ToList()).Returns(GetTestRecords());
            var userManager = new Mock<UserManager<ApplicationUser>>();       
            var apicontroller = new RecordsController(userManager.Object, authenticationContext.Object);

            var list = apicontroller.GetRecords();

            Assert.NotNull(list);
        }
        private List<Record> GetTestRecords()
        {
            var records = new List<Record>
            {
                new Record { Id=1, Title ="title1", Text = "text1", DnT = "21.07.2020", Image = null, User = null, UserId = null },
                new Record { Id=2, Title ="title2", Text = "text2", DnT = "24.07.2020", Image = null, User = null, UserId = null },
                new Record { Id=3, Title ="title3", Text = "text3", DnT = "11.07.2020", Image = null, User = null, UserId = null },
                new Record { Id=4, Title ="title4", Text = "text4", DnT = "01.07.2020", Image = null, User = null, UserId = null },
                new Record { Id=5, Title ="title5", Text = "text5", DnT = "10.07.2020", Image = null, User = null, UserId = null }
            };
            return records;
        }
    }
}
