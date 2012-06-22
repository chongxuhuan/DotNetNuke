#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2012
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using DotNetNuke.Common.Internal;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Portals.Internal;
using DotNetNuke.Web.Services;
using DotNetNuke.Web.Services.Internal;
using Moq;
using NUnit.Framework;
using ServicesRoutingManager = DotNetNuke.Web.Services.Internal.ServicesRoutingManager;

namespace DotNetNuke.Tests.Web.Services
{
    [TestFixture]
    public class ServiceRoutingManagerTests
    {
        // ReSharper disable UnusedMember.Local
        private List<string[]> _emptyStringArrays = new List<string[]>
                                                        {null, new string[0], new[] {""}, new string[] {null}};
        // ReSharper restore UnusedMember.Local
        private Mock<IPortalController> _mockPortalController;
        private IPortalController _portalController;

        [SetUp]
        public void Setup()
        {
            FakeServiceRouteMapper.RegistrationCalls = 0;

            _mockPortalController = new Mock<IPortalController>();
            _portalController = _mockPortalController.Object;
            TestablePortalController.SetTestableInstance(_portalController);
        }

        [Test]
        public void LocatesAllServiceRouteMappers()
        {
            var assemblyLocator = new Mock<IAssemblyLocator>();

            //including the assembly with object ensures that the assignabliity is done correctly
            var assembliesToReflect = new IAssembly[2];
            assembliesToReflect[0] = new AssemblyWrapper(GetType().Assembly);
            assembliesToReflect[1] = new AssemblyWrapper(typeof (Object).Assembly);

            assemblyLocator.Setup(x => x.Assemblies).Returns(assembliesToReflect);

            var locator = new TypeLocator {AssemblyLocator = assemblyLocator.Object};

            List<Type> types = locator.GetAllMatchingTypes(ServicesRoutingManager.IsValidServiceRouteMapper).ToList();

            //if new ServiceRouteMapper classes are added to the assembly they willl likely need to be added here
            CollectionAssert.AreEquivalent(
                new[]
                    {
                        typeof (FakeServiceRouteMapper),
                        typeof (ReflectedServiceRouteMappers.EmbeddedServiceRouteMapper),
                        typeof (ExceptionOnCreateInstanceServiceRouteMapper),
                        typeof (ExceptionOnRegisterServiceRouteMapper)
                    }, types);
        }



        [Test]
        public void NameSpaceRequiredOnMapRouteCalls([ValueSource("_emptyStringArrays")] string[] namespaces)
        {
            var srm = new ServicesRoutingManager(new RouteCollection());

            Assert.Throws<ArgumentException>(() => srm.MapRoute("usm", "default", "url", null, namespaces));
        }

        [Test]
        public void RegisterRoutesIsCalledOnAllServiceRouteMappersEvenWhenSomeThrowExceptions()
        {
            FakeServiceRouteMapper.RegistrationCalls = 0;
            var assembly = new Mock<IAssembly>();
            assembly.Setup(x => x.GetTypes()).Returns(new[]
                                                          {
                                                              typeof (ExceptionOnRegisterServiceRouteMapper),
                                                              typeof (ExceptionOnCreateInstanceServiceRouteMapper),
                                                              typeof (FakeServiceRouteMapper)
                                                          });
            var al = new Mock<IAssemblyLocator>();
            al.Setup(x => x.Assemblies).Returns(new[] {assembly.Object});
            var tl = new TypeLocator {AssemblyLocator = al.Object};
            var srm = new ServicesRoutingManager(new RouteCollection()) {TypeLocator = tl};

            srm.RegisterRoutes();

            Assert.AreEqual(1, FakeServiceRouteMapper.RegistrationCalls);
        }

        [Test]
        public void RegisterRoutesIsCalledOnServiceRouteMappers()
        {
            FakeServiceRouteMapper.RegistrationCalls = 0;
            var assembly = new Mock<IAssembly>();
            assembly.Setup(x => x.GetTypes()).Returns(new[] {typeof (FakeServiceRouteMapper)});
            var al = new Mock<IAssemblyLocator>();
            al.Setup(x => x.Assemblies).Returns(new[] {assembly.Object});
            var tl = new TypeLocator {AssemblyLocator = al.Object};
            var srm = new ServicesRoutingManager(new RouteCollection()) {TypeLocator = tl};

            srm.RegisterRoutes();

            Assert.AreEqual(1, FakeServiceRouteMapper.RegistrationCalls);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        public void UniqueNameRequiredOnMapRouteCalls(string uniqueName)
        {
            var srm = new ServicesRoutingManager(new RouteCollection());

            Assert.Throws<ArgumentNullException>(() => srm.MapRoute(uniqueName, "default", "url", null, new[] {"foo"}));
        }

        [Test]
        public void UrlCanStartWithSlash()
        {
            //Arrange
            _mockPortalController.Setup(x => x.GetPortals()).Returns(new ArrayList());
            
            //Act
            var srm = new ServicesRoutingManager(new RouteCollection());

            //Assert
            Assert.DoesNotThrow(() => srm.MapRoute("name", "/url", null, new[] { "foo" }));
        }

        [Test]
        public void SimpleDomainMakes1Route()
        {
            //Arrange
            var portalInfo = new ArrayList {new PortalInfo {PortalID = 0}};
            _mockPortalController.Setup(x => x.GetPortals()).Returns(portalInfo);
            var mockPac = new Mock<IPortalAliasController>();
            mockPac.Setup(x => x.GetPortalAliasByPortalId(0)).Returns(new[] {new PortalAliasInfo{HTTPAlias = "www.foo.com"}});
            TestablePortalAliasController.SetTestableInstance(mockPac.Object);
            
            var routeCollection = new RouteCollection();
            var srm = new ServicesRoutingManager(routeCollection);

            //Act
            srm.MapRoute("folder", "url", new[] {"foo"});

            //Assert
            Assert.AreEqual(1, routeCollection.Count);
        }

        [Test]
        public void VirtualDirOnlyMakesRouteWithNoWildcardPrefix()  
        {
            //Arrange
            var portalInfo = new ArrayList { new PortalInfo { PortalID = 0 } };
            _mockPortalController.Setup(x => x.GetPortals()).Returns(portalInfo);

            var mockPac = new Mock<IPortalAliasController>();
            mockPac.Setup(x => x.GetPortalAliasByPortalId(0)).Returns(new[] { new PortalAliasInfo { HTTPAlias = "www.foo.com/vdir" } });
            TestablePortalAliasController.SetTestableInstance(mockPac.Object);

            var mockGlobals = new Mock<IGlobals>();
            mockGlobals.Setup(x => x.ApplicationPath).Returns("/vdir");
            TestableGlobals.SetTestableInstance(mockGlobals.Object);

            var routeCollection = new RouteCollection();
            var srm = new ServicesRoutingManager(routeCollection);

            //Act
            srm.MapRoute("folder", "url", new[] { "foo" });

            //Assert
            Assert.AreEqual(1, routeCollection.Count);
            var route = (Route)routeCollection[0];
            StringAssert.DoesNotStartWith("{", route.Url);
        }

        [Test]
        public void VirtualDirWithChild()
        {
            //Arrange
            var portalInfo = new ArrayList { new PortalInfo { PortalID = 0 } };
            _mockPortalController.Setup(x => x.GetPortals()).Returns(portalInfo);

            var mockPac = new Mock<IPortalAliasController>();
            mockPac.Setup(x => x.GetPortalAliasByPortalId(0)).Returns(new[] { new PortalAliasInfo { HTTPAlias = "www.foo.com/vdir" }, new PortalAliasInfo{HTTPAlias = "www.foo.com/vdir/child"} });
            TestablePortalAliasController.SetTestableInstance(mockPac.Object);

            var mockGlobals = new Mock<IGlobals>();
            mockGlobals.Setup(x => x.ApplicationPath).Returns("/vdir");
            TestableGlobals.SetTestableInstance(mockGlobals.Object);

            var routeCollection = new RouteCollection();
            var srm = new ServicesRoutingManager(routeCollection);

            //Act
            srm.MapRoute("folder", "url", new[] { "foo" });

            //Assert
            Assert.AreEqual(2, routeCollection.Count);
            var urls = routeCollection.Cast<Route>().Select(x => x.Url).ToList();
            Assert.AreEqual(1, urls.Count(x => x.Contains("{prefix0}")));
            //if 2 urls and only 1 has prefix 0 then the other has no prefixesed wcon
        }

        [Test]
        public void NameIsInsertedInRouteDataTokens()
        {
            //Arrange
            var portalInfo = new ArrayList { new PortalInfo { PortalID = 0 } };
            _mockPortalController.Setup(x => x.GetPortals()).Returns(portalInfo);
            var mockPac = new Mock<IPortalAliasController>();
            mockPac.Setup(x => x.GetPortalAliasByPortalId(0)).Returns(new[] { new PortalAliasInfo { HTTPAlias = "www.foo.com" } });
            TestablePortalAliasController.SetTestableInstance(mockPac.Object);

            var routeCollection = new RouteCollection();
            var srm = new ServicesRoutingManager(routeCollection);

            //Act
            srm.MapRoute("folder", "url", new[] { "foo" });

            //Assert
            var route = (Route) routeCollection[0];
            Assert.AreEqual("folder-route0-instance0", route.DataTokens["Name"]);
        }

        [Test]
        public void TwoRoutesOnTheSameFolderHaveSimilarNames()
        {
            //Arrange
            var portalInfo = new ArrayList { new PortalInfo { PortalID = 0 } };
            _mockPortalController.Setup(x => x.GetPortals()).Returns(portalInfo);
            var mockPac = new Mock<IPortalAliasController>();
            mockPac.Setup(x => x.GetPortalAliasByPortalId(0)).Returns(new[] { new PortalAliasInfo { HTTPAlias = "www.foo.com" } });
            TestablePortalAliasController.SetTestableInstance(mockPac.Object);

            var routeCollection = new RouteCollection();
            var srm = new ServicesRoutingManager(routeCollection);

            //Act
            srm.MapRoute("folder", "url", new[] { "foo" });
            srm.MapRoute("folder", "alt/url", new[] { "foo" });

            //Assert
            var route = (Route)routeCollection[0];
            Assert.AreEqual("folder-route0-instance0", route.DataTokens["Name"]);
            route = (Route)routeCollection[1];
            Assert.AreEqual("folder-route1-instance0", route.DataTokens["Name"]);
        }
    }
}